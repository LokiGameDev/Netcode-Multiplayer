using TMPro;
using UnityEngine;

public class GemsPurchaseManager : MonoBehaviour
{
    [SerializeField] private GameObject confirmPurchasePanel;
    [SerializeField] private TMP_Text gemsInfoText;
    [SerializeField] private TMP_Text realMoneyInfoText;

    private int currentGemsForPurchase = 0;

    private void Start()
    {
        confirmPurchasePanel.SetActive(false);
    }

    public void PurchaseGems(int noOfGems, float realMoney)
    {
        currentGemsForPurchase = noOfGems;

        gemsInfoText.text = noOfGems.ToString();
        realMoneyInfoText.text = "$ " + realMoney.ToString();

        confirmPurchasePanel.SetActive(true);
    }

    public void ConfirmPurchase()
    {
        PlayerPrefs.SetInt("PlayerGems", PlayerPrefs.GetInt("PlayerGems") + currentGemsForPurchase);
        GameManager.Instance.PlayerBoughtGems(currentGemsForPurchase);
        confirmPurchasePanel.SetActive(false);
    }
}
