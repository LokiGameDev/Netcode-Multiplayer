using System.Collections;
using TMPro;
using UnityEngine;

public class GemsBoughtShower : MonoBehaviour
{
    [SerializeField] private TMP_Text amountOfGemsText;
    [SerializeField] private CanvasGroup showerCanvas;
    [SerializeField] private GameObject showerObject;

    private Coroutine coroutine;

    private void Start()
    {
        showerCanvas.alpha = 0;
        showerObject.SetActive(false);
    }

    public void PlayerBoughtGems(int amount)
    {
        amountOfGemsText.text = amount.ToString();
        
        if(coroutine!=null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(GemsBoughtPop());
    }

    private IEnumerator GemsBoughtPop()
    {
        showerObject.SetActive(true);
        showerCanvas.alpha = 1;

        yield return new WaitForSeconds(3);

        showerCanvas.alpha = 0;
        showerObject.SetActive(false);
    }
}
