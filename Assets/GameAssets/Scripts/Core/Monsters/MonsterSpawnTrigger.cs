using UnityEngine;

public class MonsterSpawnTrigger : MonoBehaviour
{
    [SerializeField] private MonsterType monsterType;
    [SerializeField] private Transform spawnPoint;

    private bool isSpawned = false;

    public void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player") && !isSpawned)
        {
            MonsterManager.Instance.SpawnMonster(spawnPoint, monsterType);
            isSpawned = true;
            gameObject.SetActive(false);
        }
    }
}
