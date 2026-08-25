using UnityEngine;

public class TaskCompletionEffect : MonoBehaviour
{
    [SerializeField] private Animator effectAnimator;

    public void PlayEffect(Vector3 position)
    {
        effectAnimator.SetTrigger("Play");
        AudioManager.Instance.Play(AudioID.TaskCompletion, position);
    }
}
