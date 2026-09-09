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

/// <summary>Initializes Unity services and manages the client connection.</summary>
public class ClientGameManager : IDisposable
{

    private JoinAllocation allocation;
    private NetworkClient networkClient;

    /// <summary>Initializes services and authenticates the local player.</summary>
    /// <returns>True when authentication succeeds.</returns>
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);

        AuthState authState = await AuthenticatorWrapper.DoAuthorize(5);

        if(authState == AuthState.Authenticated) return true;

        return false;
    }

    /// <summary>Loads the next scene, which contains the main menu.</summary>
    public void GoToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1, LoadSceneMode.Single);
    }

    /// <summary>Connects the client to a Relay allocation using a join code.</summary>
    /// <param name="joinCode">Relay code for the host session.</param>
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
            userAuthId = AuthenticationService.Instance.PlayerId,
            skinID = PlayerPrefs.GetString("PlayerSkinID", "Default")
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        QualityManager.Instance?.SetGameFPS(CurrentGameScene.Game);
        
        NetworkManager.Singleton.StartClient();
    }

    /// <summary>Disconnects the network client if it is active.</summary>
    public void Disconnect()
    {
        networkClient?.Disconnect();
    }

    /// <summary>Releases the client network resources.</summary>
    public void Dispose()
    {
        networkClient?.Dispose();
    }
}
