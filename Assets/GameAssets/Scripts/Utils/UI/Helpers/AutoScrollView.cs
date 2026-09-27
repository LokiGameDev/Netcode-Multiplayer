using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoScrollView : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float autoScrollSpeed = 0.05f;

    private bool isInteracting;

    private void Start()
    {
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void Update()
    {
        if (isInteracting)
            return;

        if (scrollRect.verticalNormalizedPosition > 0f)
        {
            scrollRect.verticalNormalizedPosition -=
                autoScrollSpeed * Time.deltaTime;
        }

        // Reached bottom
        if (scrollRect.verticalNormalizedPosition <= 0f)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isInteracting = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isInteracting = false;
    }
}