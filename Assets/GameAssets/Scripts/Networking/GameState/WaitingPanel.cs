using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Displays waiting players and starts the game when enough have joined.</summary>
public class WaitingPanel : NetworkBehaviour
{
    [Tooltip("Text displaying the number of joined players.")]
    [SerializeField] private TMP_Text playersCountText;
    [Tooltip("Button used by the host to start the game.")]
    [SerializeField] private Button startButton;

    [Tooltip("Parent transform for waiting player entries.")]
    [SerializeField] private Transform playerJoinedListContainer;
    [Tooltip("Prefab used for a waiting player entry.")]
    [SerializeField] private GameObject playerWaitingPrefab;

    [Tooltip("Button used to invite additional players.")]
    [SerializeField] private GameObject invitePlayersButton;

    private List<string> playerWaitingList = new List<string>();

    [Tooltip("Minimum number of players required to start.")]
    [SerializeField] private int requiredPlayers = 2;

    /// <summary>Subscribes to player-count and waiting-list updates.</summary>
    public void OnEnable()
    {
        playersCountText.text = GameStateManager.Instance.CurrentPlayerCount() + "/" + GameStateManager.Instance.MaxPlayerInLobby() + " Players Joined";
        GameStateManager.Instance.playercountChanged.AddListener(PlayerCountChanged);
        GameStateManager.Instance.playerWaitingList.OnListChanged += ChangePlayerList;

        invitePlayersButton.SetActive(IsServer);
        startButton.interactable = false;
        startButton.gameObject.SetActive(false);
    }

    /// <summary>Unsubscribes from player-count and waiting-list updates.</summary>
    public void OnDisable()
    {
        GameStateManager.Instance.playercountChanged.RemoveListener(PlayerCountChanged);
        GameStateManager.Instance.playerWaitingList.OnListChanged -= ChangePlayerList;
    }

    /// <summary>Initializes the waiting panel for the current network role.</summary>
    public void Start()
    {
        AddPlayer();
        PlayerCountChanged();
        invitePlayersButton.SetActive(IsHost);
    }

    /// <summary>Refreshes the player count and host start controls.</summary>
    public void PlayerCountChanged()
    {
        playersCountText.text = GameStateManager.Instance.CurrentPlayerCount() + "/" + GameStateManager.Instance.MaxPlayerInLobby() + " Players Joined";

        invitePlayersButton.SetActive(IsHost);

        if(IsHost) CheckForMinimumPlayers();
    }

    /// <summary>Updates start-button availability based on the player count.</summary>
    private void CheckForMinimumPlayers()
    {
        startButton.interactable = GameStateManager.Instance.CurrentPlayerCount() >= requiredPlayers;
        startButton.gameObject.SetActive(GameStateManager.Instance.CurrentPlayerCount() >= requiredPlayers);
    }

    /// <summary>Requests that the game state begin the match.</summary>
    public void StartTheGame()
    {
        GameStateManager.Instance.StartTheGame();
    }

    /// <summary>Refreshes the waiting-player display after a list change.</summary>
    private void ChangePlayerList(NetworkListEvent<FixedString64Bytes> changeEvent)
    {
        AddPlayer();
    }

    /// <summary>Adds newly joined players to the waiting list UI.</summary>
    public void AddPlayer()
    {
        var names = GameStateManager.Instance.playerWaitingList;

        Debug.Log("Player Count: " + names.Count);

        foreach(var name in names)
        {
            string playerName = name.ToString();

            Debug.Log("Checking " + playerName);

            if(playerWaitingList.Contains(playerName)) continue;

            playerWaitingList.Add(playerName);
            
            var item = Instantiate(playerWaitingPrefab, playerJoinedListContainer);

            Debug.Log("Adding: " + name +" to the list");

            item.GetComponent<PlayerWaitingItem>().SetPlayerName(playerName);
        }
    }
}
