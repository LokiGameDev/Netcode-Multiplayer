using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>Shows an animated, temporary notification message.</summary>
public class ShowNotification : MonoBehaviour
{
    [Header("Notification")]
    [Tooltip("Text used to display the notification.")]
    [SerializeField] private TMP_Text text;
    [Tooltip("Panel animated when the notification appears.")]
    [SerializeField] private RectTransform displayBox;
    [Tooltip("Canvas group faded when the notification closes.")]
    [SerializeField] private CanvasGroup canvasGroup;
    [Tooltip("Total notification display duration.")]
    [SerializeField] private float fadeDuration = 1;

    [SerializeField] private NotificationType notificationType = NotificationType.ScalePop;

    [Tooltip("Duration of the pop-in animation.")]
    [SerializeField] private float popDuration = 0.5f;

    private Coroutine coroutine;

    /// <summary>Initializes the notification as hidden.</summary>
    private void Start()
    {
        canvasGroup.alpha = 0;
    }

    /// <summary>Displays a message and starts its animation.</summary>
    /// <param name="message">Notification text.</param>
    public void ShowText(string message)
    {
        if(coroutine!=null)
        {
            StopCoroutine(coroutine);
        }
        text.text = message;
        canvasGroup.alpha = 1;
        coroutine = StartCoroutine(DisplayMessageFade());
    }

    /// <summary>Animates the notification in and fades it out.</summary>
    private IEnumerator DisplayMessageFade()
    {
        displayBox.localScale = Vector3.zero;

        AudioManager.Instance.Play(AudioID.Notification);

        switch(notificationType)
        {
            case NotificationType.ScalePop:
                yield return StartCoroutine(ScalePop());
                break;
            case NotificationType.PopFromBottom:
                yield return StartCoroutine(BottomPop());
                break;
        }

        displayBox.localScale = Vector3.one;

        yield return new WaitForSeconds(fadeDuration/2);

        float elapsed = 0;
        while(elapsed < fadeDuration/2)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1,0,elapsed/(fadeDuration/2));
            yield return null;
        }
        canvasGroup.alpha = 0;
        coroutine = null;
    }

    private IEnumerator ScalePop()
    {
        float time = 0f;

        while (time < popDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / popDuration);

            // Overshoot
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.15f;

            // Grow from 0 to 1 with overshoot
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            displayBox.localScale = Vector3.one * eased * scale;

            yield return null;
        }
    }

    private IEnumerator BottomPop()
    {
        float time = 0f;

        Vector2 startPos = displayBox.anchoredPosition + new Vector2(0, -50);
        Vector2 endPos = displayBox.anchoredPosition;

        while (time < popDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / popDuration);

            t = Mathf.SmoothStep(0f, 1f, t);

            displayBox.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        displayBox.anchoredPosition = endPos;
    }
}

public enum NotificationType
{
    ScalePop,
    PopFromBottom
}
