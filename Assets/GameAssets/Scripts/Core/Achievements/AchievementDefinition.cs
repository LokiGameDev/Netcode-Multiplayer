using UnityEngine;

[CreateAssetMenu(menuName = "Achievements/Achievement")]
public class AchievementDefinition : ScriptableObject
{
    public string id;
    public string title;
    [TextArea] public string description;
    public Sprite icon;
}