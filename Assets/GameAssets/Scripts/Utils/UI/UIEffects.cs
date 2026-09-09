using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>Runs fade, slide, scale, and rotation effects on UI elements.</summary>
public class UIEffects : MonoBehaviour
{
    [Header("General Settings")]
    [Tooltip("Determines when the effect starts.")]
    [SerializeField] private EffectInitiateType effectInitiateType;
    [Tooltip("Effect type to run.")]
    [SerializeField] private UIEffectType uIEffectType;
    [Tooltip("UI panel affected by the effect.")]
    [SerializeField] private RectTransform panel;

    [Header("Fade Settings")]
    [Tooltip("Canvas group used for fading.")]
    [SerializeField] private CanvasGroup canvasGroup;
    [Tooltip("Default fade duration in seconds.")]
    [SerializeField] private float defaultFadeDuration = 0.5f;
    [Tooltip("Delay before a fade starts.")]
    [SerializeField] private float waitBeforeStartingDuration = 0;
    [Tooltip("Delay after a fade completes.")]
    [SerializeField] private float waitBeforeCompletionDuration = 0;

    [Header("Slide Settings")]
    [Tooltip("Duration of slide effects.")]
    [SerializeField] private float slideDuration = 1f;
    [Tooltip("Final anchored position of the panel.")]
    [SerializeField] private Vector2 panelFinalPosition;

    [Header("Scale Effect")]
    [Tooltip("Speed of the scale animation.")]
    [SerializeField] private float scalingSpeed = 5f;
    [Tooltip("Minimum scale used by the scale effect.")]
    [SerializeField] private float minIconSize = 1f;
    [Tooltip("Maximum scale used by the scale effect.")]
    [SerializeField] private float maxIconSize = 1.25f;

    [Header("Rotate Around")]
    [Tooltip("Rotation speed in degrees per second.")]
    [SerializeField] private float rotatingSpeed = 150;
    [Tooltip("Whether the rotation runs clockwise.")]
    [SerializeField] private bool clockwiseDirection = true;

    private Coroutine currentCoroutine;

    [Header("Fade Events")]
    [Tooltip("Invoked when a fade starts.")]
    public UnityEvent OnFadeStarted;
    [Tooltip("Invoked when a fade completes.")]
    public UnityEvent OnFadeCompleted;
    

    /// <summary>Finds missing UI references and stores the final panel position.</summary>
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

    /// <summary>Starts the configured effect when enabled.</summary>
    private void OnEnable()
    {
        if(effectInitiateType == EffectInitiateType.OnEnable) StartEffect();
    }

    /// <summary>Stops active effect coroutines when disabled.</summary>
    private void OnDisable()
    {
        if(currentCoroutine!=null) StopCoroutine(currentCoroutine);
        StopAllCoroutines();
    }

    #region Fade Effect

    /// <summary>Fades the canvas in using the default duration.</summary>
    public void FadeIn()
    {
        FadeIn(defaultFadeDuration);
    }

    /// <summary>Fades the canvas out using the default duration.</summary>
    public void FadeOut()
    {
        FadeOut(defaultFadeDuration);
    }

    /// <summary>Fades the canvas in over a specified duration.</summary>
    /// <param name="duration">Fade duration in seconds.</param>
    public void FadeIn(float duration)
    {
        StartFade(1f, duration);
    }

    /// <summary>Fades the canvas out over a specified duration.</summary>
    /// <param name="duration">Fade duration in seconds.</param>
    public void FadeOut(float duration)
    {
        StartFade(0f, duration);
    }

    /// <summary>Starts a fade coroutine toward the target alpha.</summary>
    private void StartFade(float targetAlpha, float duration)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(FadeRoutine(targetAlpha, duration));
    }

    /// <summary>Animates the canvas alpha and fade events.</summary>
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

    /// <summary>Starts the configured UI effect.</summary>
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
            case UIEffectType.RotateAround:
                StartRotateEffect();
                break;
        }
    }

    /// <summary>Starts the selected fade effect.</summary>
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

    /// <summary>Starts a slide from one position to another.</summary>
    private void StartSlideEffects(Vector2 startPos, Vector2 target)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(Slide(startPos, target));
    }

    /// <summary>Animates the panel position between two points.</summary>
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

    /// <summary>Starts the continuous scale effect.</summary>
    private void StartscalingEffect()
    {
        if(currentCoroutine!=null) StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ScaleEffectEnumerator());
    }

    /// <summary>Animates the panel scale between configured limits.</summary>
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

    #region Rotate Effect

    /// <summary>Starts the continuous rotation effect.</summary>
    private void StartRotateEffect()
    {
        if(currentCoroutine!=null) StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(RotatingEffect());
    }

    /// <summary>Rotates the panel until the effect is stopped.</summary>
    private IEnumerator RotatingEffect()
    {
        while(true)
        {
            float angle = rotatingSpeed * Time.deltaTime * (clockwiseDirection?-1:1);
            panel.Rotate(0,0,angle);
            yield return null;
        }
    }

    #endregion
}

/// <summary>Available UI effect animations.</summary>
public enum UIEffectType
{
    FadeIn,
    FadeOut,
    DropDown,
    PopUp,
    LeftSlide,
    RightSlide,
    ScaleEffect,
    RotateAround
}

/// <summary>Events that can start a UI effect.</summary>
public enum EffectInitiateType
{
    OnCall,
    OnEnable
}