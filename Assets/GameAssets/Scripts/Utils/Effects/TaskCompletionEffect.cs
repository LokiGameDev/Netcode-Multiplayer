using UnityEngine;

/// <summary>Plays the visual effect shown when a task is completed.</summary>
public class TaskCompletionEffect : MonoBehaviour
{
    [Tooltip("Animator controlling the completion effect.")]
    [SerializeField] private Animator effectAnimator;

    /// <summary>Plays the completion animation and sound at a position.</summary>
    /// <param name="position">World position for the completion sound.</param>
    public void PlayEffect(Vector3 position)
    {
        effectAnimator.SetTrigger("Play");
        AudioManager.Instance.Play(AudioID.TaskCompletion, position);
    }
}
