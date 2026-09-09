using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Connects volume sliders and mute indicators to the audio manager.</summary>
public class VolumeSettingsChanger : MonoBehaviour
{
    [Tooltip("Slider used to adjust music volume.")]
    [SerializeField] private Slider musicSlider;
    [Tooltip("Slider used to adjust sound-effect volume.")]
    [SerializeField] private Slider sfxSlider;

    [Tooltip("Text displaying the current music volume.")]
    [SerializeField] private TMP_Text musicVolumeLevel;
    [Tooltip("Text displaying the current sound-effect volume.")]
    [SerializeField] private TMP_Text sfxVolumeLevel;

    [Tooltip("Image shown when music is muted.")]
    [SerializeField] private GameObject musicMutedImage;
    [Tooltip("Image shown when sound effects are muted.")]
    [SerializeField] private GameObject sfxMutedImage;

    /// <summary>Initializes sliders from the saved audio settings.</summary>
    private void Start()
    {
        musicSlider.value = AudioManager.Instance.GetMusicVolume() * musicSlider.maxValue;
        sfxSlider.value = AudioManager.Instance.GetSFXVolume() * sfxSlider.maxValue;

        musicVolumeLevel.text = $"{musicSlider.value}";
        sfxVolumeLevel.text = $"{sfxSlider.value}";

        if(musicSlider.value == 0) MuteMusic(true);
        if(sfxSlider.value == 0) MuteSFX(true);
    }

    /// <summary>Applies the current music slider value.</summary>
    public void MusicVolumeChange()
    {
        AudioManager.Instance.SetMusicVolume(musicSlider.value/musicSlider.maxValue);
        musicVolumeLevel.text = $"{musicSlider.value}";
        MuteMusic(musicSlider.value == 0);
    }

    /// <summary>Applies the current sound-effect slider value.</summary>
    public void SFXVolumeChange()
    {
        AudioManager.Instance.SetSFXVolume(sfxSlider.value/sfxSlider.maxValue);
        sfxVolumeLevel.text = $"{sfxSlider.value}";
        MuteSFX(sfxSlider.value == 0);
    }

    /// <summary>Updates the music mute indicator.</summary>
    private void MuteMusic(bool state)
    {
        musicMutedImage.SetActive(state);
        musicVolumeLevel.gameObject.SetActive(!state);
    }

    /// <summary>Updates the sound-effect mute indicator.</summary>
    private void MuteSFX(bool state)
    {
        sfxMutedImage.SetActive(state);
        sfxVolumeLevel.gameObject.SetActive(!state);
    }
}