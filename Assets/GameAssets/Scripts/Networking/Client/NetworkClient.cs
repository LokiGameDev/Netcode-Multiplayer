using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Handles client connection callbacks and graceful disconnection.</summary>
public class NetworkClient : IDisposable
{
    private const string MenuSceneName = "Menu";
    private NetworkManager _networkManager;

    /// <summary>Creates a network client wrapper for a network manager.</summary>
    public NetworkClient(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        networkManager.OnClientDisconnectCallback += OnClientDisconnected;
    }

    /// <summary>Returns to the menu when the local client disconnects.</summary>
    private void OnClientDisconnected(ulong clientID)
    {
        if(clientID !=0 && clientID != _networkManager.LocalClientId) return;

        Disconnect();
    }

    /// <summary>Unsubscribes from network manager callbacks.</summary>
    public void Dispose()
    {
        if(_networkManager!=null)
        {
            _networkManager.OnClientConnectedCallback -= OnClientDisconnected;
        }
    }

    /// <summary>Stops the network client and returns to the menu scene.</summary>
    public void Disconnect()
    {
        if(SceneManager.GetActiveScene().name != MenuSceneName)
        {
            QualityManager.Instance.SetGameFPS(CurrentGameScene.Menu);

            SceneManager.LoadScene(MenuSceneName);
        }

        if(_networkManager.IsConnectedClient)
        {
            _networkManager?.Shutdown();
        }
    }
}
