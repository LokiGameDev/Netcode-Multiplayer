using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private Transform followTarget;
    private Vector3 worldPosition;

    private AudioPool ownerPool;

    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Initialize(AudioPool pool)
    {
        ownerPool = pool;
    }

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

    public void Stop()
    {
        audioSource.Stop();

        isPlaying = false;

        followTarget = null;

        ownerPool.Return(this);
    }

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