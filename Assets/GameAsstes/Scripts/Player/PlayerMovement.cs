using System.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private PlayerAnimationManager playerAnimationManager;

    [SerializeField] private float playerSpeed = 20;
    [SerializeField] private float playerJumpForce = 1;
    [SerializeField] private float playerHeight = 1.1f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float fallMultiplier = 2.5F;

    private Vector2 previousPlayerInput;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;

        inputReader.PlayerMovementEvent += HandleMovementInput;
        inputReader.PlayerJumpEvent += HandleJumpInput;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) return;

        inputReader.PlayerMovementEvent -= HandleMovementInput;
        inputReader.PlayerJumpEvent -= HandleJumpInput;
    }

    private void FixedUpdate() 
    {
        if(!IsOwner) return; 

        Vector3 playerInput = new Vector3(previousPlayerInput.x, 0, previousPlayerInput.y).normalized;

        playerRigidbody.linearVelocity = new Vector3(playerInput.x * playerSpeed, playerRigidbody.linearVelocity.y, playerInput.z * playerSpeed);

        if (playerRigidbody.linearVelocity.y < 0)
        {
            playerRigidbody.linearVelocity +=
                Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        Vector3 moveDirection = new Vector3(playerInput.x, 0f, playerInput.z);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            playerModel.transform.rotation = Quaternion.Slerp(
                playerModel.transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }

        float currentSpeed = playerRigidbody.linearVelocity.magnitude;
        if(currentSpeed < 0.1f) currentSpeed = 0;
        
        playerAnimationManager.PlayerStateChange(PlayerState.Moving, currentSpeed);
    }

    private void PlayerJump()
    {
        if(!IsOwner) return;

        if(IsGrounded())
        {
            playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, playerJumpForce, playerRigidbody.linearVelocity.z);
            Debug.Log("Player Jumped");

            playerAnimationManager.PlayerStateChange(PlayerState.Jumping);
            StartCoroutine(PlayerInJumpState());
            playerAnimationManager.SetGroundedState(false);
        }
    }

    private void HandleMovementInput(Vector2 movement)
    {
        previousPlayerInput = movement;
    }

    private void HandleJumpInput()
    {
        PlayerJump();
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight);
    }

    private IEnumerator PlayerInJumpState()
    {
        yield return new WaitForSeconds(0.2f);
        while(!IsGrounded())
        {
            yield return true;
        }
        playerAnimationManager.SetGroundedState(true);
    }
}
