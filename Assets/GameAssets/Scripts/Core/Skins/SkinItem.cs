using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinItem : MonoBehaviour
{
    [SerializeField] private Button equipButton;
    [SerializeField] private TMP_Text skinAmountText;
    [SerializeField] private TMP_Text skinNameText;
    [SerializeField] private TMP_Text skinEquipText;
    [SerializeField] private Image skinImage;
    [SerializeField] private Image equippedImage;

    private SkinManager skinManager;
    private SkinData skinData;

    public string SkinID = "";

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

    public void EquipState(bool state)
    {
        equipButton.gameObject.SetActive(!state);
        equippedImage.gameObject.SetActive(state);
    }

    public void EquipSkin()
    {
        if(skinManager.IsOwned(skinData.skinID))
        {
            skinManager.EquipSkin(skinData.skinID);
        }

        skinManager.PurchaseSkin(skinData);
    }
}
