using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>Displays the number of gems received from a purchase.</summary>
public class GemsBoughtShower : MonoBehaviour
{
    [Header("Purchase Feedback")]
    [Tooltip("Text displaying the purchased gem amount.")]
    [SerializeField] private TMP_Text amountOfGemsText;
    [Tooltip("Canvas group faded for the purchase message.")]
    [SerializeField] private CanvasGroup showerCanvas;
    [Tooltip("Object containing the purchase message.")]
    [SerializeField] private GameObject showerObject;

    private Coroutine coroutine;

    /// <summary>Initializes the purchase message as hidden.</summary>
    private void Start()
    {
        showerCanvas.alpha = 0;
        showerObject.SetActive(false);
    }

    /// <summary>Shows the amount of gems received from a purchase.</summary>
    /// <param name="amount">Number of gems purchased.</param>
    public void PlayerBoughtGems(int amount)
    {
        amountOfGemsText.text = amount.ToString();
        
        if(coroutine!=null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(GemsBoughtPop());
    }

    /// <summary>Displays the purchase message briefly before hiding it.</summary>
    private IEnumerator GemsBoughtPop()
    {
        showerObject.SetActive(true);
        showerCanvas.alpha = 1;

        yield return new WaitForSeconds(3);

        showerCanvas.alpha = 0;
        showerObject.SetActive(false);
    }
}
