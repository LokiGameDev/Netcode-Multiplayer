using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WaitingPanel : NetworkBehaviour
{
    [SerializeField] private TMP_Text playersCountText;
    [SerializeField] private Button startButton;

    [SerializeField] private Transform playerJoinedListContainer;
    [SerializeField] private GameObject playerWaitingPrefab;

    [SerializeField] private GameObject invitePlayersButton;

    private List<string> playerWaitingList = new List<string>();

    [SerializeField] private int requiredPlayers = 2;

    public void OnEnable()
    {
        playersCountText.text = GameStateManager.Instance.CurrentPlayerCount() + "/" + GameStateManager.Instance.MaxPlayerInLobby() + " Players Joined";
        GameStateManager.Instance.playercountChanged.AddListener(PlayerCountChanged);
        GameStateManager.Instance.playerWaitingList.OnListChanged += ChangePlayerList;

        invitePlayersButton.SetActive(IsServer);
        startButton.interactable = false;
        startButton.gameObject.SetActive(false);
    }

    public void OnDisable()
    {
        GameStateManager.Instance.playercountChanged.RemoveListener(PlayerCountChanged);
        GameStateManager.Instance.playerWaitingList.OnListChanged -= ChangePlayerList;
    }

    public void Start()
    {
        AddPlayer();
        PlayerCountChanged();
        invitePlayersButton.SetActive(IsHost);
    }

    public void PlayerCountChanged()
    {
        playersCountText.text = GameStateManager.Instance.CurrentPlayerCount() + "/" + GameStateManager.Instance.MaxPlayerInLobby() + " Players Joined";

        invitePlayersButton.SetActive(IsHost);

        if(IsHost) CheckForMinimumPlayers();
    }

    private void CheckForMinimumPlayers()
    {
        startButton.interactable = GameStateManager.Instance.CurrentPlayerCount() >= requiredPlayers;
        startButton.gameObject.SetActive(GameStateManager.Instance.CurrentPlayerCount() >= requiredPlayers);
    }

    public void StartTheGame()
    {
        GameStateManager.Instance.StartTheGame();
    }

    private void ChangePlayerList(NetworkListEvent<FixedString64Bytes> changeEvent)
    {
        AddPlayer();
    }

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
