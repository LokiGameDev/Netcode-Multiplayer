using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

/// <summary>Displays one lobby and provides its join action.</summary>
public class LobbyItem : MonoBehaviour
{
    [Header("Lobby Display")]
    [Tooltip("Text displaying the lobby name.")]
    [SerializeField] private TMP_Text lobbyNameText;
    [Tooltip("Text displaying the lobby player count.")]
    [SerializeField] private TMP_Text lobbyPlayerCountText;

    private LobbyList lobbyList;
    private Lobby lobby;

    /// <summary>Initializes this item with a lobby and its owning list.</summary>
    /// <param name="lobbyList">List that handles joining the lobby.</param>
    /// <param name="lobby">Lobby represented by this item.</param>
    public void Initialize(LobbyList lobbyList, Lobby lobby)
    {
        this.lobby = lobby;
        this.lobbyList = lobbyList;
        lobbyNameText.text = lobby.Name;
        lobbyPlayerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    /// <summary>Requests to join the represented lobby.</summary>
    public void Join()
    {
        lobbyList.JoinAsync(lobby);
        lobbyList.LoadingPanelStart();
    }
}
