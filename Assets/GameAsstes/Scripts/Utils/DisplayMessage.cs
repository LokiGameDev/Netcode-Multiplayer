using System.Collections;
using TMPro;
using UnityEngine;

public class DisplayMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float fadeDuration = 1;

    private void Start()
    {
        text.gameObject.SetActive(false);
    }

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

    private IEnumerator DisplayMessageFade()
    {
        yield return new WaitForSeconds(fadeDuration);
        text.gameObject.SetActive(false);
    }
}
