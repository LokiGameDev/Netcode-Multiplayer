using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyPlayerCountText;

    private LobbyList lobbyList;
    private Lobby lobby;

    public void Initialize(LobbyList lobbyList, Lobby lobby)
    {
        this.lobby = lobby;
        this.lobbyList = lobbyList;
        lobbyNameText.text = lobby.Name;
        lobbyPlayerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void Join()
    {
        lobbyList.JoinAsync(lobby);
    }
}
