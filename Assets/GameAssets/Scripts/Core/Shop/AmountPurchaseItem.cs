using TMPro;
using UnityEngine;

public class AmountPurchaseItem : MonoBehaviour
{
    [SerializeField] private int AmountOfGems = 0;
    [SerializeField] private float RealWorldAmount = 0;

    [SerializeField] private TMP_Text gemsAmountText;
    [SerializeField] private TMP_Text realMoneyText;

    private GemsPurchaseManager gemsPurchaseManager;

    private void Start()
    {
        realMoneyText.text = $"$ {RealWorldAmount}";
        gemsAmountText.text = $"{AmountOfGems}";
        gemsPurchaseManager = GameManager.Instance.GetGemsPurchaseManager();
    }

    public void PurchaseGems()
    {
        gemsPurchaseManager.PurchaseGems(AmountOfGems, RealWorldAmount);
    }
}
