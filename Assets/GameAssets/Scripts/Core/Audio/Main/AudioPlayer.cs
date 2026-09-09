using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
/// <summary>Plays one pooled audio definition in the scene.</summary>
public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private Transform followTarget;
    private Vector3 worldPosition;

    private AudioPool ownerPool;

    private bool isPlaying;

    /// <summary>Gets whether this player is currently playing audio.</summary>
    public bool IsPlaying => isPlaying;

    /// <summary>Caches the required audio source.</summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>Associates this player with its owning pool.</summary>
    public void Initialize(AudioPool pool)
    {
        ownerPool = pool;
    }

    /// <summary>Starts playback at a fixed world position.</summary>
    public void Play(AudioDefinition definition, Vector3 position)
    {
        followTarget = null;
        worldPosition = position;

        transform.position = position;

        SetupSource(definition);

        audioSource.clip = definition.GetClip();
        audioSource.Play();

        isPlaying = true;
    }

    /// <summary>Starts playback while following a target transform.</summary>
    public void Play(AudioDefinition definition, Transform target)
    {
        followTarget = target;

        if (target != null)
            transform.position = target.position;

        SetupSource(definition);

        audioSource.clip = definition.GetClip();
        audioSource.Play();

        isPlaying = true;
    }

    /// <summary>Stops playback and returns this player to its pool.</summary>
    public void Stop()
    {
        audioSource.Stop();

        isPlaying = false;

        followTarget = null;

        ownerPool.Return(this);
    }

    /// <summary>Follows the target and returns completed non-looping players.</summary>
    private void LateUpdate()
    {
        if (followTarget != null)
        {
            transform.position = followTarget.position;
        }

        if (isPlaying &&
            !audioSource.isPlaying &&
            !audioSource.loop)
        {
            Stop();
        }
    }

    /// <summary>Applies definition settings to the audio source.</summary>
    private void SetupSource(AudioDefinition definition)
    {
        audioSource.outputAudioMixerGroup = definition.MixerGroup;

        audioSource.volume = definition.Volume;

        audioSource.pitch =
            definition.Pitch + Random.Range(
                -definition.RandomPitch,
                 definition.RandomPitch);

        audioSource.loop = definition.Loop;

        audioSource.priority = definition.Priority;

        audioSource.spatialBlend = definition.SpatialBlend;

        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
    }
}