using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Displays a skin and exposes equip and purchase actions.</summary>
public class SkinItem : MonoBehaviour
{
    [Tooltip("Button used to equip the skin.")]
    [SerializeField] private Button equipButton;
    [Tooltip("Text displaying the skin price.")]
    [SerializeField] private TMP_Text skinAmountText;
    [Tooltip("Text displaying the skin name.")]
    [SerializeField] private TMP_Text skinNameText;
    [Tooltip("Text displayed when the skin is owned.")]
    [SerializeField] private TMP_Text skinEquipText;
    [Tooltip("Image used to preview the skin.")]
    [SerializeField] private Image skinImage;
    [Tooltip("Image shown when the skin is equipped.")]
    [SerializeField] private Image equippedImage;

    private SkinManager skinManager;
    private SkinData skinData;

    public string SkinID = "";

    /// <summary>Initializes the item from skin data and its manager.</summary>
    public void SetUp(SkinData skinData,SkinManager manager,Sprite imageSprite = null)
    {
        skinManager = manager;
        this.skinData = skinData;
        SkinID = skinData.skinID;
        if(imageSprite!=null)
        {
            skinImage.sprite = imageSprite;
        }
        skinNameText.text = this.skinData.skinID.Replace("_"," ");

        if(skinManager.IsOwned(skinData.skinID))
        {
            skinEquipText.gameObject.SetActive(true);
            skinAmountText.gameObject.SetActive(false);
        }
        else
        {
            skinEquipText.gameObject.SetActive(false);
            skinAmountText.gameObject.SetActive(true);

            if(skinData.skinAmount == 0) skinAmountText.text = "Free";
            else skinAmountText.text = this.skinData.skinAmount.ToString();
        }

        equipButton.gameObject.SetActive(true);
        equippedImage.gameObject.SetActive(false);
    }

    /// <summary>Updates the visual equipped state.</summary>
    public void EquipState(bool state)
    {
        equipButton.gameObject.SetActive(!state);
        equippedImage.gameObject.SetActive(state);
    }

    /// <summary>Equips or purchases the represented skin.</summary>
    public void EquipSkin()
    {
        if(skinManager.IsOwned(skinData.skinID))
        {
            skinManager.EquipSkin(skinData.skinID);
        }

        skinManager.PurchaseSkin(skinData);
    }
}
