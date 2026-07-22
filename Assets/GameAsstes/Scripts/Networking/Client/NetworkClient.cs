using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : MonoBehaviour
{
    private const string MenuSceneName = "Menu";
    private NetworkManager _networkManager;

    public NetworkClient(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        networkManager.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientDisconnected(ulong clientID)
    {
        if(clientID !=0 && clientID != _networkManager.LocalClientId) return;

        Disconnect();
    }

    public void Dispose()
    {
        if(_networkManager!=null)
        {
            _networkManager.OnClientConnectedCallback -= OnClientDisconnected;
        }
    }

    public void Disconnect()
    {
        if(SceneManager.GetActiveScene().name != MenuSceneName)
        {
            SceneManager.LoadScene(MenuSceneName);
        }

        if(_networkManager.IsConnectedClient)
        {
            _networkManager?.Shutdown();
        }
    }
}
