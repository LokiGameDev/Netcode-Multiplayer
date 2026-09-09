using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Creates and manages the host Relay and Lobby session.</summary>
public class HostGameManager : IDisposable
{
    [Tooltip("Maximum number of players accepted by Relay.")]
    [SerializeField] private int MaxConnections = 8;
    private string joinCode;
    private string lobbyId;
    private string GameSceneName = "Game1";
    public string hostName { get; private set; }
    public string currentLobbyName { get; private set; }
    private Allocation allocation;
    public NetworkServer NetworkServer { get; private set; }

    public Lobby currentLobby { get; private set; }

    /// <summary>Creates a Relay allocation, Lobby, and network host.</summary>
    /// <param name="lobbySettings">Optional settings for the new lobby.</param>
    public async Task StartHostAsync(LobbySettings lobbySettings = null)
    {
        if(lobbySettings!=null) MaxConnections = lobbySettings.NumberOfPlayers;
        else MaxConnections = 8;
        
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }

        try
        {
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }

        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        unityTransport.SetRelayServerData(relayServerData);

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();

            string lobbyName = "";

            if(lobbySettings!=null)
            {
                lobbyName = lobbySettings.LobbyName;
                currentLobbyName = lobbyName;
                lobbyOptions.IsPrivate = !lobbySettings.PublicLobby;
            }
            else
            {
                lobbyName = PlayerPrefs.GetString("PlayerName", "DefaultHost");
                currentLobbyName = lobbyName;
                lobbyOptions.IsPrivate = false;
            }

            lobbyOptions.Data = new Dictionary<string, DataObject>
            {
                {
                    "JoinCode", new DataObject(DataObject.VisibilityOptions.Member, value: joinCode)
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName+"'s Lobby", MaxConnections, lobbyOptions);
            
            currentLobby = lobby;

            Debug.Log($"{lobby.IsPrivate}");

            lobbyId = lobby.Id;

            hostName = PlayerPrefs.GetString("PlayerName", "Player");

            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15));
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }

        NetworkServer = new NetworkServer(NetworkManager.Singleton);

        UserData userData = new UserData
        {
            userName = PlayerPrefs.GetString("PlayerName", "DefaultName"),
            userAuthId = AuthenticationService.Instance.PlayerId,
            skinID = PlayerPrefs.GetString("PlayerSkinID", "Default")
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        NetworkServer.OnClientLeft += HandleClientLeft;

        QualityManager.Instance?.SetGameFPS(CurrentGameScene.Game);

        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    /// <summary>Keeps the host lobby active while the session is running.</summary>
    /// <param name="time">Seconds between heartbeat requests.</param>
    private IEnumerator HeartBeatLobby(float time)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(time);
        while(true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    /// <summary>Removes a disconnected player from the host lobby.</summary>
    /// <param name="authId">Authentication ID of the disconnected player.</param>
    private async void HandleClientLeft(string authId)
    {
        try
        {
            await LobbyService.Instance?.RemovePlayerAsync(lobbyId, authId);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }
    }

    /// <summary>Updates whether the lobby accepts new players.</summary>
    /// <param name="state">True to lock the lobby.</param>
    public async void UpdateLobbyOptions(bool state)
    {
        UpdateLobbyOptions updateOptions = new UpdateLobbyOptions
        {
            IsLocked = state
        };

        await LobbyService.Instance.UpdateLobbyAsync(lobbyId, updateOptions);
    }

    /// <summary>Shuts down the host and releases its resources.</summary>
    public void Dispose()
    {
        Shutdown();
    }

    /// <summary>Deletes the lobby and stops the server connection.</summary>
    public async void Shutdown()
    {
        HostSingleton.Instance?.StopCoroutine(nameof(HeartBeatLobby));
        if(!string.IsNullOrEmpty(lobbyId))
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                return;
            }
            lobbyId = string.Empty;
        }
        NetworkServer.OnClientLeft -= HandleClientLeft;
        NetworkServer?.Dispose();
    }

    /// <summary>Returns the Relay join code for this host session.</summary>
    /// <returns>The current Relay join code.</returns>
    public string GetJoinCode()
    {
        return joinCode;
    }
}
