using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DraggableButton : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Canvas Canvas;
    [SerializeField] private RectTransform dragArea;

    private Vector2 boundariesMin = new Vector2(-820, -310);
    private Vector2 boundariesMax = new Vector2(820, 310);

    public void OnDrag(PointerEventData eventData)
    {
        // Vector2 newPosition = rectTransform.anchoredPosition + eventData.delta / Canvas.scaleFactor;
        // newPosition.x = Mathf.Clamp(newPosition.x, boundariesMin.x, boundariesMax.x);
        // newPosition.y = Mathf.Clamp(newPosition.y, boundariesMin.y, boundariesMax.y);
        // rectTransform.anchoredPosition = newPosition;

        Vector2 delta = eventData.delta / Canvas.scaleFactor;

        Vector2 newPosition = rectTransform.anchoredPosition + delta;

        Vector2 areaSize = dragArea.rect.size;
        Vector2 buttonSize = rectTransform.rect.size;

        float minX = -areaSize.x / 2f + buttonSize.x / 2f;
        float maxX =  areaSize.x / 2f - buttonSize.x / 2f;

        float minY = -areaSize.y / 2f + buttonSize.y / 2f;
        float maxY =  areaSize.y / 2f - buttonSize.y / 2f;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        rectTransform.anchoredPosition = newPosition;
    }
}
