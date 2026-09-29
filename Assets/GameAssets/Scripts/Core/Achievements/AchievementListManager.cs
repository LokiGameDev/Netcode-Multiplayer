using System.Collections.Generic;
using UnityEngine;

public class AchievementListManager : MonoBehaviour
{
    [SerializeField] private AchievementUIItem achievementItemPrefab;
    [SerializeField] private Transform achievementsParent;

    private Dictionary<string, AchievementUIItem> localUIItems = new();
    void OnEnable()
    {
        LoadAllAchievements();
    }

    private void LoadAllAchievements()
    {
        AchievementDefinition[] datas = AchievementManager.Instance.GetAllAchievements();

        foreach(var data in datas)
        {
            if(localUIItems.ContainsKey(data.id))
            {
                localUIItems[data.id].UpdateState(AchievementManager.Instance.IsUnlocked(data.id));
                continue;
            }

            var item = Instantiate(achievementItemPrefab, achievementsParent);
            item.SetUp(data.title, data.description, AchievementManager.Instance.IsUnlocked(data.id));
            localUIItems[data.id] = item;
        }
    }
}
