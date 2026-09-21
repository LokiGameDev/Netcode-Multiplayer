using UnityEngine;

public class PlayerOfflineAnimationManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Tooltip("Animator used for player movement and action animations.")]
    [SerializeField] private Animator playerAnimator;

    /// <summary>Finds a child animator when one is not assigned.</summary>
    public void Start()
    {
        if(playerAnimator==null) playerAnimator = GetComponentInChildren<Animator>();

        playerAnimator.SetBool("IsAlive", true);
    }

    /// <summary>Applies a player state and optional movement value to the animator.</summary>
    public void PlayerStateChange(PlayerState playerState, float value = 0)
    {
        switch(playerState)
        {
            case PlayerState.Idle:
                playerAnimator.SetBool("IsAlive", true);
                break;
            case PlayerState.Moving:
                playerAnimator.SetFloat("Speed", value);
                break;
            case PlayerState.Jumping:
                playerAnimator.SetTrigger("Jump");
                break;
            case PlayerState.Crouching:
                break;
            case PlayerState.Dead:
                playerAnimator.SetTrigger("IsDead");
                playerAnimator.SetBool("IsAlive", false);
                break;
        }
    }

    /// <summary>Updates the animator's grounded parameter.</summary>
    public void SetGroundedState(bool state)
    {
        playerAnimator.SetBool("IsGrounded", state);
    }

    /// <summary>Assigns the animator used for player animations.</summary>
    public void SetAnimator(Animator animator)
    {
        playerAnimator = animator;
    }

}