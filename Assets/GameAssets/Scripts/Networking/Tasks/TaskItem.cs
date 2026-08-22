using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskItem : MonoBehaviour
{
    [SerializeField] private TMP_Text taskNameText;
    [SerializeField] private Color completedColor;
    [SerializeField] private Image locationIconImage;
    [SerializeField] private GameObject currentTaskBorder;
    [SerializeField] private bool isCurrentTask = false;

    private UITaskManager uITaskManager;
    private string TaskName = "";
    private int taskId = 0;
    private bool isCompleted = false;

    private void Start()
    {
        isCurrentTask = false;
        currentTaskBorder.SetActive(isCurrentTask);
        locationIconImage.color = Color.black;
    }

    public void Initialize(string name, int taskID, UITaskManager uITaskManager)
    {
        taskNameText.text = name;
        TaskName = name;
        taskId = taskID;
        this.uITaskManager = uITaskManager;
    }

    public void CompleteTask()
    {
        taskNameText.color = completedColor;
        taskNameText.text = $"<s>{TaskName}<s>";
        currentTaskBorder.SetActive(false);
        isCompleted = true;
    }

    public void SetCurrentTask(bool state)
    {
        Debug.Log("I am clicked");
        if(isCompleted) return;

        if(state) uITaskManager.CurrentTask(taskId);
        isCurrentTask = state;
        currentTaskBorder.SetActive(state);
        locationIconImage.color = state ? Color.white : Color.black;
    }

    public bool IsCurrentTask()
    {
        return isCurrentTask;
    }
}
