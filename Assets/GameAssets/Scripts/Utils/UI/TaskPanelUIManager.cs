using UnityEngine;

public class TaskPanelUIManager : MonoBehaviour
{
    [SerializeField] private UITaskManager uITaskManager;
    [Header("TaskPanels")]
    [SerializeField] private TaskUIReference[] taskUIReference;

    private GameObject currentTaskPanel;
    private int currentTaskId = -1;

    private GameObject GetTaskPanel(TaskType taskType)
    {
        foreach(TaskUIReference task in taskUIReference)
        {
            if(task.taskType == taskType) return task.taskPanel;
        }
        return taskUIReference[0].taskPanel;
    }

    public void StartTask(int id, TaskType taskType)
    {
        currentTaskPanel = GetTaskPanel(taskType);

        currentTaskId = id;

        if(currentTaskPanel==null) return;

        currentTaskPanel.SetActive(true);
    }

    public void CompleteTask()
    {
        uITaskManager.CompleteTask(currentTaskId);
        currentTaskPanel.SetActive(false);
        currentTaskPanel=null;
        currentTaskId = -1;
    }

    public void CloseCurrentTaskPanel()
    {
        currentTaskPanel.SetActive(false);
        currentTaskId = -1;
    }
}

[System.Serializable]
public class TaskUIReference
{
    public TaskType taskType;
    public GameObject taskPanel;
}