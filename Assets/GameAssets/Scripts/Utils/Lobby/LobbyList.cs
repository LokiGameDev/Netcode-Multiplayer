using System;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

/// <summary>Queries available lobbies and creates their list items.</summary>
public class LobbyList : MonoBehaviour
{
    [Header("Lobby List")]
    [Tooltip("Parent transform for lobby items.")]
    [SerializeField] private Transform lobbyItemsParent;
    [Tooltip("Prefab used for each lobby item.")]
    [SerializeField] private LobbyItem lobbyItemPrefab;
    [Tooltip("Object shown when no lobbies are found.")]
    [SerializeField] private GameObject noLobbyFoundText;
    [Tooltip("Panel shown while joining a lobby.")]
    [SerializeField] private GameObject loadingPanel;

    private bool IsRefreshing = false;
    private bool IsJoining = false;

    /// <summary>Refreshes the list when the lobby panel is enabled.</summary>
    private void OnEnable()
    {
        RefreshList();
        loadingPanel.SetActive(false);
    }

    /// <summary>Refreshes the visible lobby list.</summary>
    public void Refresh()
    {
        RefreshList();
    }

    /// <summary>Queries unlocked lobbies and rebuilds their UI items.</summary>
    public async void RefreshList()
    {
        if(IsRefreshing) return;

        IsRefreshing = true;

        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;
            options.Filters = new System.Collections.Generic.List<QueryFilter>
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"
                ),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0"
                ),
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            NoLobbyFoundState(lobbies.Results.Count <= 0);

            foreach(Transform child in lobbyItemsParent)
            {
                Destroy(child.gameObject);
            }
            foreach(Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemsParent);
                lobbyItem.Initialize(this, lobby);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        IsRefreshing = false;
    }

    /// <summary>Joins a lobby and starts its client connection.</summary>
    /// <param name="lobby">Lobby to join.</param>
    public async void JoinAsync(Lobby lobby)
    {
        if(IsJoining) return;

        IsJoining = true;

        try
        {
            Lobby joiningLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }

        IsJoining = false;
    }

    /// <summary>Updates the empty-list message visibility.</summary>
    /// <param name="state">True when no lobby is available.</param>
    private void NoLobbyFoundState(bool state)
    {
        noLobbyFoundText.SetActive(state);
    }

    /// <summary>Shows the loading panel while a join is in progress.</summary>
    public void LoadingPanelStart()
    {
        loadingPanel.SetActive(true);
    }
}
