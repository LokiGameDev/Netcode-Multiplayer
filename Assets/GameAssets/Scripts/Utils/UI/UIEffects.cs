using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class UIEffects : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private EffectInitiateType effectInitiateType;
    [SerializeField] private UIEffectType uIEffectType;
    [SerializeField] private RectTransform panel;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float defaultFadeDuration = 0.5f;
    [SerializeField] private float waitBeforeStartingDuration = 0;
    [SerializeField] private float waitBeforeCompletionDuration = 0;

    [Header("Slide Settings")]
    [SerializeField] private float slideDuration = 1f;
    [SerializeField] private Vector2 panelFinalPosition;

    [Header("Scale Effect")]
    [SerializeField] private float scalingSpeed = 5f;
    [SerializeField] private float minIconSize = 1f;
    [SerializeField] private float maxIconSize = 1.25f;

    private Coroutine currentCoroutine;

    [Header("Fade Events")]
    public UnityEvent OnFadeStarted;
    public UnityEvent OnFadeCompleted;
    

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        if(panel!=null)
        {
            panel = GetComponent<RectTransform>();
            if(panel!=null) panelFinalPosition = panel.anchoredPosition;
        }
    }

    private void OnEnable()
    {
        if(effectInitiateType == EffectInitiateType.OnEnable) StartEffect();
    }

    private void OnDisable()
    {
        if(currentCoroutine!=null) StopCoroutine(currentCoroutine);
        StopAllCoroutines();
    }

    #region Fade Effect

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
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(FadeRoutine(targetAlpha, duration));
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

        currentCoroutine = null;
        OnFadeCompleted?.Invoke();
    }

    #endregion

    #region Slide Effects

    public void StartEffect()
    {
        if(panel==null) return;

        float screenWidth = ((RectTransform)panel.parent).rect.width;
        float panelWidth = panel.rect.width;

        float screenheight = ((RectTransform)panel.parent).rect.height;
        float panelheigth = panel.rect.height;
        
        Vector2 startPos;

        switch(uIEffectType)
        {
            case UIEffectType.DropDown:
                startPos = new Vector2(panelFinalPosition.x,screenheight / 2f + panelheigth / 2f);
                StartSlideEffects(startPos, panelFinalPosition);
                break;
            case UIEffectType.PopUp:
                startPos = new Vector2(panelFinalPosition.x,-(screenheight / 2f + panelheigth / 2f));
                StartSlideEffects(startPos, panelFinalPosition);
                break;
            case UIEffectType.LeftSlide:
                startPos = new Vector2(-(screenWidth / 2f + panelWidth / 2f),panelFinalPosition.y);
                StartSlideEffects(startPos, panelFinalPosition);
                break;
            case UIEffectType.RightSlide:
                startPos = new Vector2(screenWidth / 2f + panelWidth / 2f,panelFinalPosition.y);
                StartSlideEffects(startPos, panelFinalPosition);
                break;
            case UIEffectType.FadeIn:
                StartFadeEffect(UIEffectType.FadeIn);
                break;
            case UIEffectType.FadeOut:
                StartFadeEffect(UIEffectType.FadeOut);
                break;
            case UIEffectType.ScaleEffect:
                StartscalingEffect();
                break;
        }
    }

    private void StartFadeEffect(UIEffectType uIEffectType)
    {
        if(canvasGroup==null) return;

        switch(uIEffectType)
        {
            case UIEffectType.FadeIn:
                FadeIn();
                break;
            case UIEffectType.FadeOut:
            FadeOut();
                break;
        }
    }

    private void StartSlideEffects(Vector2 startPos, Vector2 target)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(Slide(startPos, target));
    }

    private IEnumerator Slide(Vector2 start, Vector2 target)
    {
        float time = 0f;

        while (time < slideDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / slideDuration);

            // Smooth movement
            t = Mathf.SmoothStep(0f, 1f, t);

            panel.anchoredPosition = Vector2.Lerp(start, target, t);

            yield return null;
        }

        panel.anchoredPosition = target;
    }

    #endregion

    #region Scale Effect

    private void StartscalingEffect()
    {
        if(currentCoroutine!=null) StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ScaleEffectEnumerator());
    }

    private IEnumerator ScaleEffectEnumerator()
    {
        while(true)
        {
            float scale = minIconSize + (1+Mathf.Sin(Time.time * scalingSpeed))/2 * (maxIconSize-minIconSize);
            panel.localScale = Vector3.one * scale;
            yield return null;
        }
    }

    #endregion
}

public enum UIEffectType
{
    FadeIn,
    FadeOut,
    DropDown,
    PopUp,
    LeftSlide,
    RightSlide,
    ScaleEffect
}

public enum EffectInitiateType
{
    OnCall,
    OnEnable
}