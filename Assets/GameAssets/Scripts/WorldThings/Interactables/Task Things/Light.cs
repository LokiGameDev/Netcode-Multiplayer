using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Light : NetworkBehaviour, ITask
{
    public int taskId { get; set; }
    public TaskType taskType => TaskType.RepairLight;
    public string ActionName { get; set; }

    public NetworkVariable<int> TaskId { get; set; } = new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    [SerializeField] private string toActivateText = "";
 
    [SerializeField] private float lightLitTime = 3;
    [SerializeField] private int DebugTaskID = 0;

    private PlayerTaskManager playerTaskManager;
    private Coroutine coroutine;

    private void OnEnable()
    {
        ActionName = toActivateText;
    }

    public void CompleteTask()
    {
        Debug.Log("Completed task");
        playerTaskManager.TaskComplete(taskId);
    }

    public string GetActionName()
    {
        return ActionName;
    }

    public void Interact(PlayerTaskManager playerTaskManager)
    {
        Debug.Log("Interacted");
        this.playerTaskManager = playerTaskManager;
        if(coroutine!=null) StopCoroutine(coroutine);
        
        coroutine = StartCoroutine(StartInteracting());
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

    private IEnumerator StartInteracting()
    {
        Debug.Log("Started task");
        yield return new WaitForSeconds(lightLitTime);
        CompleteTask();
        coroutine = null;
    }
}
