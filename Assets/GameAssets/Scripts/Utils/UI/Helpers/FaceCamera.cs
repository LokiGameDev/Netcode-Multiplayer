using UnityEngine;

/// <summary>Rotates a UI or world object to face the main camera.</summary>
public class FaceCamera : MonoBehaviour
{
    [SerializeField] private bool is3D = false;
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

        if(is3D) transform.forward = new Vector3(mainCamera.transform.forward.x, transform.forward.y, transform.forward.z);
        else transform.forward = mainCamera.transform.forward;
    }
}