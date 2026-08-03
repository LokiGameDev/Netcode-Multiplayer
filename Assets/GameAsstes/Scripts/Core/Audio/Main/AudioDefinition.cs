using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioDefinition", menuName = "Custom/Audio/Audio Definition")]
public class AudioDefinition : ScriptableObject
{
    public AudioID id;
    public AudioClip[] clips;
    public float Volume = 1;
    public float Pitch = 1;
    public float RandomPitch = 0;
    public bool Loop = true;
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
