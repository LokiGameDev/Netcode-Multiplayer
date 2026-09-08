using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkinDatabase", menuName = "Custom/Skin/Skin Database")]
public class SkinDatatbase : ScriptableObject
{
    [SerializeField] private List<SkinData> definitions;

    private Dictionary<string, SkinData> lookup;

    public void Initialize()
    {
        lookup = new Dictionary<string, SkinData>();

        foreach (var definition in definitions)
        {
            lookup[definition.skinID] = definition;
        }
    }

    public SkinData Get(string id)
    {
        if (lookup.TryGetValue(id, out var definition))
            return definition;

        Debug.LogWarning($"Skin '{id}' not found.");
        return null;
    }

    public List<SkinData> GetAllSkins()
    {
        return definitions;
    }

    public void PurchaseSkin(string skinID)
    {
        foreach(var skin in definitions)
        {
            if(skin.skinID == skinID)
            {
                PlayerPrefs.SetInt(skin.skinID, 1);
            }
        }
    }
}
