using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBarAutoLoad : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;
    [SerializeField] private float loadingSpeed = 0.2f;

    private void OnEnable()
    {
        loadingBar.value = 0;
        StartCoroutine(LoadingBarLoad());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        loadingBar.value = 0;
    }

    private IEnumerator LoadingBarLoad()
    {
        while(loadingBar.value < 0.9)
        {
            loadingBar.value = Mathf.MoveTowards(loadingBar.value, 1, loadingSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
