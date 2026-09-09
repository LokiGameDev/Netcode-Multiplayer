using UnityEngine;
using TMPro;

/// <summary>Displays the action prompt for a nearby world target.</summary>
public class InteractionUI : MonoBehaviour
{
    [Header("Interaction Text")]
    [Tooltip("Text displaying the target name when configured.")]
    [SerializeField] private TMP_Text objectNameText;
    [Tooltip("Text displaying the available action.")]
    [SerializeField] private TMP_Text actionText;

    private Transform target;

    /// <summary>Shows the prompt for an interactable object.</summary>
    /// <param name="interactable">Interactable represented by the prompt.</param>
    /// <param name="target">World transform followed by the prompt.</param>
    public void Show(IInteractable interactable, Transform target)
    {
        this.target = target;

        //objectNameText.text = interactable.GetObjectName();
        actionText.text = interactable.GetActionName();

        gameObject.SetActive(true);
    }

    /// <summary>Shows the prompt for an assigned task.</summary>
    /// <param name="task">Task represented by the prompt.</param>
    /// <param name="target">World transform followed by the prompt.</param>
    public void Show(ITask task, Transform target)
    {
        this.target = target;

        //objectNameText.text = interactable.GetObjectName();
        actionText.text = task.GetActionName();

        gameObject.SetActive(true);
    }

    /// <summary>Hides the interaction prompt.</summary>
    public void Hide()
    {
        target = null;
        gameObject.SetActive(false);
    }

    /// <summary>Positions and rotates the prompt above its target.</summary>
    private void LateUpdate()
    {
        if (target == null)
            return;

        transform.position = target.position + Vector3.up * 2f;

        Camera cam = Camera.main;

        if (cam == null)
            return;

        transform.rotation = cam.transform.rotation;
    }
}