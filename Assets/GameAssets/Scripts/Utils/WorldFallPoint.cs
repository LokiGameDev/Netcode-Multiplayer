using UnityEngine;

/// <summary>Returns players to a safe position after they fall from the world.</summary>
public class WorldFallPoint : MonoBehaviour
{
    [Tooltip("Position used to respawn fallen players.")]
    [SerializeField] private Transform spawnPoint;

    /// <summary>Finds the default world spawn when needed.</summary>
    private void Start()
    {
        if(spawnPoint==null) spawnPoint = GameObject.Find("WorldSpawnPoint").GetComponent<Transform>();
    }

    /// <summary>Moves players entering the fall trigger back to safety.</summary>
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            collider.gameObject.GetComponent<PlayerMovement>().ResetVelocity();
            if(spawnPoint==null) collider.gameObject.transform.position = new Vector3(0,0,0);
            else collider.gameObject.transform.position = spawnPoint.position + new Vector3(0,10,0);
        }
    }
}
