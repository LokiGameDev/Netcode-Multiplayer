using UnityEngine;

/// <summary>Rotates a UI or world object to face the main camera.</summary>
public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    /// <summary>Caches the current main camera.</summary>
    private void Start()
    {
        mainCamera = Camera.main;
    }

    /// <summary>Updates the object's forward direction to match the camera.</summary>
    private void LateUpdate()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            return;
        }

        transform.forward = mainCamera.transform.forward;
    }
}