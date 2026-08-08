using UnityEngine;
using System.Collections;

public class MenuButtonAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform menuButton;
    [SerializeField] private RectTransform[] buttons;

    private Vector2[] targetPositions;

    private float duration = 0.25f;

    private bool isOpen = false;

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

    public void ChangeTheMenuPanelState()
    {
        isOpen = !isOpen;
        if(isOpen) OpenMenu();
        else CloseMenu();
    }

    public void OpenMenu()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            StartCoroutine(AnimateButton(buttons[i], targetPositions[i], 1));
        }
    }

    public void CloseMenu()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            StartCoroutine(AnimateButton(buttons[i], menuButton.anchoredPosition, 0));
        }
    }

    private IEnumerator AnimateButton(RectTransform button, Vector2 targetPos, int finalScale)
    {
        Vector2 startPos = button.anchoredPosition;

        float time = 0f;
        
        button.localScale = finalScale == 1 ? Vector2.zero : Vector2.one;
        button.gameObject.SetActive(true);

        Vector3 startScale = finalScale == 1 ? Vector2.zero : Vector2.one;
        Vector3 endScale = finalScale == 1 ? Vector2.one : Vector2.zero;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            button.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            button.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        button.anchoredPosition = targetPos;
        button.localScale = finalScale == 1 ? Vector2.one : Vector2.zero;
    }
}
