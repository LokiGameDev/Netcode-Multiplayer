using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerTaskManager : NetworkBehaviour
{
    private Dictionary<int, PlayerTask> playerTasks = new Dictionary<int, PlayerTask>();

    private List<int> taskIds = new List<int>();

    [Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server)]
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
    public void PlayerTaskCompletedRpc(int taskId, RpcParams rpcParams = default)
    {
        CompletedTask(taskId);
    }

    private void DisplayAllTasks()
    {
        Debug.Log("I am Assigned :");
        foreach(PlayerTask task in playerTasks.Values)
        {
            Debug.Log($"{task.TaskId}:{task.Type}");
        }
        UIManager.Instance?.InitiatePlayerTasksUI(playerTasks);
    }

    private void CompletedTask(int taskId)
    {
       UIManager.Instance.CompleteTask(taskId); 
    }

    public bool IsPlayerHaveThisTask(int id)
    {
        return taskIds.Contains(id);
    }
}
