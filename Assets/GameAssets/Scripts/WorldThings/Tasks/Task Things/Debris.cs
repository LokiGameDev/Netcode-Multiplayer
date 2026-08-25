using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Debris : NetworkBehaviour, ITask
{
    public TaskType taskType => TaskType.CleanDebris;
    public string ActionName { get; set; }
    
    public NetworkVariable<bool> isCompleted { get; set; } = new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
    
    public NetworkVariable<bool> isAssigned { get; set; } = new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<int> TaskId { get; set; } = new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    [SerializeField] private string toActivateText = "";

    [SerializeField] private int DebugTaskID = 0;

    public UnityEvent OnStartEvent;
    public UnityEvent OnCompletionEvent;

    private Coroutine coroutine;

    private void OnEnable()
    {
        ActionName = toActivateText;
        isCompleted.OnValueChanged += CompletionTaskThings;
    }

    private void CompletionTaskThings(bool prev, bool current)
    {
        if(current) OnCompletionEvent?.Invoke();
        else OnStartEvent?.Invoke();
    }

    public void CompleteTask()
    {
        Debug.Log("Completed task");
        isCompleted.Value = true;
        OnCompletionEvent?.Invoke();
    }

    public string GetActionName()
    {
        return ActionName;
    }

    public void Interact()
    {
        Debug.Log("Interacted");

        if(isCompleted.Value) return;

        if(coroutine!=null) StopCoroutine(coroutine);
        
        UIManager.Instance.InitiateTask(TaskId.Value, taskType);
    }

    public void AssignTaskID(int id)
    {
        TaskId.Value = id;
        DebugTaskID = id;
        isAssigned.Value = true;
        OnStartEvent?.Invoke();
    }

    public Transform GetInteractionPoint()
    {
        return gameObject.transform;
    }
}
