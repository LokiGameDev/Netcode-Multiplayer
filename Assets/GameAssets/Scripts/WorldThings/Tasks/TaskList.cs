using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>Discovers world tasks and returns their assigned instances.</summary>
public class TaskList : MonoBehaviour
{
    [Tooltip("All task components discovered in the scene.")]
    [SerializeField] private List<ITask> listOfAllAvailableTasks = new List<ITask>();

    /// <summary>Refreshes the task list when the object starts.</summary>
    private void Start()
    {
        RefreshList();
    }

    /// <summary>Finds all task implementations in the scene.</summary>
    private void RefreshList()
    {
        listOfAllAvailableTasks = FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None)
            .OfType<ITask>().ToList();

        Debug.Log("List of Tasks:" + listOfAllAvailableTasks.Count);
    }

    /// <summary>Returns a map of assigned task IDs to task instances.</summary>
    public Dictionary<int ,ITask> GetAssignedTasks()
    {
        RefreshList();

        Dictionary<int, ITask> assignedTasks = new Dictionary<int, ITask>();
        foreach(ITask task in listOfAllAvailableTasks)
        {
            if(task.isAssigned.Value)
            {
                assignedTasks[task.TaskId.Value] = task;
                Debug.Log($"In List {task.TaskId.Value} : {task.taskType}");
            }
        }
        Debug.Log("Assigned Tasks:" + assignedTasks.Count);
        return assignedTasks;
    }

    /// <summary>Reserved entry point for retrieving all task instances.</summary>
    public void GetAllTask()
    {
        
    }
}
