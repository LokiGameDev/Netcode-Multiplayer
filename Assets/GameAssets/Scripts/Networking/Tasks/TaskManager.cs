using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class TaskManager : NetworkBehaviour
{

    private Dictionary<ulong, List<PlayerTask>> assignedTasks = new Dictionary<ulong, List<PlayerTask>>();

    private Dictionary<TaskType, List<ITask>> availableTasks = new();

    private List<ulong> pendingPlayers = new List<ulong>();

    private int currentTaskId = 1;

    private static TaskManager instance;
    public static TaskManager Instance
    {
        get
        {
            if(instance != null) return instance;

            instance = FindAnyObjectByType<TaskManager>();

            if(instance==null) Debug.LogError("TaskManager is null");

            return instance;
        }
    }

    public override void OnNetworkSpawn()
    {
        if(!IsHost) return;

        NetworkManager.Singleton.OnClientConnectedCallback += PlayerConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += PlayerDisconnected;

        RegisterTaskObjects();
    }

    public override void OnNetworkDespawn()
    {
        if(!IsHost) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= PlayerConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= PlayerDisconnected;
    }

    private void RegisterTaskObjects()
    {
        ITask[] taskObjects =
            FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None)
            .OfType<ITask>()
            .ToArray();

        foreach (ITask task in taskObjects)
        {
            if (!availableTasks.ContainsKey(task.taskType))
                availableTasks[task.taskType] = new List<ITask>();

            availableTasks[task.taskType].Add(task);
        }

        Debug.Log("Tasks Found:");
        foreach(var pair in availableTasks)
        {
            Debug.Log($"{pair.Key} : {pair.Value.Count}");
        }

        // Host already exists
        if (NetworkManager.Singleton.IsHost)
        {
            ulong hostId = NetworkManager.Singleton.LocalClientId;
            AssignTasks(hostId);
        }
    }

    public void CompleteTask(ulong playerId, int taskId)
    {
        if (!IsServer)
            return;

        // Find the player's task
        if (!assignedTasks.TryGetValue(playerId, out var tasks))
            return;

        PlayerTask task = tasks.Find(x => x.TaskId == taskId);

        if (task.Completed)
            return;

        // IMPORTANT:
        // Validation that the player is actually allowed to complete this task.

        task.Completed = true;

        Debug.Log($"Player {playerId} completed task {taskId}");

        // Notify that player's client
        SendTaskCompletedRpc(
            taskId, playerId,
            RpcTarget.Single(playerId, RpcTargetUse.Temp)
        );

        // Server checks global progress
        //CheckAllTasksCompleted();
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SendTaskCompletedRpc(int taskId, ulong playerId, RpcParams rpcParams = default)
    {
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(
        playerId, out Unity.Netcode.NetworkClient client))
        return;

        PlayerTaskManager player =
            client.PlayerObject.GetComponent<PlayerTaskManager>();

        player.PlayerTaskCompletedRpc(
            taskId,
            RpcTarget.Single(playerId, RpcTargetUse.Temp)
        );
    }

    private void AssignTasksToPendingPlayers()
    {
        foreach(var id in pendingPlayers)
        {
            AssignTasks(id);
            pendingPlayers.Remove(id);
        }
    }

    private void PlayerConnected(ulong playerId)
    {
        AssignTasks(playerId);
    }

    private void PlayerDisconnected(ulong playerId)
    {
        
    }

    private void SendTasksToPlayer(ulong playerId)
    {
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(
        playerId, out Unity.Netcode.NetworkClient client))
        return;

        PlayerTaskManager player =
            client.PlayerObject.GetComponent<PlayerTaskManager>();

        player.SetPlayerTasksRpc(
            assignedTasks[playerId].ToArray(),
            RpcTarget.Single(playerId, RpcTargetUse.Temp)
        );
    }

    private void AssignTasks(ulong playerId)
    {
        int numberOfTasks = Random.Range(3,5);

        Debug.Log($"For {playerId} Tasks are:");

        List<PlayerTask> tasks = new List<PlayerTask>();

        List<ITask> allAvailable =
        availableTasks.Values
            .SelectMany(x => x)
            .ToList();

        numberOfTasks = Mathf.Min(numberOfTasks, allAvailable.Count);

        for (int i = 0; i < numberOfTasks; i++)
        {
            int randomIndex = Random.Range(0, allAvailable.Count);

            ITask worldTask = allAvailable[randomIndex];

            worldTask.AssignTaskID(currentTaskId);
            currentTaskId++;
            // Add task to player's list
            tasks.Add(new PlayerTask
            {
                TaskId = worldTask.taskId,
                Type = worldTask.taskType
            });

            // Remove it from available pools
            availableTasks[worldTask.taskType].Remove(worldTask);
            allAvailable.RemoveAt(randomIndex);
        }

        assignedTasks[playerId] = tasks;

        Debug.Log($"Assigned {tasks.Count} tasks to player {playerId}");

        foreach (var task in tasks)
        {
            Debug.Log(
                $"Player {playerId}: {task.Type} (ID {task.TaskId})");
        }   

        SendTasksToPlayer(playerId);
    }
}

public struct PlayerTask : INetworkSerializable
{
    public int TaskId;
    public TaskType Type;
    public bool Completed;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref TaskId);
        serializer.SerializeValue(ref Type);
        serializer.SerializeValue(ref Completed);
    }
}

public enum TaskType
{
    RepairLight,
    FixBrokenChair,
    CleanDebris,
    RebuildBrokenTomb,
    CloseTheOpenedTomb
}
