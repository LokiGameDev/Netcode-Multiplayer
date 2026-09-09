using UnityEngine;
using System.Collections;

/// <summary>Animates a menu button group between open and closed states.</summary>
public class MenuButtonAnimation : MonoBehaviour
{
    [Header("Menu Buttons")]
    [Tooltip("Main button used as the closed position.")]
    [SerializeField] private RectTransform menuButton;
    [Tooltip("Buttons animated from the main button position.")]
    [SerializeField] private RectTransform[] buttons;

    private Vector2[] targetPositions;

    private float duration = 0.25f;

    private bool isOpen = false;

    /// <summary>Stores target positions and initializes the closed state.</summary>
    private void Awake()
    {
        targetPositions = new Vector2[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            targetPositions[i] = buttons[i].anchoredPosition;

            buttons[i].anchoredPosition = menuButton.anchoredPosition;
            buttons[i].localScale = Vector3.zero;
        }

        if(isOpen) OpenMenu();
        else CloseMenu();
    }

    /// <summary>Toggles the menu between open and closed states.</summary>
    public void ChangeTheMenuPanelState()
    {
        isOpen = !isOpen;
        if(isOpen) OpenMenu();
        else CloseMenu();
    }

    /// <summary>Animates all menu buttons into their target positions.</summary>
    public void OpenMenu()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            StartCoroutine(AnimateButton(buttons[i], targetPositions[i], 1));
        }
        isOpen = true;
    }

    /// <summary>Animates all menu buttons back to the main button.</summary>
    public void CloseMenu()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            StartCoroutine(AnimateButton(buttons[i], menuButton.anchoredPosition, 0));
        }
        isOpen = false;
    }

    /// <summary>Animates one menu button's position, scale, and opacity.</summary>
    private IEnumerator AnimateButton(RectTransform button, Vector2 targetPos, int finalScale)
    {
        Vector2 startPos = button.anchoredPosition;
        CanvasGroup canvasGroup = button.gameObject.GetComponent<CanvasGroup>();

        float time = 0f;
        
        button.localScale = finalScale == 1 ? Vector2.zero : Vector2.one;
        button.gameObject.SetActive(true);
        canvasGroup.alpha = finalScale == 1 ? 0 : 1;

        Vector3 startScale = finalScale == 1 ? Vector2.zero : Vector2.one;
        Vector3 endScale = finalScale == 1 ? Vector2.one : Vector2.zero;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            button.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            button.localScale = Vector3.Lerp(startScale, endScale, t);
            canvasGroup.alpha = Mathf.Lerp(finalScale == 1 ? 0 : 1, finalScale, t);

            yield return null;
        }

        canvasGroup.alpha = finalScale;
        button.anchoredPosition = targetPos;
        button.localScale = finalScale == 1 ? Vector2.one : Vector2.zero;
    }
}
