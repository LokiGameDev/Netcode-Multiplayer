using System.Collections;
using TMPro;
using UnityEngine;

public class ShowNotification : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private RectTransform displayBox;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1;
    [SerializeField] private float popDuration = 0.5f;

    private Coroutine coroutine;

    private void Start()
    {
        canvasGroup.alpha = 0;
    }

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

    private IEnumerator DisplayMessageFade()
    {
        float time = 0f;

        displayBox.localScale = Vector3.zero;

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
}
