using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SkinManager : MonoBehaviour
{
    [SerializeField] private SkinDatatbase skinDatatbase;
    [SerializeField] private GameObject playerRoot;
    [SerializeField] private GameObject demoPlayer;
    [SerializeField] private SkinMenuManager skinMenuManager;
    [SerializeField] private ShowNotification showNotification;
    [SerializeField] private GameObject purchaseSkinConfirmationPanel;
    [SerializeField] private TMP_Text skinNameText;
    [SerializeField] private TMP_Text skinCostText;

    private string PlayerSkinID = "PlayerSkinID";
    private GameObject currentSkin;
    private SkinData currentSkinForPurchase;

    public UnityEvent skinEquippedEvent;

    private void Start()
    {
        skinDatatbase.Initialize();

        InitializePlayerOwnership();

        InitiatePlayerSkin();

        InitiateSkinMenu();
    }

    private void InitiatePlayerSkin()
    {
        SkinData skinData = skinDatatbase.Get(PlayerPrefs.GetString(PlayerSkinID, "Default"));

        currentSkin = Instantiate(skinData.skinPrefab, playerRoot.transform);

        demoPlayer.SetActive(true);

        demoPlayer.GetComponent<MenuPlayerInteraction>().Initialize();
    }

    private void ChangeSkin(string id)
    {
        Debug.Log($"Trying to equip: {id}");

        if(PlayerPrefs.GetString(PlayerSkinID) == id) return;

        demoPlayer.SetActive(false);

        SkinData skinData = skinDatatbase.Get(id);

        if(skinData==null)
        {
            demoPlayer.SetActive(true);
            return;
        }

        Destroy(currentSkin);

        PlayerPrefs.SetString(PlayerSkinID, id);

        currentSkin = Instantiate(skinData.skinPrefab, playerRoot.transform);

        demoPlayer.GetComponent<MenuPlayerInteraction>().ResetSkin(currentSkin);

        demoPlayer.SetActive(true);

        skinMenuManager.CurrentEquippedSkin(id);

        skinEquippedEvent?.Invoke();

        Debug.Log($"Equipped: {id}");
    }

    public void EquipSkin(string skinId)
    {
        if(PlayerPrefs.GetString(PlayerSkinID) == skinId) return;

        ChangeSkin(skinId);
    }

    public bool PurchaseSkin(SkinData skinData)
    {
        int amount = PlayerPrefs.GetInt("PlayerGems");

        if(amount >= skinData.skinAmount)
        {
            currentSkinForPurchase = skinData;
            skinNameText.text = currentSkinForPurchase.skinID;
            skinCostText.text = currentSkinForPurchase.skinAmount.ToString();
            purchaseSkinConfirmationPanel.SetActive(true);
        }
        else
        {
            showNotification.ShowText("You don't have enough money");
        }
        return false;
    }

    public void PurchaseSkinConfirmation()
    {
        skinDatatbase.PurchaseSkin(currentSkinForPurchase.skinID);
        EquipSkin(currentSkinForPurchase.skinID);
        GameManager.Instance.PlayerSpentGems(currentSkinForPurchase.skinAmount);
    }

    public List<SkinData> GetAllSkins()
    {
        return skinDatatbase.GetAllSkins();
    }

    public bool IsOwned(string id)
    {
        return PlayerPrefs.GetInt(id, 0) == 1;
    }

    private void InitiateSkinMenu()
    {
        skinMenuManager.Initialize(this);
    }

    private void InitializePlayerOwnership()
    {
        List<SkinData> skinDatas = skinDatatbase.GetAllSkins();

        foreach(var skin in skinDatas)
        {
            if(!PlayerPrefs.HasKey(skin.skinID))
            {
                if(skin.skinID == "Default") PlayerPrefs.SetInt(skin.skinID, 1);
                else PlayerPrefs.SetInt(skin.skinID, 0);
            }
        }
    }
}
