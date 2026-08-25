using System;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    [SerializeField] private Transform lobbyItemsParent;
    [SerializeField] private LobbyItem lobbyItemPrefab;
    [SerializeField] private GameObject noLobbyFoundText;
    [SerializeField] private GameObject loadingPanel;

    private bool IsRefreshing = false;
    private bool IsJoining = false;

    private void OnEnable()
    {
        RefreshList();
        loadingPanel.SetActive(false);
    }

    public void Refresh()
    {
        RefreshList();
    }

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

    private void NoLobbyFoundState(bool state)
    {
        noLobbyFoundText.SetActive(state);
    }

    public void LoadingPanelStart()
    {
        loadingPanel.SetActive(true);
    }
}
