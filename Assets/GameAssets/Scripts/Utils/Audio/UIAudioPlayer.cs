using UnityEngine;
using UnityEngine.UI;

public class UIAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioID audioID = AudioID.ButtonClick;

    [SerializeField] private Button button;
    [SerializeField] private Toggle toggle;

    private void Awake()
    {
        button = GetComponent<Button>();
        toggle = GetComponent<Toggle>();

        if(button!=null) button.onClick.AddListener(OnClicked);
        if(toggle!=null) toggle.onValueChanged.AddListener(OnChanged);
    }

    private void OnDestroy()
    {
        if(button!=null) button.onClick.RemoveListener(OnClicked);
    }

    private void OnChanged(bool state = false)
    {
        AudioManager.Instance?.Play(audioID);
    }

    private void OnClicked()
    {
        AudioManager.Instance?.Play(audioID);
    }
}
