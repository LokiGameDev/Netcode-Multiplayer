using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MonsterManager : NetworkBehaviour
{
    [SerializeField] private List<MonsterData> monsterData;

    [Serializable]
    public class MonsterData
    {
        public MonsterType monsterType;
        public GameObject monsterPrefab;
    }

    private static MonsterManager instance;
    public static MonsterManager Instance
    {
        get
        {
            if(instance==null)
            {
                Debug.LogError("Monster manager is null");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if(instance!=null && instance!=this) Destroy(this);

        if(instance==null) instance = this;
    }

    private List<NetworkObject> spawnedMonsters = new List<NetworkObject>();

    public void SpawnMonster(Transform spawnPos, MonsterType monsterType)
    {
        if(!IsHost) return;

        NetworkObject monster = Instantiate(GetMonsterPrefab(monsterType), spawnPos.position, Quaternion.identity).GetComponent<NetworkObject>();

        monster.Spawn();

        spawnedMonsters.Add(monster);
    }

    private GameObject GetMonsterPrefab(MonsterType monsterType)
    {
        foreach(var monster in monsterData)
        {
            if(monster.monsterType == monsterType)
            {
                return monster.monsterPrefab;
            }
        }
        return null;
    }
}

public enum MonsterType
{
    Skeleton,
    Mage
}
