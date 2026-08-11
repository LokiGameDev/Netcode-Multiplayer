using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;
    public Action<string> OnClientLeft;
    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authToUserDate = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += NetworkStarted;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
        authToUserDate[userData.userAuthId] = userData;

        response.Approved = true;
        response.Position = new Vector3(0,0,0);
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true;
    }

    private void NetworkStarted()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if(clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            clientIdToAuth.Remove(clientId);
            authToUserDate.Remove(authId);
            OnClientLeft?.Invoke(authId);
        }
    }

    public void Dispose()
    {
        if(networkManager!=null)
        {
            networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            networkManager.OnServerStarted -= NetworkStarted;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }

        if(networkManager.IsListening)
        {
            networkManager.Shutdown();
        }
    }
}
