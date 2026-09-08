using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

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

    public override void OnNetworkDespawn()
    {
        if(!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= PlayerConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= PlayerDisconnected;
    }

    public void Start()
    {
        OnGameStateChanged(currentGameState.Value, currentGameState.Value);
    }

    private void PlayerConnected(ulong obj)
    {
        playerCount.Value += 1;
        PlayerJoined(HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(obj).userName);
        if(playerCount.Value >= maxPlayerCount.Value) StartTheGame();
    }

    private void PlayerDisconnected(ulong obj)
    {
        playerCount.Value -= 1;
    }

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

    private void ShowWaitingUI()
    {
        UIManager.Instance.ShowCurrentPanel(GameState.WaitingForPlayers);
    }

    private void StartGame()
    {
        UIManager.Instance.ShowCurrentPanel(GameState.Playing);
    }

    private void ShowGameWonUI()
    {
        UIManager.Instance.ShowCurrentPanel(GameState.GameWon);
    }

    private void ShowGameLostUI()
    {
        UIManager.Instance.ShowCurrentPanel(GameState.GameLost);
    }

    private void ShowLoadingUI()
    {
        UIManager.Instance.ShowCurrentPanel(GameState.Loading);
    }

    private void PlayerCountChanged(int a, int b)
    {
        playercountChanged?.Invoke();
    }

    public int CurrentPlayerCount()
    {
        return playerCount.Value;
    }

    public int MaxPlayerInLobby()
    {
        return maxPlayerCount.Value;
    }

    public void StartTheGame()
    {
        currentGameState.Value = GameState.Loading;
        StartCoroutine(LoadingPanelTimer());
    }

    private IEnumerator LoadingPanelTimer()
    {
        yield return new WaitForSeconds(2);
        currentGameState.Value = GameState.Playing;
    }

    public void GameFinished(GameState gameState)
    {
        if(gameState == GameState.GameWon) currentGameState.Value = GameState.GameWon;
        else if(gameState == GameState.GameLost) currentGameState.Value = GameState.GameLost;
    }

    public void PlayerJoined(string name)
    {
        playerWaitingList.Add(name);
    }

    public void FinishTheGame()
    {
        if(NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.Dispose();
        }
        ClientSingleton.Instance.GameManager.Disconnect();
    }
}

public enum GameState
{
    None,
    WaitingForPlayers,
    Playing,
    GameWon,
    GameLost,
    Loading
}
