using UnityEngine;

public class TaskMarker : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("UI")]
    [SerializeField] private RectTransform indicatorArea;
    [SerializeField] private RectTransform targetIcon;
    [SerializeField] private RectTransform directionArrow;

    [Header("Settings")]
    [SerializeField] private float radius = 350;
    [SerializeField] private float minDistanceBetweenTask = 5;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        targetIcon.gameObject.SetActive(false);
        directionArrow.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if(target==null)
        {
            targetIcon.gameObject.SetActive(false);
            directionArrow.gameObject.SetActive(false);
            return;
        }

        targetIcon.gameObject.SetActive(true);

        UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        Vector3 screenPosition =
            cam.WorldToScreenPoint(target.position + new Vector3(0,0.5f,0));

        Transform player = UIManager.Instance.GetPlayerPosition();

        Vector2 center = indicatorArea.position;

        if(player!=null) center = cam.WorldToScreenPoint(player.position);

        Vector2 direction = (Vector2)screenPosition - center;

        if (screenPosition.z < 0) direction = -direction;

        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(indicatorArea, screenPosition, null);

        if (screenPosition.z > 0 && isInside)
        {
            // Target is inside circle
            targetIcon.gameObject.SetActive(true);
            directionArrow.gameObject.SetActive(false);

            targetIcon.position = cam.WorldToScreenPoint(target.position + new Vector3(0,2.5f,0));

            float distance = Vector3.Distance(UIManager.Instance.GetPlayerPosition().position, target.position);

            if(distance < minDistanceBetweenTask) targetIcon.gameObject.SetActive(false);
            else targetIcon.gameObject.SetActive(true);
        }
        else
        {
            // Target is outside circle
            targetIcon.gameObject.SetActive(false);
            directionArrow.gameObject.SetActive(true);

            direction.Normalize();

            Vector2 arrowPosition = center + (direction * radius);

            directionArrow.position = arrowPosition;

            // Rotate arrow toward target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            directionArrow.rotation = Quaternion.Euler(0, 0, angle - 90);
            
            //UIManager.Instance.DisplayDebugValues(screenPosition, center, arrowPosition, angle);
        }
    }

    public void SetCurrentTarget(Transform currentTarget)
    {
        if(currentTarget==null)
        {
            target = null;
            targetIcon.gameObject.SetActive(false);
            directionArrow.gameObject.SetActive(false);
            return;
        }
        Debug.Log($"Setting current target: {currentTarget.name}");
        target = currentTarget;
    }
}