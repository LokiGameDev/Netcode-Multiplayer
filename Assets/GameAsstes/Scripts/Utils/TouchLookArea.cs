using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        previousPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        LookDelta = eventData.position - previousPosition;
        previousPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        LookDelta = Vector2.zero;
    }

    private void LateUpdate()
    {
        // Consume the delta once per frame.
        if (!dragging)
            LookDelta = Vector2.zero;

        OnLook?.Invoke(LookDelta);
        if(Application.isMobilePlatform) inputReader?.SetLook(LookDelta);
    }
}