using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

/// <summary>Assigns tasks to players and tracks shared task progress.</summary>
public class TaskManager : NetworkBehaviour
{
    [Header("Task Configuration")]
    [Tooltip("List of world tasks available for assignment.")]
    public TaskList taskList;

    [Tooltip("Effect prefab played when a task is completed.")]
    [SerializeField] private GameObject taskCompletionEffectPrefab;

    private TaskCompletionEffect taskCompletionEffect;

    private Dictionary<ulong, List<PlayerTask>> assignedTasks = new Dictionary<ulong, List<PlayerTask>>();

    private Dictionary<ulong, int> completedTasksPlayerWise = new();

    private Dictionary<int, ITask> assignedTasksObjects = new Dictionary<int, ITask>();

    private Dictionary<TaskType, List<ITask>> availableTasks = new();

    private List<ulong> pendingPlayers = new List<ulong>();

    private int currentTaskId = 1;

    public NetworkVariable<int> currentTaskCount { get; set; } = new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<bool> isAssigning { get; set; } = new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<int> completedTaskCount { get; set; } = new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

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

    /// <summary>Creates the task completion effect instance.</summary>
    public void Start()
    {
        taskCompletionEffect = Instantiate(taskCompletionEffectPrefab, this.transform).GetComponent<TaskCompletionEffect>();
        
        RegisterTaskObjects();
    }

    /// <summary>Registers host callbacks and discovers world tasks.</summary>
    public override void OnNetworkSpawn()
    {
        if(!IsHost) return;

        // NetworkManager.Singleton.OnClientConnectedCallback += PlayerConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += PlayerDisconnected;
    }

    /// <summary>Unregisters host callbacks when the manager despawns.</summary>
    public override void OnNetworkDespawn()
    {
        if(!IsHost) return;

        // NetworkManager.Singleton.OnClientConnectedCallback -= PlayerConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= PlayerDisconnected;
    }

    /// <summary>Collects available task objects and assigns the host's tasks.</summary>
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

    /// <summary>Finds the world task associated with an assigned ID.</summary>
    /// <param name="id">Assigned task ID.</param>
    /// <returns>The matching task object, or null when it is not found.</returns>
    public ITask GetTaskObjectLocation(int id)
    {
        Dictionary<int, ITask> keyValuePairs = taskList.GetAssignedTasks();
        if(keyValuePairs.ContainsKey(id)) return keyValuePairs[id];
        return null;
    }

    /// <summary>Completes a player's task and updates shared progress.</summary>
    /// <param name="playerId">Client that completed the task.</param>
    /// <param name="taskId">Task ID to complete.</param>
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

        int index = tasks.FindIndex(t => t.TaskId == taskId);

        if (index != -1)
        {
            PlayerTask taskTP = tasks[index];
            taskTP.Completed = true;
            tasks[index] = taskTP;
        }

        assignedTasks[playerId] = tasks;

        assignedTasksObjects[taskId].CompleteTask();

        taskCompletionEffect.transform.position = assignedTasksObjects[taskId].GetInteractionPoint().position;
        taskCompletionEffect.PlayEffect(taskCompletionEffect.transform.position);

        Debug.Log($"[TASK MANAGER] Player {playerId} completed task {taskId}");

        if(!completedTasksPlayerWise.ContainsKey(playerId)) completedTasksPlayerWise.Add(playerId, 1);
        else completedTasksPlayerWise[playerId] += 1;

        Debug.Log($"[TASK MANAGER] Added 1 completion to player {playerId}, Total completion: {completedTasksPlayerWise[playerId]}");

        completedTaskCount.Value++;

        // Notify that player's client
        SendTaskCompletedRpc(
            taskId, playerId,
            RpcTarget.Single(playerId, RpcTargetUse.Temp)
        );

        // Server checks global progress
        CheckAllTasksCompleted();
    }

    [Rpc(SendTo.SpecifiedInParams)]
    /// <summary>Notifies a client that its task was completed.</summary>
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

    /// <summary>Checks whether all assigned tasks are complete.</summary>
    private void CheckAllTasksCompleted()
    {
        if(completedTaskCount.Value >= currentTaskCount.Value)
        {
            GameStateManager.Instance.GameFinished(GameState.GameWon);
        }
    }

    /// <summary>Assigns tasks to players waiting for task allocation.</summary>
    private void AssignTasksToPendingPlayers()
    {
        foreach(var id in pendingPlayers)
        {
            AssignTasks(id);
            pendingPlayers.Remove(id);
        }
    }

    /// <summary>Assigns tasks when a player connects.</summary>
    public void PlayerConnected(ulong playerId)
    {
        isAssigning.Value = true;
        AssignTasks(playerId);
    }

    /// <summary>Handles a player leaving the session.</summary>
    public void PlayerDisconnected(ulong playerId)
    {
        RemovePlayerTasks(playerId);
    }

    /// <summary>Sends the assigned task list to a client.</summary>
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

    /// <summary>Selects available tasks and assigns them to a player.</summary>
    private void AssignTasks(ulong playerId)
    {
        isAssigning.Value = true;

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

            currentTaskCount.Value++;

            // Add task to player's list
            tasks.Add(new PlayerTask
            {
                TaskId = worldTask.TaskId.Value,
                Type = worldTask.taskType
            });

            assignedTasksObjects[worldTask.TaskId.Value] = worldTask;

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

        if(!completedTasksPlayerWise.ContainsKey(playerId)) completedTasksPlayerWise.Add(playerId, 0);

        isAssigning.Value = false;
    }

    public void RemovePlayerTasks(ulong playerId)
    {
        Debug.Log("[TASK MANAGER] Removing player tasks");

        int playerCompletedTaskCount = completedTasksPlayerWise[playerId];

        completedTasksPlayerWise.Remove(playerId);

        Debug.Log($"[TASK MANAGER] Removing {playerCompletedTaskCount} completed tasks");

        completedTaskCount.Value -= playerCompletedTaskCount;

        currentTaskCount.Value -= assignedTasks[playerId].Count();

        Debug.Log("[TASK MANAGER] Current total tasks: " + currentTaskCount.Value);

        assignedTasks.Remove(playerId);
    }
}

/// <summary>Network-serializable task data assigned to one player.</summary>
public struct PlayerTask : INetworkSerializable
{
    public int TaskId;
    public TaskType Type;
    public bool Completed;

    /// <summary>Serializes the task ID, type, and completion state.</summary>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref TaskId);
        serializer.SerializeValue(ref Type);
        serializer.SerializeValue(ref Completed);
    }
}

/// <summary>Types of tasks that can appear in the game world.</summary>
public enum TaskType
{
    RepairLight,
    FixBrokenChair,
    CleanDebris,
    RebuildBrokenTomb,
    CloseTheOpenedTomb
}
