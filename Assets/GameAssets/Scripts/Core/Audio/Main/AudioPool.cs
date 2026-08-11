using System.Collections.Generic;
using UnityEngine;

public class AudioPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private AudioPlayer playerPrefab;
    [SerializeField] private int initialPoolSize = 20;
    [SerializeField] private bool expandable = true;

    private readonly Queue<AudioPlayer> availablePlayers = new();
    private readonly List<AudioPlayer> allPlayers = new();

    private void Awake()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreatePlayer();
        }
    }

    private AudioPlayer CreatePlayer(bool enqueue = true)
    {
        AudioPlayer player = Instantiate(playerPrefab, transform);

        player.Initialize(this);

        player.gameObject.SetActive(false);

        allPlayers.Add(player);

        if (enqueue)
            availablePlayers.Enqueue(player);

        return player;
    }

    public AudioPlayer Get()
    {
        if (availablePlayers.Count > 0)
        {
            AudioPlayer player = availablePlayers.Dequeue();

            player.gameObject.SetActive(true);

            return player;
        }

        if (!expandable)
        {
            Debug.LogWarning("AudioPool is exhausted!");

            return null;
        }
        if (expandable)
        {
            AudioPlayer player = CreatePlayer(false);

            player.gameObject.SetActive(true);

            return player;
        }

        AudioPlayer newPlayer = CreatePlayer();

        newPlayer.gameObject.SetActive(true);

        availablePlayers.Dequeue();

        return newPlayer;
    }

    public void Return(AudioPlayer player)
    {
        player.gameObject.SetActive(false);

        availablePlayers.Enqueue(player);
    }
}