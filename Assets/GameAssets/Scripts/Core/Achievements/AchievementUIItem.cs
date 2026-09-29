using TMPro;
using UnityEngine;

public class AchievementUIItem : MonoBehaviour
{
    [SerializeField] private TMP_Text achievementTitle;
    [SerializeField] private TMP_Text achievementState;
    [SerializeField] private TMP_Text achievementDescription;
    
    [SerializeField] private Color notAchievedColor;
    [SerializeField] private Color achievedColor;

    private AchievementData achievementData = new AchievementData("", false);

    public void SetUp(string title,string des, bool state)
    {
        achievementData.id = title;
        achievementData.unlocked = state;
        achievementDescription.text = des;

        achievementTitle.text = achievementData.id;
        achievementState.text = achievementData.unlocked ? "Achieved" : "Not Achieved";
        achievementState.color = achievementData.unlocked ? achievedColor : notAchievedColor;
    }

    public void UpdateState(bool state)
    {
        achievementData.unlocked = state;

        achievementState.text = achievementData.unlocked ? "Achieved" : "Not Achieved";
        achievementState.color = achievementData.unlocked ? achievedColor : notAchievedColor;
    }
}
