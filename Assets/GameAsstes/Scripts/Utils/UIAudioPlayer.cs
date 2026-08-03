using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioID audioID;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        AudioManager.Instance.Play(audioID);
    }
}
