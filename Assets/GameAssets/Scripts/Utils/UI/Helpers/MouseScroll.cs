using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MouseScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private float scrollSpeed = 0.1f;

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