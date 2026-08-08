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

public class HostGameManager : IDisposable
{
    [SerializeField] private int MaxConnections = 20;
    private string joinCode;
    private string lobbyId;
    private string GameSceneName = "Main";
    private Allocation allocation;
    public NetworkServer NetworkServer { get; private set; }

    public async Task StartHostAsync(LobbySettings lobbySettings = null)
    {
        if(lobbySettings!=null) MaxConnections = lobbySettings.NumberOfPlayers;
        else MaxConnections = 20;
        
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
                lobbyOptions.IsPrivate = !lobbySettings.PublicLobby;
            }
            else
            {
                lobbyName = PlayerPrefs.GetString("PlayerName", "DefaultHost");
                lobbyOptions.IsPrivate = false;
            }

            lobbyOptions.Data = new Dictionary<string, DataObject>
            {
                {
                    "JoinCode", new DataObject(DataObject.VisibilityOptions.Member, value: joinCode)
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName+"'s Lobby", MaxConnections, lobbyOptions);

            Debug.Log($"{lobby.IsPrivate}");

            lobbyId = lobby.Id;
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
            userAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        NetworkServer.OnClientLeft += HandleClientLeft;

        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    private IEnumerator HeartBeatLobby(float time)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(time);
        while(true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private async void HandleClientLeft(string authId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, authId);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }
    }

    public async void UpdateLobbyOptions(bool state)
    {
        UpdateLobbyOptions updateOptions = new UpdateLobbyOptions
        {
            IsLocked = state
        };

        await LobbyService.Instance.UpdateLobbyAsync(lobbyId, updateOptions);
    }

    public void Dispose()
    {
        Shutdown();
    }

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
}
