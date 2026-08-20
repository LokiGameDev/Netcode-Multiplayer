using System.Collections.Generic;
using UnityEngine;

public class UITaskManager : MonoBehaviour
{
    [SerializeField] private Transform taskContainer;
    [SerializeField] private TaskItem taskItemPrefab;

    private Dictionary<int, PlayerTask> currentPlayerTasks = new Dictionary<int, PlayerTask>();
    private Dictionary<int, TaskItem> currentTaskItems = new Dictionary<int, TaskItem>();

    public void InitiatePlayerTasks(Dictionary<int, PlayerTask> tasks)
    {
        foreach(PlayerTask playerTask in tasks.Values)
        {
            TaskItem task = Instantiate(taskItemPrefab, taskContainer);
            task.SetTaskName(GetTaskName(playerTask.Type));
            currentTaskItems[playerTask.TaskId] = task;
            currentPlayerTasks[playerTask.TaskId] = playerTask;
        }
    }

    public void CompleteTask(int taskID)
    {
        if(!currentPlayerTasks[taskID].Completed)
        {
            currentTaskItems[taskID].CompleteTask();
            PlayerTask completedTask = currentPlayerTasks[taskID];
            completedTask.Completed = true;
            currentPlayerTasks[taskID] = completedTask;
        }
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
}
