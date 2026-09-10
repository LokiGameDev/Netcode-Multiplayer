using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkinDatabase", menuName = "Custom/Skin/Skin Database")]
/// <summary>Stores skin definitions and provides skin purchase lookups.</summary>
public class SkinDatatbase : ScriptableObject
{
    [Tooltip("Skin definitions available to the skin system.")]
    [SerializeField] private List<SkinData> definitions;

    private Dictionary<string, SkinData> lookup;

    /// <summary>Builds the lookup table from the configured skin definitions.</summary>
    public void Initialize()
    {
        lookup = new Dictionary<string, SkinData>();

        foreach (var definition in definitions)
        {
            lookup[definition.skinID] = definition;
        }
    }

    /// <summary>Returns the skin definition for an identifier.</summary>
    public SkinData Get(string id)
    {
        if (lookup.TryGetValue(id, out var definition))
            return definition;

        Debug.LogWarning($"Skin '{id}' not found.");
        return null;
    }

    /// <summary>Returns all configured skin definitions.</summary>
    public List<SkinData> GetAllSkins()
    {
        return definitions;
    }

    /// <summary>Stores ownership for the selected skin.</summary>
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

    /// <summary>
    /// Returns whether the database contains the skinID
    /// </summary>
    /// <param name="skinID">SkinID of the particular skin</param>
    /// <returns>Presence of SkinID in database</returns>
    public bool Contains(string skinID)
    {
        foreach(var skin in definitions)
        {
            if(skin.skinID == skinID)
            {
                return true;
            }
        }
        return false;
    }
}
