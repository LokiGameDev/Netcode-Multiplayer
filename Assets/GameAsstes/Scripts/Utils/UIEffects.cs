using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class UIEffects : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float defaultFadeDuration = 0.5f;
    [SerializeField] private float waitBeforeStartingDuration = 0;
    [SerializeField] private float waitBeforeCompletionDuration = 0;

    private Coroutine fadeCoroutine;

    public UnityEvent OnFadeStarted;
    public UnityEvent OnFadeCompleted;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeIn()
    {
        FadeIn(defaultFadeDuration);
    }

    public void FadeOut()
    {
        FadeOut(defaultFadeDuration);
    }

    public void FadeIn(float duration)
    {
        StartFade(1f, duration);
    }

    public void FadeOut(float duration)
    {
        StartFade(0f, duration);
    }

    private void StartFade(float targetAlpha, float duration)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha, duration));
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        yield return new WaitForSeconds(waitBeforeStartingDuration);

        OnFadeStarted?.Invoke();
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        if (targetAlpha > 0f)
        {
            gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                elapsed / duration);

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (targetAlpha <= 0f)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        yield return new WaitForSeconds(waitBeforeCompletionDuration);

        fadeCoroutine = null;
        OnFadeCompleted?.Invoke();
    }
}
