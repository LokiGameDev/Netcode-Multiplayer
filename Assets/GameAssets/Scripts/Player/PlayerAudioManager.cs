using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSoruce;

    private bool currentMovementState = false;

    public void PlayerMovementState(bool state)
    {
        if(currentMovementState == state) return;

        if(state) audioSoruce.Play();
        else audioSoruce.Stop();

        currentMovementState = state;
    }
}
