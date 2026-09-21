using System.Collections.Generic;
using UnityEngine;

public class PlayerOfflineSkinManager : MonoBehaviour
{
    public string SkinID = "Default";

    [Header("Skin References")]
    [Tooltip("Database containing the available player skins.")]
    [SerializeField] private SkinDatatbase skinDatatbase;

    [Tooltip("Updates animations for the equipped skin.")]
    [SerializeField] private PlayerOfflineAnimationManager playerAnimationManager;

    [Tooltip("Parent transform for the equipped skin instance.")]
    [SerializeField] private Transform playerSkinRoot;

    /// <summary>Equips the initial skin when the player starts.</summary>
    private void Start()
    {
        SkinID = PlayerPrefs.GetString("PlayerSkinID", "Default");

        EquipNewSkin();
    }

    /// <summary>Sets and equips a skin by its identifier.</summary>
    /// <param name="id">Identifier of the skin to equip.</param>
    public void Initialize(string id)
    {
        if(SkinID == id) return;

        SkinID = id;

        EquipNewSkin();
    }

    /// <summary>Replaces the current skin model with the selected skin.</summary>
    private void EquipNewSkin()
    {
        SkinData currentSkin = skinDatatbase.Get(SkinID);

        Debug.Log($"{SkinID}: Got {currentSkin.skinID}");

        List<GameObject> skins = new List<GameObject>();

        for(int i = 0;i < playerSkinRoot.childCount; i++)
        {
            if(playerSkinRoot.GetChild(i).CompareTag("PlayerSkin"))
            {
                skins.Add(playerSkinRoot.GetChild(i).gameObject);
            }
        }

        foreach(var skin in skins)
        {
            Destroy(skin);
        }

        var skinAnimator = Instantiate(currentSkin.skinPrefab, playerSkinRoot).GetComponent<Animator>();

        playerAnimationManager.SetAnimator(skinAnimator);
    }
}
