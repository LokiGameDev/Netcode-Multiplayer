using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Database")]
    [SerializeField] private AudioDatabase database;

    [Header("Managers")]
    [SerializeField] private AudioPool audioPool;

    [Header("Music")]
    [SerializeField] private MusicManager musicManager;

    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;

    public static AudioManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        musicManager.Initialize(database,musicSourceA,musicSourceB);

        database.Initialize();
    }

    #region Static API

    public void Play(AudioID id)
    {
        Instance.PlayInternal(id, Vector3.zero, null);
    }

    public void Play(AudioID id, Vector3 position)
    {
        Instance.PlayInternal(id, position, null);
    }

    public void Play(AudioID id, Transform target)
    {
        Instance.PlayInternal(id, Vector3.zero, target);
    }

    public void PlayMusic(AudioID id)
    {
        Instance.musicManager.Play(id);
    }

    public void StopMusic()
    {
        Instance.musicManager.Stop();
    }

    #endregion

    private void PlayInternal(AudioID id, Vector3 position, Transform target)
    {
        AudioDefinition definition = database.Get(id);

        if (definition == null)
        {
            Debug.LogWarning($"Audio '{id}' not found.");
            return;
        }

        AudioPlayer player = audioPool.Get();

        if (player == null)
            return;

        if (target != null)
            player.Play(definition, target);
        else
            player.Play(definition, position);
    }
}