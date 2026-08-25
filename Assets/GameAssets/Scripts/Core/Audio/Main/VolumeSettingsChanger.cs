using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettingsChanger : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private TMP_Text musicVolumeLevel;
    [SerializeField] private TMP_Text sfxVolumeLevel;

    [SerializeField] private GameObject musicMutedImage;
    [SerializeField] private GameObject sfxMutedImage;

    private void Start()
    {
        musicSlider.value = AudioManager.Instance.GetMusicVolume() * musicSlider.maxValue;
        sfxSlider.value = AudioManager.Instance.GetSFXVolume() * sfxSlider.maxValue;

        musicVolumeLevel.text = $"{musicSlider.value}";
        sfxVolumeLevel.text = $"{sfxSlider.value}";

        if(musicSlider.value == 0) MuteMusic(true);
        if(sfxSlider.value == 0) MuteSFX(true);
    }

    public void MusicVolumeChange()
    {
        AudioManager.Instance.SetMusicVolume(musicSlider.value/musicSlider.maxValue);
        musicVolumeLevel.text = $"{musicSlider.value}";
        MuteMusic(musicSlider.value == 0);
    }

    public void SFXVolumeChange()
    {
        AudioManager.Instance.SetSFXVolume(sfxSlider.value/sfxSlider.maxValue);
        sfxVolumeLevel.text = $"{sfxSlider.value}";
        MuteSFX(sfxSlider.value == 0);
    }

    private void MuteMusic(bool state)
    {
        musicMutedImage.SetActive(state);
        musicVolumeLevel.gameObject.SetActive(!state);
    }

    private void MuteSFX(bool state)
    {
        sfxMutedImage.SetActive(state);
        sfxVolumeLevel.gameObject.SetActive(!state);
    }
}