using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Skin/Skin Data")]
/// <summary>Defines the identifier, price, and prefab for a player skin.</summary>
public class SkinData : ScriptableObject
{
    /// <summary>Unique identifier for the skin.</summary>
    public string skinID;
    /// <summary>Gem cost required to purchase the skin.</summary>
    public int skinAmount;
    /// <summary>Prefab used to render the skin.</summary>
    public GameObject skinPrefab;
}