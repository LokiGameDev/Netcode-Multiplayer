using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>Persists and applies music and sound-effect volume settings.</summary>
public class AudioSettingsUpdater : MonoBehaviour
{
    [Tooltip("Mixer containing the music and sound-effect volume parameters.")]
    [SerializeField] private AudioMixer audioMixer;

    private const string MusicKey = "MusicVolume";
    private const string SFXKey = "SFXVolume";

    /// <summary>Loads saved volume settings when the component starts.</summary>
    private void Start()
    {
        LoadSettings();
    }

    /// <summary>Applies and stores the music volume.</summary>
    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat(MusicKey, ConvertToDecibels(value));

        PlayerPrefs.SetFloat(MusicKey, value);
    }

    /// <summary>Applies and stores the sound-effect volume.</summary>
    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat(SFXKey, ConvertToDecibels(value));

        PlayerPrefs.SetFloat(SFXKey, value);
    }

    /// <summary>Returns the saved music volume.</summary>
    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MusicKey, 1f);
    }

    /// <summary>Returns the saved sound-effect volume.</summary>
    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFXKey, 1f);
    }

    /// <summary>Applies saved volume values to the mixer.</summary>
    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicKey, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFXKey, 1f);

        audioMixer.SetFloat(
            MusicKey,
            ConvertToDecibels(musicVolume)
        );

        audioMixer.SetFloat(
            SFXKey,
            ConvertToDecibels(sfxVolume)
        );
    }

    /// <summary>Converts a normalized volume value to decibels.</summary>
    private float ConvertToDecibels(float value)
    {
        if (value <= 0.0001f)
            return -80f;

        return Mathf.Log10(value) * 20f;
    }
}
