using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Displays one assigned task and its current selection state.</summary>
public class TaskItem : MonoBehaviour
{
    [Header("Task Display")]
    [Tooltip("Text displaying the task name.")]
    [SerializeField] private TMP_Text taskNameText;
    [Tooltip("Text color used after the task is completed.")]
    [SerializeField] private Color completedColor;
    [Tooltip("Icon showing the task selection state.")]
    [SerializeField] private Image locationIconImage;
    [Tooltip("Border shown around the currently selected task.")]
    [SerializeField] private GameObject currentTaskBorder;
    [Tooltip("Whether this item is currently selected.")]
    [SerializeField] private bool isCurrentTask = false;

    private UITaskManager uITaskManager;
    private string TaskName = "";
    private int taskId = 0;
    private bool isCompleted = false;

    /// <summary>Resets the item to its initial unselected state.</summary>
    private void Start()
    {
        isCurrentTask = false;
        currentTaskBorder.SetActive(isCurrentTask);
        locationIconImage.color = Color.black;
    }

    /// <summary>Initializes the task text, ID, and owning UI manager.</summary>
    /// <param name="name">Player-facing task name.</param>
    /// <param name="taskID">Unique task identifier.</param>
    /// <param name="uITaskManager">Manager that owns this task item.</param>
    public void Initialize(string name, int taskID, UITaskManager uITaskManager)
    {
        taskNameText.text = name;
        TaskName = name;
        taskId = taskID;
        this.uITaskManager = uITaskManager;
    }

    /// <summary>Marks the task as completed in the UI.</summary>
    public void CompleteTask()
    {
        taskNameText.color = completedColor;
        taskNameText.text = $"<s>{TaskName}<s>";
        currentTaskBorder.SetActive(false);
        isCompleted = true;
    }

    /// <summary>Sets whether this task is the current task.</summary>
    /// <param name="state">True to select the task.</param>
    public void SetCurrentTask(bool state)
    {
        Debug.Log("I am clicked");
        if(isCompleted) return;

        if(state) uITaskManager.CurrentTask(taskId);
        isCurrentTask = state;
        currentTaskBorder.SetActive(state);
        locationIconImage.color = state ? Color.white : Color.black;
    }

    /// <summary>Returns whether this task is currently selected.</summary>
    public bool IsCurrentTask()
    {
        return isCurrentTask;
    }
}
