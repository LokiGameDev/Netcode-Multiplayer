using System.Collections;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private float rotationSpeed = 180;

    private bool isActivated;

    private Coroutine rotateRoutine;

    private void OnEnable()
    {
        if(rotateRoutine!=null) StopCoroutine(rotateRoutine);

        rotateRoutine = StartCoroutine(RotateRoutine(Quaternion.Euler(isActivated ? 0 : -90, 0, 0)));
    }

    public void OnStateChange(bool state)
    {
        isActivated = state;

        if(rotateRoutine!=null) StopCoroutine(rotateRoutine);

        rotateRoutine = StartCoroutine(RotateRoutine(Quaternion.Euler(isActivated ? 0 : -90, 0, 0)));
    }

    private IEnumerator RotateRoutine(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);

            yield return null;
        }

        transform.localRotation = targetRotation;
        rotateRoutine = null;
    }
}
