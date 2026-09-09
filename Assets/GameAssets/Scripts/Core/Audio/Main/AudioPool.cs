using System.Collections.Generic;
using UnityEngine;

/// <summary>Maintains a reusable pool of audio players.</summary>
public class AudioPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [Tooltip("Prefab used to create pooled audio players.")]
    [SerializeField] private AudioPlayer playerPrefab;
    [Tooltip("Number of audio players created when the pool starts.")]
    [SerializeField] private int initialPoolSize = 20;
    [Tooltip("Whether the pool may create players after its initial capacity is used.")]
    [SerializeField] private bool expandable = true;

    private readonly Queue<AudioPlayer> availablePlayers = new();
    private readonly List<AudioPlayer> allPlayers = new();

    /// <summary>Creates the initial pool contents.</summary>
    private void Awake()
    {
        CreatePool();
    }

    /// <summary>Creates the configured number of pooled players.</summary>
    private void CreatePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreatePlayer();
        }
    }

    /// <summary>Creates and optionally queues one pooled audio player.</summary>
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

    /// <summary>Gets an available player or expands the pool when allowed.</summary>
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

    /// <summary>Deactivates a player and returns it to the available queue.</summary>
    public void Return(AudioPlayer player)
    {
        player.gameObject.SetActive(false);

        availablePlayers.Enqueue(player);
    }
}