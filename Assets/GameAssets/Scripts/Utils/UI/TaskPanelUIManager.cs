using UnityEngine;

/// <summary>Opens and closes the panel for the selected task.</summary>
public class TaskPanelUIManager : MonoBehaviour
{
    [Header("Task Panels")]
    [Tooltip("Task manager notified when a task is completed.")]
    [SerializeField] private UITaskManager uITaskManager;
    [Tooltip("Panel reference for each task type.")]
    [SerializeField] private TaskUIReference[] taskUIReference;

    private GameObject currentTaskPanel;
    private int currentTaskId = -1;

    /// <summary>Finds the configured panel for a task type.</summary>
    /// <param name="taskType">Task type to find.</param>
    /// <returns>The matching panel or the first configured panel.</returns>
    private GameObject GetTaskPanel(TaskType taskType)
    {
        foreach(TaskUIReference task in taskUIReference)
        {
            if(task.taskType == taskType) return task.taskPanel;
        }
        return taskUIReference[0].taskPanel;
    }

    /// <summary>Opens the panel for a selected task.</summary>
    /// <param name="id">Task identifier.</param>
    /// <param name="taskType">Task type used to select the panel.</param>
    public void StartTask(int id, TaskType taskType)
    {
        currentTaskPanel = GetTaskPanel(taskType);

        currentTaskId = id;

        if(currentTaskPanel==null) return;

        currentTaskPanel.SetActive(true);
    }

    /// <summary>Completes the selected task and closes its panel.</summary>
    public void CompleteTask()
    {
        uITaskManager.CompleteTask(currentTaskId);
        currentTaskPanel.SetActive(false);
        currentTaskPanel=null;
        currentTaskId = -1;
    }

    /// <summary>Closes the selected task panel without completing it.</summary>
    public void CloseCurrentTaskPanel()
    {
        if(currentTaskPanel!=null) currentTaskPanel.SetActive(false);
        currentTaskId = -1;
    }
}

[System.Serializable]
public class TaskUIReference
{
    [Tooltip("Task type represented by the panel.")]
    public TaskType taskType;
    [Tooltip("Panel displayed for the task type.")]
    public GameObject taskPanel;
}