using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskList : MonoBehaviour
{
    [SerializeField] private List<ITask> listOfAllAvailableTasks = new List<ITask>();

    private void Start()
    {
        RefreshList();
    }

    private void RefreshList()
    {
        listOfAllAvailableTasks = FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None)
            .OfType<ITask>().ToList();

        Debug.Log("List of Tasks:" + listOfAllAvailableTasks.Count);
    }

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

    public void GetAllTask()
    {
        
    }
}
