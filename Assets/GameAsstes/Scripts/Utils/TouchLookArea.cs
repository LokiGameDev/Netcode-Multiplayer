using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class TouchLookArea : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public Action<Vector2> OnLook;

    private Vector2 previousPosition;

    public void OnPointerDown(PointerEventData eventData)
    {
        previousPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - previousPosition;
        previousPosition = eventData.position;

        OnLook?.Invoke(delta);
    }
}