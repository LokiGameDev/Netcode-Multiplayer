using Unity.Netcode;
using UnityEngine;

public interface ITask
{
    public NetworkVariable<int> TaskId {get; set;}
    public NetworkVariable<bool> isAssigned { get; set; }
    public TaskType taskType {get;} 
    public NetworkVariable<bool> isCompleted { get; set; }
    public string ActionName { get; set; }
    public string GetActionName();
    public void AssignTaskID(int id);
    public void Interact();
    public void CompleteTask();
    public Transform GetInteractionPoint();
}
