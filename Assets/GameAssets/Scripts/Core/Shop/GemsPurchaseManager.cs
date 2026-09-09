using TMPro;
using UnityEngine;

/// <summary>Controls gem purchase confirmation and completion.</summary>
public class GemsPurchaseManager : MonoBehaviour
{
    [Tooltip("Panel shown while a gem purchase awaits confirmation.")]
    [SerializeField] private GameObject confirmPurchasePanel;
    [Tooltip("Text displaying the selected gem amount.")]
    [SerializeField] private TMP_Text gemsInfoText;
    [Tooltip("Text displaying the selected real-world price.")]
    [SerializeField] private TMP_Text realMoneyInfoText;

    private int currentGemsForPurchase = 0;

    /// <summary>Hides the confirmation panel when the manager starts.</summary>
    private void Start()
    {
        confirmPurchasePanel.SetActive(false);
    }

    /// <summary>Displays a confirmation prompt for a gem purchase.</summary>
    public void PurchaseGems(int noOfGems, float realMoney)
    {
        currentGemsForPurchase = noOfGems;

        gemsInfoText.text = noOfGems.ToString();
        realMoneyInfoText.text = "$ " + realMoney.ToString();

        confirmPurchasePanel.SetActive(true);
    }

    /// <summary>Completes the pending purchase and updates the player balance.</summary>
    public void ConfirmPurchase()
    {
        PlayerPrefs.SetInt("PlayerGems", PlayerPrefs.GetInt("PlayerGems") + currentGemsForPurchase);
        GameManager.Instance.PlayerBoughtGems(currentGemsForPurchase);
        confirmPurchasePanel.SetActive(false);
    }
}
