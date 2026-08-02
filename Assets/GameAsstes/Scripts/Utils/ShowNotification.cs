using System.Collections;
using TMPro;
using UnityEngine;

public class ShowNotification : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1;

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
