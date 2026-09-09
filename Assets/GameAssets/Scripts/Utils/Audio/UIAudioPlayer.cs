using UnityEngine;
using UnityEngine.UI;

/// <summary>Plays configured UI sounds for local interface actions.</summary>
public class UIAudioPlayer : MonoBehaviour
{
    [Header("UI Audio")]
    [Tooltip("Audio ID played for the UI action.")]
    [SerializeField] private AudioID audioID = AudioID.ButtonClick;

    [Tooltip("Button whose click event triggers the sound.")]
    [SerializeField] private Button button;
    [Tooltip("Toggle whose value change triggers the sound.")]
    [SerializeField] private Toggle toggle;

    /// <summary>Connects UI events to the configured audio.</summary>
    private void Awake()
    {
        button = GetComponent<Button>();
        toggle = GetComponent<Toggle>();

        if(button!=null) button.onClick.AddListener(OnClicked);
        if(toggle!=null) toggle.onValueChanged.AddListener(OnChanged);
    }

    /// <summary>Removes the button audio listener.</summary>
    private void OnDestroy()
    {
        if(button!=null) button.onClick.RemoveListener(OnClicked);
    }

    /// <summary>Plays the configured sound after a toggle changes.</summary>
    private void OnChanged(bool state = false)
    {
        AudioManager.Instance?.Play(audioID);
    }

    /// <summary>Plays the configured sound after a button click.</summary>
    private void OnClicked()
    {
        AudioManager.Instance?.Play(audioID);
    }
}
