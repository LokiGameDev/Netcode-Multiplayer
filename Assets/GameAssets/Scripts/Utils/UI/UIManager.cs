using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameHUD gameHUD;
    [SerializeField] private UITaskManager uITaskManager;
    [SerializeField] private TMP_Text debugText;

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

    private void Start()
    {
        TaskManager.Instance.completedTaskCount.OnValueChanged += FillCompletedTaskCount;
        TaskManager.Instance.currentTaskCount.OnValueChanged += FillTheTaskBar;

        debugText.gameObject.SetActive(false);
    }

    public void InitiatePlayerTasksUI(Dictionary<int, PlayerTask> playerTasks)
    {
        uITaskManager.InitiatePlayerTasks(playerTasks);
        FillTheTaskBar(0,0);
    }

    public void CompleteTask(int taskId)
    {
        uITaskManager.CompleteTask(taskId);
    }

    public void FillTheTaskBar(int a, int b)
    {
        uITaskManager.GameTotalTasks(TaskManager.Instance.currentTaskCount.Value);
    }

    public void FillCompletedTaskCount(int a, int b)
    {
        uITaskManager.CompletedTaskCount(TaskManager.Instance.completedTaskCount.Value);
    }

    public Transform GetPlayerPosition()
    {
        //GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(NetworkManager.Singleton == null) return null;

        NetworkObject player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
    
        if(player!=null) return player.transform;

        return null;
    }

    public void InitiateTask(int id, TaskType taskType)
    {
        uITaskManager.StartTaskPanel(id, taskType);
    }

    public void DisplayDebugValues(Vector3 a, Vector2 b, Vector2 c, float d)
    {
        if(!debugText.gameObject.activeInHierarchy) debugText.gameObject.SetActive(true);
        debugText.text = $"ObjectToScreen: {a}\nPlayerToScreen: {b}\nPointerPosition: {c}\nAngle: {d}";
    }
}
