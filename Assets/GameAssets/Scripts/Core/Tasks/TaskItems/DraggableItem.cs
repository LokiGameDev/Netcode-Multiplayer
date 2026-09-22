using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Canvas Canvas;
    [SerializeField] private RectTransform dragArea;

    [SerializeField] private RectTransform TaskFinishItem;

    public void OnDrag(PointerEventData eventData)
    {
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

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsInside(rectTransform, TaskFinishItem))
        {
            Debug.Log("I am inside: " + GetComponent<ITaskFinishItem>().itemName);
            TaskFinishItem.GetComponent<ITaskFinisher>().ItemIsInside(GetComponent<ITaskFinishItem>());
        }
    }


    private bool IsInside(RectTransform movingImage, RectTransform targetImage)
    {
        Vector3[] movingCorners = new Vector3[4];
        Vector3[] targetCorners = new Vector3[4];

        movingImage.GetWorldCorners(movingCorners);
        targetImage.GetWorldCorners(targetCorners);

        Bounds targetBounds = new Bounds(targetCorners[0], Vector3.zero);

        for (int i = 1; i < 4; i++)
        {
            targetBounds.Encapsulate(targetCorners[i]);
        }

        // Check all 4 corners of Image 1
        for (int i = 0; i < 4; i++)
        {
            if (!targetBounds.Contains(movingCorners[i]))
                return false;
        }

        return true;
    }
}
