using UnityEngine;
using UnityEngine.Audio;

/// <summary>Provides the central API for music and sound-effect playback.</summary>
public class AudioManager : MonoBehaviour
{

    [Header("Database")]
    [Tooltip("Database containing available audio definitions.")]
    [SerializeField] private AudioDatabase database;

    [Header("Managers")]
    [Tooltip("Pool that provides reusable audio players.")]
    [SerializeField] private AudioPool audioPool;
    [Tooltip("Applies music and sound-effect volume settings.")]
    [SerializeField] private AudioSettingsUpdater audioSettingsUpdater;

    [Header("Music")]
    [Tooltip("Controls music playback and track transitions.")]
    [SerializeField] private MusicManager musicManager;

    [Tooltip("First audio source used for music playback.")]
    [SerializeField] private AudioSource musicSourceA;
    [Tooltip("Second audio source used for music playback.")]
    [SerializeField] private AudioSource musicSourceB;

    /// <summary>Gets the active audio manager instance.</summary>
    public static AudioManager Instance { get; private set; }
    /// <summary>Initializes the singleton and audio services.</summary>
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

    /// <summary>Plays an audio definition at the origin.</summary>
    public void Play(AudioID id)
    {
        Instance.PlayInternal(id, Vector3.zero, null);
    }

    /// <summary>Plays an audio definition at a world position.</summary>
    public void Play(AudioID id, Vector3 position)
    {
        Instance.PlayInternal(id, position, null);
    }

    /// <summary>Plays an audio definition attached to a target.</summary>
    public void Play(AudioID id, Transform target)
    {
        Instance.PlayInternal(id, Vector3.zero, target);
    }

    /// <summary>Starts the requested music track.</summary>
    public void PlayMusic(AudioID id)
    {
        Instance.musicManager.Play(id);
    }

    /// <summary>Stops the current music track.</summary>
    public void StopMusic()
    {
        Instance.musicManager.Stop();
    }

    /// <summary>Sets the music volume.</summary>
    public void SetMusicVolume(float value)
    {
        audioSettingsUpdater.SetMusicVolume(value);
    }

    /// <summary>Sets the sound-effect volume.</summary>
    public void SetSFXVolume(float value)
    {
        audioSettingsUpdater.SetSFXVolume(value);
    }

    /// <summary>Returns the current music volume.</summary>
    public float GetMusicVolume()
    {
        return audioSettingsUpdater.GetMusicVolume();
    }

    /// <summary>Returns the current sound-effect volume.</summary>
    public float GetSFXVolume()
    {
        return audioSettingsUpdater.GetSFXVolume();
    }

    #endregion

    /// <summary>Resolves a definition and routes it to an audio player.</summary>
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