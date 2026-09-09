using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>Builds and updates the skin selection list.</summary>
public class SkinMenuManager : MonoBehaviour
{
    [Tooltip("Parent transform for skin list items.")]
    [SerializeField] private Transform skinsContainer;
    [Tooltip("Prefab used for each skin list item.")]
    [SerializeField] private SkinItem skinItemPrefab;
    [Tooltip("Skin data and preview image mappings.")]
    [SerializeField] private SkinImageRef[] skinImageRefs;

    [Serializable]
    /// <summary>Maps a skin definition to its preview image.</summary>
    public class SkinImageRef
    {
        /// <summary>Skin represented by this mapping.</summary>
        public SkinData skinData;
        /// <summary>Preview image for the skin.</summary>
        public Sprite image;
    }

    private SkinManager skinManager;
    private List<SkinData> skinDatas = new List<SkinData>();

    private List<SkinItem> skinItems = new List<SkinItem>();

    /// <summary>Creates list items for all skins managed by the menu.</summary>
    public void Initialize(SkinManager manager)
    {
        skinManager = manager;
        skinDatas = skinManager.GetAllSkins();
        foreach(var skin in skinDatas)
        {
            Sprite image = GetSpriteForSkin(skin);
            var item = Instantiate(skinItemPrefab, skinsContainer).GetComponent<SkinItem>();
            item.SetUp(skin, skinManager, image);
            skinItems.Add(item);
            if(PlayerPrefs.GetString("PlayerSkinID")==skin.skinID) item.EquipState(true);
        }
    }

    /// <summary>Updates list items to show the currently equipped skin.</summary>
    public void CurrentEquippedSkin(string id)
    {
        foreach(var skin in skinItems)
        {
            skin.EquipState(id == skin.SkinID);
        }
    }

    /// <summary>Finds the preview sprite mapped to a skin.</summary>
    private Sprite GetSpriteForSkin(SkinData skinData)
    {
        foreach(var skin in skinImageRefs)
        {
            if(skin.skinData.skinID == skinData.skinID) return skin.image;
        }

        return null;
    }
}
