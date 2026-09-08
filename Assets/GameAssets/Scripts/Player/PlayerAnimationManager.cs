using Unity.Netcode;
using UnityEngine;

public class PlayerAnimationManager : NetworkBehaviour
{
    [SerializeField] private Animator playerAnimator;

    public void Start()
    {
        if(playerAnimator==null) playerAnimator = GetComponentInChildren<Animator>();
    }

    public void PlayerStateChange(PlayerState playerState, float value = 0)
    {
        switch(playerState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Moving:
                playerAnimator.SetFloat("Speed", value);
                break;
            case PlayerState.Jumping:
                playerAnimator.SetTrigger("Jump");
                break;
            case PlayerState.Crouching:
                break;
        }
    }

    public void SetGroundedState(bool state)
    {
        playerAnimator.SetBool("IsGrounded", state);
    }

    public void SetAnimator(Animator animator)
    {
        playerAnimator = animator;
    }

}

public enum PlayerState
{
    Idle,
    Moving,
    Jumping,
    Crouching
}