using TMPro;
using UnityEngine;

public class TaskItem : MonoBehaviour
{
    [SerializeField] private TMP_Text taskNameText;
    [SerializeField] private Color completedColor;

    private string TaskName = "";

    public void SetTaskName(string name)
    {
        taskNameText.text = name;
        TaskName = name;
    }

    public void CompleteTask()
    {
        taskNameText.color = completedColor;
        taskNameText.text = $"<s>{TaskName}<s>";
    }
}
