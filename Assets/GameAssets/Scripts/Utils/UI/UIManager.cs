using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>Coordinates gameplay panels and task-related UI updates.</summary>
public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("HUD used for gameplay UI.")]
    [SerializeField] private GameHUD gameHUD;
    [Tooltip("Manager used to display assigned tasks.")]
    [SerializeField] private UITaskManager uITaskManager;
    [Tooltip("Text used for temporary debug values.")]
    [SerializeField] private TMP_Text debugText;

    [Header("Panels")]
    [Tooltip("Panel shown while waiting for players.")]
    [SerializeField] private GameObject waitingPanel;
    [Tooltip("Panel shown during active gameplay.")]
    [SerializeField] private GameObject playingPanel;
    [Tooltip("Panel shown after the game is won.")]
    [SerializeField] private GameObject gameWonPanel;
    [Tooltip("Panel shown after the game is lost.")]
    [SerializeField] private GameObject gameLostPanel;
    [Tooltip("Panel shown while the game is loading.")]
    [SerializeField] private GameObject loadingPanel;

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

    /// <summary>Registers this object as the active UI manager.</summary>
    public void Awake()
    {
        if(instance!=null && instance!=this) Destroy(gameObject);

        if(instance==null) instance = this;
    }

    /// <summary>Subscribes to task counters and shows the waiting panel.</summary>
    private void Start()
    {
        TaskManager.Instance.completedTaskCount.OnValueChanged += FillCompletedTaskCount;
        TaskManager.Instance.currentTaskCount.OnValueChanged += FillTheTaskBar;

        debugText.gameObject.SetActive(false);

        SetPanel(waitingPanel);
    }

    /// <summary>Creates UI entries for the player's assigned tasks.</summary>
    /// <param name="playerTasks">Tasks to display.</param>
    public void InitiatePlayerTasksUI(Dictionary<int, PlayerTask> playerTasks)
    {
        uITaskManager.InitiatePlayerTasks(playerTasks);
        FillTheTaskBar(0,0);
    }

    /// <summary>Marks a task complete in the task UI.</summary>
    /// <param name="taskId">Completed task identifier.</param>
    public void CompleteTask(int taskId)
    {
        uITaskManager.CompleteTask(taskId);
    }

    /// <summary>Refreshes the total task progress bar count.</summary>
    private void FillTheTaskBar(int a, int b)
    {
        uITaskManager.GameTotalTasks(TaskManager.Instance.currentTaskCount.Value);
    }

    /// <summary>Refreshes the completed task progress count.</summary>
    private void FillCompletedTaskCount(int a, int b)
    {
        uITaskManager.CompletedTaskCount(TaskManager.Instance.completedTaskCount.Value);
    }

    /// <summary>Returns the local network player's transform.</summary>
    /// <returns>The local player transform, or null when unavailable.</returns>
    public Transform GetPlayerPosition()
    {
        //GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(NetworkManager.Singleton == null) return null;

        NetworkObject player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
    
        if(player!=null) return player.transform;

        return null;
    }

    /// <summary>Opens the panel for a selected task.</summary>
    /// <param name="id">Selected task identifier.</param>
    /// <param name="taskType">Selected task type.</param>
    public void InitiateTask(int id, TaskType taskType)
    {
        uITaskManager.StartTaskPanel(id, taskType);
    }

    /// <summary>Displays task-marker debug values.</summary>
    public void DisplayDebugValues(Vector3 a, Vector2 b, Vector2 c, float d)
    {
        if(!debugText.gameObject.activeInHierarchy) debugText.gameObject.SetActive(true);
        debugText.text = $"ObjectToScreen: {a}\nPlayerToScreen: {b}\nPointerPosition: {c}\nAngle: {d}";
    }

    /// <summary>Shows the panel matching the current game state.</summary>
    /// <param name="gameState">State whose panel should be shown.</param>
    public void ShowCurrentPanel(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.WaitingForPlayers:
                SetPanel(waitingPanel);
                break;
            case GameState.Playing:
                SetPanel(playingPanel);
                break;
            case GameState.GameWon:
                SetPanel(gameWonPanel);
                break;
            case GameState.GameLost:
                SetPanel(gameLostPanel);
                break;
            case GameState.Loading:
                SetPanel(loadingPanel);
                break;
        }
    }

    /// <summary>Activates one panel and disables the other game panels.</summary>
    /// <param name="activePanel">Panel to leave active.</param>
    private void SetPanel(GameObject activePanel)
    {
        waitingPanel.SetActive(activePanel == waitingPanel);
        playingPanel.SetActive(activePanel == playingPanel);
        gameWonPanel.SetActive(activePanel == gameWonPanel);
        gameLostPanel.SetActive(activePanel == gameLostPanel);
        loadingPanel.SetActive(activePanel == loadingPanel);
    }
}
