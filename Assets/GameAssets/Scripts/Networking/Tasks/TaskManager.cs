using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class TaskManager : NetworkBehaviour
{
    public TaskList taskList;

    [SerializeField] private GameObject taskCompletionEffectPrefab;

    private TaskCompletionEffect taskCompletionEffect;

    private Dictionary<ulong, List<PlayerTask>> assignedTasks = new Dictionary<ulong, List<PlayerTask>>();

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

    public void Start()
    {
        taskCompletionEffect = Instantiate(taskCompletionEffectPrefab, this.transform).GetComponent<TaskCompletionEffect>();
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

    public ITask GetTaskObjectLocation(int id)
    {
        Dictionary<int, ITask> keyValuePairs = taskList.GetAssignedTasks();
        if(keyValuePairs.ContainsKey(id)) return keyValuePairs[id];
        return null;
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

        assignedTasksObjects[taskId].CompleteTask();

        taskCompletionEffect.transform.position = assignedTasksObjects[taskId].GetInteractionPoint().position;
        taskCompletionEffect.PlayEffect(taskCompletionEffect.transform.position);

        Debug.Log($"Player {playerId} completed task {taskId}");

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

    private void CheckAllTasksCompleted()
    {
        if(completedTaskCount.Value >= assignedTasksObjects.Count)
        {
            GameStateManager.Instance.GameFinished(GameState.GameWon);
        }
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
        isAssigning.Value = true;
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

        isAssigning.Value = false;
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
