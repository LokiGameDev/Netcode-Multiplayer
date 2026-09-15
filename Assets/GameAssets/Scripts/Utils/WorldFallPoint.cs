using Unity.Netcode;
using UnityEngine;

/// <summary>Returns players to a safe position after they fall from the world.</summary>
public class WorldFallPoint : NetworkBehaviour
{
    [Tooltip("Position used to respawn fallen players.")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PlayerSpawnManager spawnManager;

    /// <summary>Finds the default world spawn when needed.</summary>
    private void Start()
    {
        if(spawnPoint==null) spawnPoint = GameObject.Find("WorldSpawnPoint").GetComponent<Transform>();
    }

    /// <summary>Moves players entering the fall trigger back to safety.</summary>
    private void OnTriggerEnter(Collider collider)
    {
        if(!IsHost) return;

        if(collider.CompareTag("Player"))
        {
            //spawnManager.SpawnPlayer(collider.GetComponent<PlayerManager>());
        }
    }
}
