using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameHUD gameHUD;
    [SerializeField] private UITaskManager uITaskManager;

    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if(instance==null)
            {
                Debug.LogError("UIManager is null");
                return null;
            }
            return instance;
        }
    }

    public void Awake()
    {
        if(instance!=null && instance!=this) Destroy(gameObject);

        if(instance==null) instance = this;
    }

    public void InitiatePlayerTasksUI(Dictionary<int, PlayerTask> playerTasks)
    {
        uITaskManager.InitiatePlayerTasks(playerTasks);
    }

    public void CompleteTask(int taskId)
    {
        uITaskManager.CompleteTask(taskId);
    }

    public void FillTheTaskBar(int count)
    {
        uITaskManager.GameTotalTasks(count);
    }

    public void FillCompletedTaskCount(int count)
    {
        uITaskManager.CompletedTaskCount(count);
    }

    public Transform GetPlayerPositionToScreen()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player!=null) return player.transform;

        return null;
    }

    public void InitiateTask(int id, TaskType taskType)
    {
        uITaskManager.StartTaskPanel(id, taskType);
    }
}
