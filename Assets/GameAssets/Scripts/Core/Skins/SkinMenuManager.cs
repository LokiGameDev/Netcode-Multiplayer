using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SkinMenuManager : MonoBehaviour
{
    [SerializeField] private Transform skinsContainer;
    [SerializeField] private SkinItem skinItemPrefab;
    [SerializeField] private SkinImageRef[] skinImageRefs;

    [Serializable]
    public class SkinImageRef
    {
        public SkinData skinData;
        public Sprite image;
    }

    private SkinManager skinManager;
    private List<SkinData> skinDatas = new List<SkinData>();

    private List<SkinItem> skinItems = new List<SkinItem>();

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

    public void CurrentEquippedSkin(string id)
    {
        foreach(var skin in skinItems)
        {
            skin.EquipState(id == skin.SkinID);
        }
    }

    private Sprite GetSpriteForSkin(SkinData skinData)
    {
        foreach(var skin in skinImageRefs)
        {
            if(skin.skinData.skinID == skinData.skinID) return skin.image;
        }

        return null;
    }
}
