using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

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