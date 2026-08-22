using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BrokenTomb : NetworkBehaviour, ITask
{
    public int taskId { get; set; }
    public TaskType taskType => TaskType.RebuildBrokenTomb;
    public string ActionName { get; set; }
    public bool isCompleted { get; set; } = false;

    public NetworkVariable<int> TaskId { get; set; } = new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    [SerializeField] private string toActivateText = "";

    [SerializeField] private int DebugTaskID = 0;

    private Coroutine coroutine;

    private void OnEnable()
    {
        ActionName = toActivateText;
    }

    public void CompleteTask()
    {
        Debug.Log("Completed task");
        isCompleted = true;
    }

    public string GetActionName()
    {
        return ActionName;
    }

    public void Interact()
    {
        Debug.Log("Interacted");

        if(isCompleted) return;

        UIManager.Instance.InitiateTask(TaskId.Value, taskType);
    }

    public void AssignTaskID(int id)
    {
        taskId = id;
        TaskId.Value = id;
        DebugTaskID = id;
    }

    public Transform GetInteractionPoint()
    {
        return gameObject.transform;
    }
}
