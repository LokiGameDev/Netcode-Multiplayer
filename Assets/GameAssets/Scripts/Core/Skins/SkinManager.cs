using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>Manages skin ownership, purchases, and equipped player visuals.</summary>
public class SkinManager : MonoBehaviour
{
    [Tooltip("Database containing the available skins.")]
    [SerializeField] private SkinDatatbase skinDatatbase;
    [Tooltip("Root transform for the currently displayed player skin.")]
    [SerializeField] private GameObject playerRoot;
    [Tooltip("Demo player shown in the skin menu.")]
    [SerializeField] private GameObject demoPlayer;
    [Tooltip("Manager responsible for the skin list UI.")]
    [SerializeField] private SkinMenuManager skinMenuManager;
    [Tooltip("Displays purchase and ownership messages.")]
    [SerializeField] private ShowNotification showNotification;
    [Tooltip("Confirmation panel shown before purchasing a skin.")]
    [SerializeField] private GameObject purchaseSkinConfirmationPanel;
    [Tooltip("Text displaying the selected skin name.")]
    [SerializeField] private TMP_Text skinNameText;
    [Tooltip("Text displaying the selected skin cost.")]
    [SerializeField] private TMP_Text skinCostText;

    private string PlayerSkinID = "PlayerSkinID";
    private GameObject currentSkin;
    private SkinData currentSkinForPurchase;

    public UnityEvent skinEquippedEvent;

    /// <summary>Initializes skin data, ownership, visuals, and menu UI.</summary>
    private void Start()
    {
        skinDatatbase.Initialize();

        InitializePlayerOwnership();

        InitiatePlayerSkin();

        InitiateSkinMenu();
    }

    /// <summary>Creates the currently equipped skin in the demo player.</summary>
    private void InitiatePlayerSkin()
    {
        if(!skinDatatbase.Contains(PlayerPrefs.GetString(PlayerSkinID, "Default")))
        {
            PlayerPrefs.SetString(PlayerSkinID, "Default");
        }

        SkinData skinData = skinDatatbase.Get(PlayerPrefs.GetString(PlayerSkinID, "Default"));

        currentSkin = Instantiate(skinData.skinPrefab, playerRoot.transform);

        demoPlayer.SetActive(true);

        demoPlayer.GetComponent<MenuPlayerInteraction>().Initialize();
    }

    /// <summary>Changes the demo player to the requested skin.</summary>
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

    /// <summary>Equips a skin when it is not already active.</summary>
    public void EquipSkin(string skinId)
    {
        if(PlayerPrefs.GetString(PlayerSkinID) == skinId) return;

        ChangeSkin(skinId);
    }

    /// <summary>Opens purchase confirmation when the player can afford a skin.</summary>
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

    /// <summary>Completes the pending skin purchase and equips it.</summary>
    public void PurchaseSkinConfirmation()
    {
        skinDatatbase.PurchaseSkin(currentSkinForPurchase.skinID);
        EquipSkin(currentSkinForPurchase.skinID);
        GameManager.Instance.PlayerSpentGems(currentSkinForPurchase.skinAmount);
    }

    /// <summary>Returns all skins available in the database.</summary>
    public List<SkinData> GetAllSkins()
    {
        return skinDatatbase.GetAllSkins();
    }

    /// <summary>Returns whether the player owns the requested skin.</summary>
    public bool IsOwned(string id)
    {
        return PlayerPrefs.GetInt(id, 0) == 1;
    }

    /// <summary>Initializes the skin menu with this manager.</summary>
    private void InitiateSkinMenu()
    {
        skinMenuManager.Initialize(this);
    }

    /// <summary>Creates default ownership entries for all configured skins.</summary>
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
