using Unity.Netcode;
using UnityEngine;

public interface ITask
{
    public int taskId {get; set;}
    public NetworkVariable<int> TaskId {get; set;}
    public TaskType taskType {get;} 
    public string ActionName { get; set; }
    public string GetActionName();
    public void AssignTaskID(int id);
    public void Interact(PlayerTaskManager playerTaskManager);
    public void CompleteTask();
    public Transform GetInteractionPoint();
}
