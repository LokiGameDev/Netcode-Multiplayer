using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Advances a loading bar while a scene is being prepared.</summary>
public class LoadingBarAutoLoad : MonoBehaviour
{
    [Header("Loading")]
    [Tooltip("Slider displaying loading progress.")]
    [SerializeField] private Slider loadingBar;
    [Tooltip("Loading progress change per second.")]
    [SerializeField] private float loadingSpeed = 0.2f;

    /// <summary>Resets and starts the loading animation.</summary>
    private void OnEnable()
    {
        loadingBar.value = 0;
        AudioManager.Instance.Play(AudioID.Loading);
        StartCoroutine(LoadingBarLoad());
    }

    /// <summary>Stops the loading animation and resets the slider.</summary>
    private void OnDisable()
    {
        StopAllCoroutines();
        loadingBar.value = 0;
    }

    /// <summary>Advances the loading bar toward its target value.</summary>
    private IEnumerator LoadingBarLoad()
    {
        while(loadingBar.value < 0.9)
        {
            loadingBar.value = Mathf.MoveTowards(loadingBar.value, 1, loadingSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
