using UnityEngine;

/// <summary>Controls background music playback and track transitions.</summary>
public class MusicManager : MonoBehaviour
{
    private AudioSource musicSourceA;
    private AudioSource musicSourceB;

    private AudioSource activeSource;
    private AudioSource inactiveSource;

    private AudioDatabase database;

    /// <summary>Initializes the music sources and audio database.</summary>
    public void Initialize(AudioDatabase database,
                        AudioSource sourceA,
                        AudioSource sourceB)
    {
        this.database = database;

        musicSourceA = sourceA;
        musicSourceB = sourceB;

        activeSource = musicSourceA;
        inactiveSource = musicSourceB;
    }

    public bool isStartedBackground = false;

    /// <summary>Starts the next track when background music finishes.</summary>
    private void Update()
    {
        if (isStartedBackground && !activeSource.isPlaying)
        {
            PlayNext();
        }
    }

    /// <summary>Plays the requested music definition.</summary>
    public void Play(AudioID id)
    {
        AudioDefinition definition = database.Get(id);

        if (definition == null)
            return;

        AudioClip clip = definition.GetClip();

        if (clip == null)
            return;

        // Already playing
        if (activeSource.clip == clip && activeSource.isPlaying)
            return;

        activeSource.clip = clip;
        activeSource.volume = definition.Volume;
        activeSource.loop = definition.Loop;
        activeSource.outputAudioMixerGroup = definition.MixerGroup;

        activeSource.Play();
        Debug.Log("Plying:" + clip.name);

        isStartedBackground=true;
    }

    /// <summary>Stops the active music source.</summary>
    public void Stop()
    {
        activeSource.Stop();
    }

    /// <summary>Pauses the active music source.</summary>
    public void Pause()
    {
        activeSource.Pause();
    }

    /// <summary>Resumes the active music source.</summary>
    public void Resume()
    {
        activeSource.UnPause();
    }

    /// <summary>Plays the next clip from the music definition.</summary>
    private void PlayNext()
    {
        AudioDefinition definition = database.Get(AudioID.Music);

        if (definition == null)
            return;

        AudioClip clip = definition.GetClip();

        if (clip == null)
            return;

        // Already playing
        if (activeSource.clip == clip && activeSource.isPlaying)
            return;

        activeSource.clip = clip;
        Debug.Log("Plying:" + clip.name);

        activeSource.Play();
    }

    /// <summary>Gets the clip currently assigned to the active source.</summary>
    public AudioClip CurrentClip => activeSource.clip;
}