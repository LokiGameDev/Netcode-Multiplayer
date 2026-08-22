using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UITaskManager : NetworkBehaviour
{
    [SerializeField] private Transform taskContainer;
    [SerializeField] private TaskItem taskItemPrefab;

    [SerializeField] private Transform taskCompletionContainer;
    [SerializeField] private GameObject taskBarPrefab;

    [SerializeField] private Color completedColor;

    [SerializeField] private TaskMarker taskMarker;
    [SerializeField] private TaskPanelUIManager taskPanelUIManager;

    private GameObject[] taskBars;

    private Dictionary<int, PlayerTask> currentPlayerTasks = new Dictionary<int, PlayerTask>();
    private Dictionary<int, TaskItem> currentTaskItems = new Dictionary<int, TaskItem>();

    private int currentTaskId = 0;

    public void InitiatePlayerTasks(Dictionary<int, PlayerTask> tasks)
    {
        foreach(PlayerTask playerTask in tasks.Values)
        {
            TaskItem task = Instantiate(taskItemPrefab, taskContainer);
            task.Initialize(GetTaskName(playerTask.Type), playerTask.TaskId, this);
            currentTaskItems[playerTask.TaskId] = task;
            currentPlayerTasks[playerTask.TaskId] = playerTask;
        }
    }

    public void CompleteTask(int taskID)
    {
        if(!currentPlayerTasks[taskID].Completed && currentTaskId == taskID)
        {
            currentTaskItems[taskID].CompleteTask();
            PlayerTask completedTask = currentPlayerTasks[taskID];
            completedTask.Completed = true;
            currentPlayerTasks[taskID] = completedTask;
            CompleteTaskRpc(taskID);
            taskMarker.SetCurrentTarget(null);
        }
    }

    public void GameTotalTasks(int count)
    {
        taskBars = new GameObject[count];

        for(int i=taskCompletionContainer.childCount;i<count;i++)
        {
            GameObject taskBar = Instantiate(taskBarPrefab, taskCompletionContainer);
            taskBars[i] = taskBar;
        }
    }

    public void CompletedTaskCount(int count)
    {
        for(int i=0;i<count;i++)
        {
            taskBars[i].GetComponent<Image>().color = completedColor;
        }
    }

    public void CurrentTask(int id)
    {
        foreach(var item in currentTaskItems)
        {
            if(item.Key != id) item.Value.SetCurrentTask(false);
        }

        ITask target = TaskManager.Instance.GetTaskObjectLocation(id);
        Debug.Log(target.GetInteractionPoint());
        if(target==null) return;
        taskMarker.SetCurrentTarget(target.GetInteractionPoint());
    }

    public void StartTaskPanel(int id, TaskType taskType)
    {
        taskPanelUIManager.StartTask(id, taskType);
        currentTaskId = id;
    }

    private string GetTaskName(TaskType taskType)
    {
        string taskName = "";
        switch(taskType)
        {
            case TaskType.RepairLight:
                taskName = "Repair the lights";
                break;
            case TaskType.CleanDebris:
                taskName = "Clean the debris";
                break;
            case TaskType.CloseTheOpenedTomb:
                taskName = "Close the opened tomb";
                break;
            case TaskType.FixBrokenChair:
                taskName = "Fix the broken chair";
                break;
            case TaskType.RebuildBrokenTomb:
                taskName = "Rebuild broken tomb";
                break;
        }
        return taskName;
    }

    [Rpc(SendTo.Server)]
    private void CompleteTaskRpc(int taskId)
    {
        TaskManager.Instance.CompleteTask(
            OwnerClientId,
            taskId
        );
    }
}
