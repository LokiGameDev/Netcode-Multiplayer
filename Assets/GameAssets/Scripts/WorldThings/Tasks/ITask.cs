using Unity.Netcode;
using UnityEngine;

/// <summary>Defines the networked contract for a world task.</summary>
public interface ITask
{
    /// <summary>Gets or sets the task identifier.</summary>
    public NetworkVariable<int> TaskId {get; set;}
    /// <summary>Gets or sets whether the task has been assigned.</summary>
    public NetworkVariable<bool> isAssigned { get; set; }
    /// <summary>Gets the task category.</summary>
    public TaskType taskType {get;} 
    /// <summary>Gets or sets whether the task is complete.</summary>
    public NetworkVariable<bool> isCompleted { get; set; }
    /// <summary>Gets or sets the action label shown to the player.</summary>
    public string ActionName { get; set; }
    /// <summary>Returns the current interaction label.</summary>
    public string GetActionName();
    /// <summary>Assigns an identifier to the task.</summary>
    public void AssignTaskID(int id);
    /// <summary>Starts interaction with the task.</summary>
    public void Interact();
    /// <summary>Marks the task as complete.</summary>
    public void CompleteTask();
    /// <summary>Returns the world point used for interaction.</summary>
    public Transform GetInteractionPoint();
}
