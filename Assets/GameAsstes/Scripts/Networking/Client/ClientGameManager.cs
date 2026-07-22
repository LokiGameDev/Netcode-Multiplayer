using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;

public class ClientGameManager : IDisposable
{

    private JoinAllocation allocation;
    private NetworkClient networkClient;

    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);

        AuthState authState = await AuthenticatorWrapper.DoAuthorize(5);

        if(authState == AuthState.Authenticated) return true;

        return false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1, LoadSceneMode.Single);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }

        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        unityTransport.SetRelayServerData(relayServerData);

        UserData userData = new UserData
        {
            userName = PlayerPrefs.GetString("PlayerName", "PlayerName"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();
    }

    public void Disconnect()
    {
        networkClient?.Disconnect();
    }

    public void Dispose()
    {
        networkClient?.Dispose();
    }
}
