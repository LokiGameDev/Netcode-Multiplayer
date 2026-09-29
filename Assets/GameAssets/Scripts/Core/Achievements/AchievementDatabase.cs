using UnityEngine;

[CreateAssetMenu(menuName = "Achievements/AchievementDatabase")]
public class AchievementDatabase : ScriptableObject
{
    public AchievementDefinition[] achievementDefinitions;
}