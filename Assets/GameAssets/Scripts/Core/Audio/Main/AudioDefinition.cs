using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioDefinition", menuName = "Custom/Audio/Audio Definition")]
/// <summary>Describes the clips and playback settings for one audio ID.</summary>
public class AudioDefinition : ScriptableObject
{
    /// <summary>Identifier used to request this audio definition.</summary>
    public AudioID id;
    /// <summary>Clips available for playback.</summary>
    public AudioClip[] clips;
    [Range(0f, 1f)]
    public float Volume = 1;
    public float Pitch = 1;
    public float RandomPitch = 0;
    public bool Loop = true;
    [Range(0f, 1f)]
    public float SpatialBlend = 0;
    public int Priority = 0;
    public AudioMixerGroup MixerGroup;

    public SelectionMode selectionMode = SelectionMode.Sequential;

    public enum SelectionMode
    {
        Random,
        Sequential
    }

    public int currentClipIndex = 0;

    /// <summary>Selects the next clip according to the configured selection mode.</summary>
    public AudioClip GetClip()
    {
        if (clips == null || clips.Length == 0)
            return null;

        if(selectionMode == SelectionMode.Random)
        {
            return clips[Random.Range(0, clips.Length)];
        }
        else
        {
            if(currentClipIndex >= clips.Length) currentClipIndex = 0;

            AudioClip clip = clips[currentClipIndex];
            currentClipIndex++;
            return clip;
        }
    }
}
