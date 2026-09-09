using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>Approves clients and tracks their authenticated user data.</summary>
public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;
    public Action<string> OnClientLeft;
    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authToUserDate = new Dictionary<string, UserData>();

    /// <summary>Subscribes to the network manager's server callbacks.</summary>
    /// <param name="networkManager">Network manager owned by the server.</param>
    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += NetworkStarted;
    }

    /// <summary>Validates a connection payload and approves the player object.</summary>
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

    /// <summary>Subscribes to client disconnect notifications after startup.</summary>
    private void NetworkStarted()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    /// <summary>Removes a disconnected client and notifies the host.</summary>
    /// <param name="clientId">Network ID of the disconnected client.</param>
    private void OnClientDisconnect(ulong clientId)
    {
        if(clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            clientIdToAuth.Remove(clientId);
            authToUserDate.Remove(authId);
            OnClientLeft?.Invoke(authId);
        }
    }

    /// <summary>Gets the user data associated with a network client.</summary>
    /// <param name="clientId">Network ID of the client.</param>
    /// <returns>The matching user data, or null when no entry exists.</returns>
    public UserData GetUserDataByClientId(ulong clientId)
    {
        if(clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            if(authToUserDate.TryGetValue(authId, out UserData data))
            {
                return data;
            }
        }
        return null;
    }

    /// <summary>Unsubscribes callbacks and shuts down the network manager.</summary>
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
