using TMPro;
using UnityEngine;

/// <summary>Displays one purchasable gem amount and its price.</summary>
public class AmountPurchaseItem : MonoBehaviour
{
    [Tooltip("Number of gems included in this purchase.")]
    [SerializeField] private int AmountOfGems = 0;
    [Tooltip("Real-world price displayed for this purchase.")]
    [SerializeField] private float RealWorldAmount = 0;

    [Tooltip("Text displaying the gem amount.")]
    [SerializeField] private TMP_Text gemsAmountText;
    [Tooltip("Text displaying the real-world price.")]
    [SerializeField] private TMP_Text realMoneyText;

    private GemsPurchaseManager gemsPurchaseManager;

    /// <summary>Initializes the displayed price and purchase manager.</summary>
    private void Start()
    {
        realMoneyText.text = $"$ {RealWorldAmount}";
        gemsAmountText.text = $"{AmountOfGems}";
        gemsPurchaseManager = GameManager.Instance.GetGemsPurchaseManager();
    }

    /// <summary>Requests the configured gem purchase.</summary>
    public void PurchaseGems()
    {
        gemsPurchaseManager.PurchaseGems(AmountOfGems, RealWorldAmount);
    }
}
