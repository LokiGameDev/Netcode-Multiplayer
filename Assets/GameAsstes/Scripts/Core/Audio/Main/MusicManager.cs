using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource musicSourceA;
    private AudioSource musicSourceB;

    private AudioSource activeSource;
    private AudioSource inactiveSource;

    private AudioDatabase database;

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

    private void Update()
    {
        if (isStartedBackground && !activeSource.isPlaying)
        {
            PlayNext();
        }
    }

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

    public void Stop()
    {
        activeSource.Stop();
    }

    public void Pause()
    {
        activeSource.Pause();
    }

    public void Resume()
    {
        activeSource.UnPause();
    }

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

    public AudioClip CurrentClip => activeSource.clip;
}