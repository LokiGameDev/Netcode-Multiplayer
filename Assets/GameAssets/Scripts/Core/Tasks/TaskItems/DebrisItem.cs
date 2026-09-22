using UnityEngine;

public class DebrisItem : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private RectTransform TaskFinishItem;

    public void Update()
    {
        if (IsInside(TaskFinishItem, rectTransform))
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
