using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>Converts touch dragging into camera look input.</summary>
public class TouchLookArea : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IDragHandler
{
    public Vector2 LookDelta { get; private set; }

    private Vector2 previousPosition;
    private bool dragging;

    public Action<Vector2> OnLook;

    public InputReader inputReader;

    /// <summary>Starts tracking a touch look gesture.</summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        previousPosition = eventData.position;
    }

    /// <summary>Updates the look delta while the pointer is dragged.</summary>
    public void OnDrag(PointerEventData eventData)
    {
        LookDelta = eventData.position - previousPosition;
        previousPosition = eventData.position;
    }

    /// <summary>Stops tracking the touch look gesture.</summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        LookDelta = Vector2.zero;
    }

    /// <summary>Publishes the current look delta and updates mobile input.</summary>
    private void LateUpdate()
    {
        // Consume the delta once per frame.
        if (!dragging)
            LookDelta = Vector2.zero;

        OnLook?.Invoke(LookDelta);
        if(Application.isMobilePlatform) inputReader?.SetLook(LookDelta);
    }
}