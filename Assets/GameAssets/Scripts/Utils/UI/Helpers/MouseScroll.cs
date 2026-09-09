using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>Maps mouse wheel input to horizontal scroll position.</summary>
public class MouseScroll : MonoBehaviour
{
    [Tooltip("Scroll view controlled by the mouse wheel.")]
    [SerializeField] private ScrollRect scrollView;
    [Tooltip("Scroll distance applied per mouse wheel unit.")]
    [SerializeField] private float scrollSpeed = 0.1f;

    /// <summary>Reads the mouse wheel and updates the scroll position.</summary>
    private void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            scrollView.horizontalNormalizedPosition += scroll * scrollSpeed;
            scrollView.horizontalNormalizedPosition =
                Mathf.Clamp01(scrollView.horizontalNormalizedPosition);
        }
    }
}