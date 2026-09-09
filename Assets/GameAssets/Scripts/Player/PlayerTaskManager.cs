using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

/// <summary>Stores the local player's assigned tasks and completion updates.</summary>
public class PlayerTaskManager : NetworkBehaviour
{
    private Dictionary<int, PlayerTask> playerTasks = new Dictionary<int, PlayerTask>();

    private List<int> taskIds = new List<int>();

    [Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server)]
    /// <summary>Receives and displays tasks assigned by the server.</summary>
    /// <param name="playerTask">Tasks assigned to this player.</param>
    public void SetPlayerTasksRpc(PlayerTask[] playerTask, RpcParams rpcParams = default)
    {
        foreach(PlayerTask playerTask1 in playerTask)
        {
            playerTasks[playerTask1.TaskId] = playerTask1;
            taskIds.Add(playerTask1.TaskId);
        }
        DisplayAllTasks();
    }

    [Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server)]
    /// <summary>Receives a server notification that a task was completed.</summary>
    /// <param name="taskId">Completed task identifier.</param>
    public void PlayerTaskCompletedRpc(int taskId, RpcParams rpcParams = default)
    {
        CompletedTask(taskId);
    }

    /// <summary>Logs and sends the assigned tasks to the game UI.</summary>
    private void DisplayAllTasks()
    {
        Debug.Log("I am Assigned :");
        foreach(PlayerTask task in playerTasks.Values)
        {
            Debug.Log($"{task.TaskId}:{task.Type}");
        }
        UIManager.Instance?.InitiatePlayerTasksUI(playerTasks);
    }

    /// <summary>Marks a task complete in the game UI.</summary>
    private void CompletedTask(int taskId)
    {
       UIManager.Instance.CompleteTask(taskId); 
    }

    /// <summary>Checks whether the player owns a task ID.</summary>
    /// <param name="id">Task ID to find.</param>
    public bool IsPlayerHaveThisTask(int id)
    {
        return taskIds.Contains(id);
    }
}
