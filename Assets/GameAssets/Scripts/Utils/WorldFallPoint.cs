using UnityEngine;

public class WorldFallPoint : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    private void Start()
    {
        if(spawnPoint==null) spawnPoint = GameObject.Find("WorldSpawnPoint").GetComponent<Transform>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            if(spawnPoint==null) collider.gameObject.transform.position = new Vector3(0,0,0);
            else collider.gameObject.transform.position = spawnPoint.position + new Vector3(0,10,0);
        }
    }
}
