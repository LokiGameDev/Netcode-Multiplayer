using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text objectNameText;
    [SerializeField] private TMP_Text actionText;

    private Transform target;

    public void Show(IInteractable interactable, Transform target)
    {
        this.target = target;

        //objectNameText.text = interactable.GetObjectName();
        actionText.text = interactable.GetActionName();

        gameObject.SetActive(true);
    }

    public void Show(ITask task, Transform target)
    {
        this.target = target;

        //objectNameText.text = interactable.GetObjectName();
        actionText.text = task.GetActionName();

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        target = null;
        gameObject.SetActive(false);
    }

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