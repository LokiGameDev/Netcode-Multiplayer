using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Skin/Skin Data")]
public class SkinData : ScriptableObject
{
    public string skinID;
    public int skinAmount;
    public GameObject skinPrefab;
}