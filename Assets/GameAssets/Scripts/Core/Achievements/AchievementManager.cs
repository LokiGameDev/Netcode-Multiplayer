using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    private static AchievementManager instance;
    public static AchievementManager Instance
    {
        get
        {
            if(instance==null)
            {
                Debug.LogError("Achievement manager is null");
            }
            return instance;
        }
    }

    [SerializeField] private ShowNotification showNotification;

    private void Awake()
    {
        if(instance!=null && instance!=this) Destroy(gameObject);

        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        filePath = Path.Combine(Application.persistentDataPath, "achievements.json");
        
        LoadAchievements();
    }

    private void Start()
    {
        if(achievementDatas==null) return;

        foreach(var data in achievementDatas)
        {
            if(data.unlocked) unlocked.Add(data.id);
        }
    }

    [SerializeField] private AchievementDatabase achievementDatabase;

    private List<AchievementData> achievementDatas = new();
    private HashSet<string> unlocked = new();

    string filePath;

    public void Unlock(string id)
    {
        if (unlocked.Contains(id))
            return;

        foreach(var data in achievementDatas)
        {
            if(data.id == id)
            {
                data.unlocked = true;
                break;
            }
        }
        unlocked.Add(id);

        SaveAchievements();

        Debug.Log($"Achievement Unlocked: {id}");

        showNotification.ShowText("Achievement Unlocked");

        // Show unlock popup
    }

    public bool IsUnlocked(string id)
    {
        return unlocked.Contains(id);
    }

    private void SaveAchievements()
    {
        AchievementSaveData data = new AchievementSaveData
        {
            achievements = achievementDatas
        };

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(filePath, json);

        Debug.Log("Saved to: " + filePath);
    }

    public List<AchievementData> LoadAchievements()
    {
        if (!File.Exists(filePath))
        {
            InitiateAchievements();
            return null;
        }

        string json = File.ReadAllText(filePath);

        achievementDatas = JsonUtility.FromJson<AchievementSaveData>(json).achievements;

        return achievementDatas;
    }

    private void InitiateAchievements()
    {
        AchievementDefinition[] datas = GetAllAchievements();

        List<AchievementData> newData = new();

        foreach(var data in datas)
        {
            newData.Add(new AchievementData(data.id, false));
        }

        AchievementSaveData saveData = new AchievementSaveData
        {
            achievements = newData
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(filePath, json);

        Debug.Log("Initiated achievements: " + filePath);

        achievementDatas = newData;

        foreach(var data in achievementDatas)
        {
            if(data.unlocked) unlocked.Add(data.id);
        }
    }

    public AchievementDefinition[] GetAllAchievements()
    {
        return achievementDatabase.achievementDefinitions;
    }
}

[System.Serializable]
public class AchievementSaveData
{
    public List<AchievementData> achievements;
}

[System.Serializable]
public class AchievementData
{
    public string id;
    public bool unlocked;

    public AchievementData(string id, bool unlocked)
    {
        this.id = id;
        this.unlocked = unlocked;
    }
}