using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

/// <summary>Represents the networked chair-repair task.</summary>
public class Chair : NetworkBehaviour, ITask
{
    /// <summary>Gets the task category.</summary>
    public TaskType taskType => TaskType.FixBrokenChair;
    /// <summary>Gets or sets the player-facing action label.</summary>
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

    [Tooltip("Action label shown while the chair can be repaired.")]
    [SerializeField] private string toActivateText = "";
    [Tooltip("Debug task identifier assigned at runtime.")]
    [SerializeField] private int DebugTaskID = 0;
    public UnityEvent OnStartEvent;
    public UnityEvent OnCompletionEvent;

    private Coroutine coroutine;
 
    /// <summary>Initializes the action label and completion callback.</summary>
    private void OnEnable()
    {
        ActionName = toActivateText;
        isCompleted.OnValueChanged += CompletionTaskThings;
    }

    /// <summary>Invokes the start or completion event for the task state.</summary>
    private void CompletionTaskThings(bool prev, bool current)
    {
        if(current) OnCompletionEvent?.Invoke();
        else OnStartEvent?.Invoke();
    }

    /// <summary>Marks the chair task complete.</summary>
    public void CompleteTask()
    {
        Debug.Log("Completed task");
        isCompleted.Value = true;
        OnCompletionEvent?.Invoke();
    }

    /// <summary>Returns the current action label.</summary>
    public string GetActionName()
    {
        return ActionName;
    }

    /// <summary>Starts the chair task UI when the task is incomplete.</summary>
    public void Interact()
    {
        Debug.Log("Interacted");

        if(isCompleted.Value) return;
        
        UIManager.Instance.InitiateTask(TaskId.Value, taskType);
    }

    /// <summary>Assigns the task ID and marks the task as active.</summary>
    public void AssignTaskID(int id)
    {
        TaskId.Value = id;
        DebugTaskID = id;
        isAssigned.Value = true;
        OnStartEvent?.Invoke();
    }

    /// <summary>Returns the chair transform used for interaction.</summary>
    public Transform GetInteractionPoint()
    {
        return gameObject.transform;
    }
}
