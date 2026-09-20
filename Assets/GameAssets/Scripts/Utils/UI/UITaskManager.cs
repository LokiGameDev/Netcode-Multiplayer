using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Builds task list UI and sends completion requests to the server.</summary>
public class UITaskManager : NetworkBehaviour
{
    [Header("Task List")]
    [Tooltip("Parent transform for assigned task entries.")]
    [SerializeField] private Transform taskContainer;
    [Tooltip("Prefab used for each assigned task entry.")]
    [SerializeField] private TaskItem taskItemPrefab;

    [Header("Progress")]
    [Tooltip("Parent transform for task progress bars.")]
    [SerializeField] private Transform taskCompletionContainer;
    [Tooltip("Prefab used for each task progress bar.")]
    [SerializeField] private GameObject taskBarPrefab;

    [Tooltip("Color applied to task bars.")]
    [SerializeField] private Color normalColor;
    [Tooltip("Color applied to completed task bars.")]
    [SerializeField] private Color completedColor;

    [Header("Task Guidance")]
    [Tooltip("Marker that points to the current task.")]
    [SerializeField] private TaskMarker taskMarker;
    [Tooltip("Manager that opens task detail panels.")]
    [SerializeField] private TaskPanelUIManager taskPanelUIManager;

    private List<GameObject> taskBars = new List<GameObject>();

    private Dictionary<int, PlayerTask> currentPlayerTasks = new Dictionary<int, PlayerTask>();
    private Dictionary<int, TaskItem> currentTaskItems = new Dictionary<int, TaskItem>();

    private int currentTaskId = 0;

    /// <summary>Creates UI entries for the supplied player tasks.</summary>
    /// <param name="tasks">Tasks to display.</param>
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

    /// <summary>Completes the selected task and requests server confirmation.</summary>
    /// <param name="taskID">Task identifier to complete.</param>
    public void CompleteTask(int taskID)
    {
        Debug.Log($"{taskID} Completion intiated");

        if(!currentPlayerTasks.ContainsKey(taskID)) return;

        if(!currentPlayerTasks[taskID].Completed && currentTaskId == taskID)
        {
            currentTaskItems[taskID].CompleteTask();
            PlayerTask completedTask = currentPlayerTasks[taskID];
            completedTask.Completed = true;
            currentPlayerTasks[taskID] = completedTask;
            CompleteTaskRpc(taskID, NetworkManager.Singleton.LocalClientId);
            taskMarker.SetCurrentTarget(null);
        }
    }

    /// <summary>Creates progress bars until the total task count is represented.</summary>
    /// <param name="count">Total number of tasks.</param>
    public void GameTotalTasks(int count)
    {
        Debug.Log("Game total tasks: " + count);

        int difference = count - taskCompletionContainer.childCount;

        if(difference < 0)
        {
            for(int i = 0; i < Math.Abs(difference); i++)
            {
                var obj = taskBars[0];
                taskBars.RemoveAt(0);
                Debug.Log($"Deleting task bar {obj.name}");
                Destroy(obj);
            }
        }
        else
        {
            for(int i = taskCompletionContainer.childCount; i < count; i++)
            {
                var taskBar = Instantiate(taskBarPrefab, taskCompletionContainer);
                taskBars.Add(taskBar);
            }
        }

        CompletedTaskCount(TaskManager.Instance.completedTaskCount.Value);
    }

    /// <summary>Colors the progress bars for completed tasks.</summary>
    /// <param name="count">Number of completed tasks.</param>
    public void CompletedTaskCount(int count)
    {
        Debug.Log($"Comparing task bars: {taskBars.Count} : {taskCompletionContainer.childCount}");

        for(int i = 0; i < Math.Min(taskBars.Count, taskCompletionContainer.childCount); i++)
        {
            if(i<count) taskBars[i].GetComponent<Image>().color = completedColor;
            else taskBars[i].GetComponent<Image>().color = normalColor;
        }
    }

    /// <summary>Selects a task and updates its world marker.</summary>
    /// <param name="id">Task identifier to select.</param>
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

    /// <summary>Opens the detail panel for a task.</summary>
    /// <param name="id">Task identifier.</param>
    /// <param name="taskType">Task type used to choose the panel.</param>
    public void StartTaskPanel(int id, TaskType taskType)
    {
        taskPanelUIManager.StartTask(id, taskType);
        currentTaskId = id;
    }

    /// <summary>Returns the player-facing name for a task type.</summary>
    /// <param name="taskType">Task type to name.</param>
    /// <returns>Display name for the task type.</returns>
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
                taskName = "Close the opened grave";
                break;
            case TaskType.FixBrokenChair:
                taskName = "Fix the broken chair";
                break;
            case TaskType.RebuildBrokenTomb:
                taskName = "Rebuild broken grave";
                break;
        }
        return taskName;
    }

    [Rpc(SendTo.Server)]
    /// <summary>Requests that the server complete a task for this player.</summary>
    /// <param name="taskId">Task identifier to complete.</param>
    private void CompleteTaskRpc(int taskId, ulong clientId)
    {
        TaskManager.Instance.CompleteTask(
            clientId,
            taskId
        );
    }
}
