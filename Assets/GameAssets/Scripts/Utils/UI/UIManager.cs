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
}
