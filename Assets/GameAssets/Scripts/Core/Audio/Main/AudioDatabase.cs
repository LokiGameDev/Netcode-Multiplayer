using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Custom/Audio/Audio Database")]
public class AudioDatabase : ScriptableObject
{
    [SerializeField] private List<AudioDefinition> definitions;

    private Dictionary<AudioID, AudioDefinition> lookup;

    public void Initialize()
    {
        lookup = new Dictionary<AudioID, AudioDefinition>();

        foreach (var definition in definitions)
        {
            lookup[definition.id] = definition;
        }
    }

    public AudioDefinition Get(AudioID id)
    {
        if (lookup.TryGetValue(id, out var definition))
            return definition;

        Debug.LogWarning($"Audio '{id}' not found.");
        return null;
    }

}
