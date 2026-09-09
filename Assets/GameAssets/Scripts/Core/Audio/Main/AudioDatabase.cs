using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Custom/Audio/Audio Database")]
/// <summary>Stores audio definitions and provides efficient ID lookups.</summary>
public class AudioDatabase : ScriptableObject
{
    [Tooltip("Audio definitions available to the audio system.")]
    [SerializeField] private List<AudioDefinition> definitions;

    private Dictionary<AudioID, AudioDefinition> lookup;

    /// <summary>Builds the lookup table from the configured definitions.</summary>
    public void Initialize()
    {
        lookup = new Dictionary<AudioID, AudioDefinition>();

        foreach (var definition in definitions)
        {
            lookup[definition.id] = definition;
        }
    }

    /// <summary>Returns the definition registered for an audio ID.</summary>
    public AudioDefinition Get(AudioID id)
    {
        if (lookup.TryGetValue(id, out var definition))
            return definition;

        Debug.LogWarning($"Audio '{id}' not found.");
        return null;
    }

}
