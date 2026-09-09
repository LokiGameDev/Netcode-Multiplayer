using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>Shows a temporary message and hides it after a delay.</summary>
public class DisplayMessage : MonoBehaviour
{
    [Tooltip("Text used to display the message.")]
    [SerializeField] private TMP_Text text;
    [Tooltip("Seconds before the message is hidden.")]
    [SerializeField] private float fadeDuration = 1;

    /// <summary>Hides the message until it is requested.</summary>
    private void Start()
    {
        text.gameObject.SetActive(false);
    }

    /// <summary>Displays a message and restarts its hide timer.</summary>
    /// <param name="message">Text to display.</param>
    public void ShowText(string message)
    {
        if(text.gameObject.activeSelf)
        {
            StopAllCoroutines();
        }
        text.text = message;
        text.gameObject.SetActive(true);
        StartCoroutine(DisplayMessageFade());
    }

    /// <summary>Waits before hiding the displayed message.</summary>
    private IEnumerator DisplayMessageFade()
    {
        yield return new WaitForSeconds(fadeDuration);
        text.gameObject.SetActive(false);
    }
}
