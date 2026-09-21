using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerOfflineMovement : MonoBehaviour
{
    [Header("Player References")]
    [Tooltip("Reads movement and look input for the local player.")]
    [SerializeField] private InputReader inputReader;
    [Tooltip("Rigidbody moved by the player controller.")]
    [SerializeField] private Rigidbody playerRigidbody;
    [Tooltip("Model rotated to face the movement direction.")]
    [SerializeField] private GameObject playerModel;
    [Tooltip("Updates movement and jump animations.")]
    [SerializeField] private PlayerOfflineAnimationManager playerAnimationManager;
    [Tooltip("Controls movement-related player effects.")]
    [SerializeField] private PlayerEffectsManager playerEffectsManager;
    [SerializeField] private PlayerOfflineManager playerManager;
    [Tooltip("Player transform used as the camera position anchor.")]
    [SerializeField] private Transform player;
    [Tooltip("Pivot rotated by look input.")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private CinemachineBasicMultiChannelPerlin cameraNoise;
    [SerializeField] private GameObject footCircleEffect;

    [Header("Movement Settings")]
    [Tooltip("Horizontal movement speed.")]
    [SerializeField] private float playerSpeed = 20;
    [Tooltip("Vertical force applied when jumping.")]
    [SerializeField] private float playerJumpForce = 1;
    [Tooltip("Raycast distance used to detect the ground.")]
    [SerializeField] private float playerHeight = 1.1f;
    [Tooltip("Speed used to rotate the player model.")]
    [SerializeField] private float rotationSpeed = 15f;
    [Tooltip("Additional gravity applied while falling.")]
    [SerializeField] private float fallMultiplier = 2.5F;

    [SerializeField] private float movingAmplitude = 0.45f;
    [SerializeField] private float shakeSmoothSpeed = 1f;

    [Header("Look Settings")]
    [Tooltip("Sensitivity applied to look input.")]
    [SerializeField] private float lookSensitivity = 10f;
    [Tooltip("Minimum vertical camera angle.")]
    [SerializeField] private float minPitch = -30f;
    [Tooltip("Maximum vertical camera angle.")]
    [SerializeField] private float maxPitch = 10f;

    private bool jumpRequested;

    private float yaw;
    private float pitch;

    private Vector2 previousPlayerInput;

    /// <summary>Subscribes the local player to movement and jump input.</summary>
    public void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = player.eulerAngles.y;
        inputReader.PlayerMovementEvent += HandleMovementInput;
        inputReader.PlayerJumpEvent += HandleJumpInput;

        playerEffectsManager.SetTrailEffectsState(IsGrounded());
    }

    /// <summary>Unsubscribes the local player from movement and jump input.</summary>
    public void OnDisable()
    {
        inputReader.PlayerMovementEvent -= HandleMovementInput;
        inputReader.PlayerJumpEvent -= HandleJumpInput;
    }

    //---- New Movement Code----//
    /// <summary>Applies jumping, movement, rotation, animation, and gravity.</summary>
    private void FixedUpdate()
    {
        // -------------------------
        // JUMP
        // -------------------------

        if(!playerManager.IsAlive)
        {
            return;
        }

        if (jumpRequested && IsGrounded())
        {
            playerRigidbody.linearVelocity = new Vector3(
                playerRigidbody.linearVelocity.x,
                playerJumpForce,
                playerRigidbody.linearVelocity.z
            );

            jumpRequested = false;

            Debug.Log("Player Jumped");
        }

        // -------------------------
        // MOVEMENT
        // -------------------------

        if (IsGrounded())
        {
            Vector3 forward = cameraPivot.forward;
            Vector3 right = cameraPivot.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection =
                forward * previousPlayerInput.y +
                right * previousPlayerInput.x;

            if (moveDirection.sqrMagnitude > 1f)
                moveDirection.Normalize();

            playerRigidbody.linearVelocity = new Vector3(
                moveDirection.x * playerSpeed,
                playerRigidbody.linearVelocity.y,
                moveDirection.z * playerSpeed
            );

            // Rotation
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(moveDirection);

                playerModel.transform.rotation =
                    Quaternion.Slerp(
                        playerModel.transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.fixedDeltaTime
                    );
            }

            // Animation
            float currentSpeed =
                new Vector3(
                    playerRigidbody.linearVelocity.x,
                    0,
                    playerRigidbody.linearVelocity.z
                ).magnitude;

            if (currentSpeed < 0.1f)
                currentSpeed = 0;

            playerAnimationManager.PlayerStateChange(
                PlayerState.Moving,
                currentSpeed
            );

            Vector3 velocity = playerRigidbody.linearVelocity;
            velocity.y = 0f;

            bool isMoving = velocity.magnitude > 0.1f;

            float targetAmplitude = isMoving ? movingAmplitude : 0f;

            cameraNoise.AmplitudeGain = Mathf.MoveTowards(
                cameraNoise.AmplitudeGain,
                targetAmplitude,
                shakeSmoothSpeed * Time.deltaTime
            );
        }

        // -------------------------
        // FALL GRAVITY
        // -------------------------

        if (playerRigidbody.linearVelocity.y < 0)
        {
            playerRigidbody.linearVelocity +=
                Vector3.up *
                Physics.gravity.y *
                (fallMultiplier - 1f) *
                Time.fixedDeltaTime;
        }
    }

    /// <summary>Updates the local camera pivot from look input.</summary>
    private void LateUpdate()
    {
        yaw += inputReader.MouseInput.x * lookSensitivity * Time.deltaTime;
        pitch -= inputReader.MouseInput.y * lookSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraPivot.rotation = Quaternion.Euler(pitch, yaw, 0f);

        cameraPivot.position = player.position;

        playerEffectsManager.SetTrailEffectsState(IsGrounded());
    }

    /// <summary>Requests and applies a jump for the local player.</summary>
    private void PlayerJump()
    {
        if(IsGrounded())
        {
            jumpRequested = true;
            playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, playerJumpForce, playerRigidbody.linearVelocity.z);
            Debug.Log("Player Jumped");

            playerAnimationManager.PlayerStateChange(PlayerState.Jumping);
            footCircleEffect.SetActive(false);
            StartCoroutine(PlayerInJumpState());
            playerAnimationManager.SetGroundedState(false);
        }
    }

    /// <summary>Stores the latest movement input.</summary>
    /// <param name="movement">Movement vector from the input reader.</param>
    private void HandleMovementInput(Vector2 movement)
    {
        previousPlayerInput = movement;
    }

    /// <summary>Handles a jump event from the input reader.</summary>
    private void HandleJumpInput()
    {
        PlayerJump();
    }

    /// <summary>Checks whether the player is touching the ground.</summary>
    /// <returns>True when the ground raycast hits a collider.</returns>
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight);
    }

    /// <summary>Waits for landing before restoring the grounded animation state.</summary>
    private IEnumerator PlayerInJumpState()
    {
        yield return new WaitForSeconds(0.2f);

        while(!IsGrounded())
        {
            yield return null;
        }

        footCircleEffect.SetActive(true);
        playerAnimationManager.SetGroundedState(true);
    }

    /// <summary>Stops the player's current rigidbody movement.</summary>
    public void ResetVelocity()
    {
        playerRigidbody.linearVelocity = new Vector3(0, 0, 0);
    }
}
