using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

/// <summary>Tracks the shared game state and player readiness.</summary>
public class GameStateManager : NetworkBehaviour
{
    private static GameStateManager instance;
    public static GameStateManager Instance
    {
        get
        {
            if(instance==null)
            {
                Debug.LogError("Game State Manager is null");
            }
            return instance;
        }
    }

    /// <summary>Registers this component as the active game-state manager.</summary>
    private void Awake()
    {
        if(instance!=null & instance!=this) Destroy(this);

        if(instance==null) instance = this;
    }

    NetworkVariable<int> playerCount = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    NetworkVariable<int> maxPlayerCount = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    NetworkVariable<GameState> currentGameState = new NetworkVariable<GameState>(
        GameState.None,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkList<FixedString64Bytes> playerWaitingList =
        new NetworkList<FixedString64Bytes>(
            null,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public UnityEvent playercountChanged;

    /// <summary>Subscribes to network callbacks and initializes the host state.</summary>
    public override void OnNetworkSpawn()
    {
        currentGameState.OnValueChanged += OnGameStateChanged;
        playerCount.OnValueChanged += PlayerCountChanged;

        if(!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += PlayerConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += PlayerDisconnected;
        
        playerCount.Value += 1;
        maxPlayerCount.Value = HostSingleton.Instance.GameManager.currentLobby.MaxPlayers;
        PlayerJoined(HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId).userName);
        currentGameState.Value = GameState.WaitingForPlayers;
    }

    /// <summary>Unsubscribes from network callbacks when the object despawns.</summary>
    public override void OnNetworkDespawn()
    {
        if(!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= PlayerConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= PlayerDisconnected;
    }

    /// <summary>Applies the current state to the local UI.</summary>
    public void Start()
    {
        OnGameStateChanged(currentGameState.Value, currentGameState.Value);
    }

    /// <summary>Adds a connected player and starts the game when the lobby is full.</summary>
    private void PlayerConnected(ulong obj)
    {
        playerCount.Value += 1;
        PlayerJoined(HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(obj).userName);
        if(playerCount.Value >= maxPlayerCount.Value) StartTheGame();
    }

    /// <summary>Removes a disconnected player from the count.</summary>
    private void PlayerDisconnected(ulong obj)
    {
        playerCount.Value -= 1;
    }

    /// <summary>Updates the UI when the networked game state changes.</summary>
    private void OnGameStateChanged(GameState previous, GameState current)
    {
        Debug.Log($"Game State: {previous} -> {current}");

        switch (current)
        {
            case GameState.WaitingForPlayers:
                ShowWaitingUI();
                break;

            case GameState.Playing:
                StartGame();
                break;

            case GameState.GameWon:
                ShowGameWonUI();
                break;
            case GameState.GameLost:
                ShowGameLostUI();
                break;
            case GameState.Loading:
                ShowLoadingUI();
                break;
        }
    }

    /// <summary>Shows the waiting-for-players panel.</summary>
    private void ShowWaitingUI()
    {
        UIManager.Instance.ShowCurrentPanel(GameState.WaitingForPlayers);
    }

    /// <summary>Shows the active gameplay panel.</summary>
    private void StartGame()
    {
        UIManager.Instance.ShowCurrentPanel(GameState.Playing);
    }

    /// <summary>Shows the game-won panel.</summary>
    private void ShowGameWonUI()
    {
        UIManager.Instance.ShowCurrentPanel(GameState.GameWon);
    }

    /// <summary>Shows the game-lost panel.</summary>
    private void ShowGameLostUI()
    {
        UIManager.Instance.ShowCurrentPanel(GameState.GameLost);
    }

    /// <summary>Shows the loading panel.</summary>
    private void ShowLoadingUI()
    {
        UIManager.Instance.ShowCurrentPanel(GameState.Loading);
    }

    /// <summary>Notifies the UI that the player count changed.</summary>
    private void PlayerCountChanged(int a, int b)
    {
        playercountChanged?.Invoke();
    }

    /// <summary>Returns the current number of players.</summary>
    public int CurrentPlayerCount()
    {
        return playerCount.Value;
    }

    /// <summary>Returns the lobby's maximum player count.</summary>
    public int MaxPlayerInLobby()
    {
        return maxPlayerCount.Value;
    }

    /// <summary>Begins the short loading phase before gameplay.</summary>
    public void StartTheGame()
    {
        currentGameState.Value = GameState.Loading;
        StartCoroutine(LoadingPanelTimer());
    }

    /// <summary>Waits briefly before switching to the playing state.</summary>
    private IEnumerator LoadingPanelTimer()
    {
        yield return new WaitForSeconds(2);
        currentGameState.Value = GameState.Playing;
    }

    /// <summary>Sets the final game state when the match ends.</summary>
    /// <param name="gameState">Winning or losing state to apply.</param>
    public void GameFinished(GameState gameState)
    {
        if(gameState == GameState.GameWon) currentGameState.Value = GameState.GameWon;
        else if(gameState == GameState.GameLost) currentGameState.Value = GameState.GameLost;
    }

    /// <summary>Adds a player name to the waiting list.</summary>
    /// <param name="name">Name of the joining player.</param>
    public void PlayerJoined(string name)
    {
        playerWaitingList.Add(name);
    }

    /// <summary>Stops the active host or client session.</summary>
    public void FinishTheGame()
    {
        if(NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.Dispose();
        }
        ClientSingleton.Instance.GameManager.Disconnect();
    }
}

/// <summary>States displayed during a multiplayer match.</summary>
public enum GameState
{
    None,
    WaitingForPlayers,
    Playing,
    GameWon,
    GameLost,
    Loading
}
