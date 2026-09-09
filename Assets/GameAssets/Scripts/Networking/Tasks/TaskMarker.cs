using UnityEngine;

/// <summary>Points the HUD toward the currently selected task.</summary>
public class TaskMarker : MonoBehaviour
{
    [Tooltip("World transform of the current task target.")]
    [SerializeField] private Transform target;

    [Header("UI")]
    [Tooltip("Screen area used to position the task indicator.")]
    [SerializeField] private RectTransform indicatorArea;
    [Tooltip("Icon shown when the task is on screen.")]
    [SerializeField] private RectTransform targetIcon;
    [Tooltip("Arrow shown when the task is off screen.")]
    [SerializeField] private RectTransform directionArrow;

    [Header("Settings")]
    [Tooltip("Distance from the indicator center used for the direction arrow.")]
    [SerializeField] private float radius = 350;
    [Tooltip("Minimum distance before the on-screen task icon is hidden.")]
    [SerializeField] private float minDistanceBetweenTask = 5;

    private Camera cam;

    /// <summary>Initializes the camera and hides task indicators.</summary>
    private void Start()
    {
        cam = Camera.main;
        targetIcon.gameObject.SetActive(false);
        directionArrow.gameObject.SetActive(false);
    }

    /// <summary>Updates the task indicator during the frame.</summary>
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

    /// <summary>Positions and rotates the indicator for the target.</summary>
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

    /// <summary>Sets the world target represented by this marker.</summary>
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