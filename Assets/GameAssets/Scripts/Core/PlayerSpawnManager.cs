using Unity.Netcode;
using UnityEngine;

public class PlayerSpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private TaskManager taskManager;

    private int spawnIndex;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsHost)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayer(clientId);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"CLIENT CONNECTED: {clientId}");

        if (!IsHost)
            return;

        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        Debug.Log($"SPAWNING PLAYER FOR: {clientId}");

        Transform spawnPoint = spawnPoints[spawnIndex];

        GameObject player = Instantiate(
            playerPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        NetworkObject networkObject =
            player.GetComponent<NetworkObject>();

        networkObject.SpawnAsPlayerObject(clientId);

        spawnIndex++;

        if (spawnIndex >= spawnPoints.Length)
            spawnIndex = 0;

        taskManager.PlayerConnected(clientId);
    }
}
