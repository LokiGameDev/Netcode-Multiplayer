using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsUpdater : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private const string MusicKey = "MusicVolume";
    private const string SFXKey = "SFXVolume";

    private void Start()
    {
        LoadSettings();
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat(MusicKey, ConvertToDecibels(value));

        PlayerPrefs.SetFloat(MusicKey, value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat(SFXKey, ConvertToDecibels(value));

        PlayerPrefs.SetFloat(SFXKey, value);
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MusicKey, 1f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFXKey, 1f);
    }

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

    private float ConvertToDecibels(float value)
    {
        if (value <= 0.0001f)
            return -80f;

        return Mathf.Log10(value) * 20f;
    }
}
