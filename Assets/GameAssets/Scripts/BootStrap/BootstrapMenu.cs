using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootstrapMenu : MonoBehaviour
{
    private const string PlayerName = "PlayerName";

    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private GameObject newAccountPanel;
    [SerializeField] private ShowNotification displayMessage;
    [SerializeField] private Button enterTheGameButton;
    
    [SerializeField] private GameObject connectionLostPanel;

    [SerializeField] private int minPlayerNameLength = 1;
    [SerializeField] private int maxPlayerNameLength = 10;

    public UnityEvent OnStartEvent;

    private void Start()
    {
        Debug.Log(PlayerPrefs.GetString(PlayerName));
        connectionLostPanel.SetActive(false);

        OnStartEvent?.Invoke();
    }

    public void PlayerNameValidation()
    {
        if(playerNameInputField.text.Length < minPlayerNameLength || playerNameInputField.text.Length > maxPlayerNameLength)
        {
            enterTheGameButton.interactable = false;
            displayMessage.ShowText($"Player should be between {minPlayerNameLength} and {maxPlayerNameLength} length");
        }
        else
        {
            enterTheGameButton.interactable = true;
        }
    }

    public void EnterTheGame()
    {
        if(playerNameInputField.text.Length > minPlayerNameLength && playerNameInputField.text.Length < maxPlayerNameLength)
        {
            PlayerPrefs.SetString(PlayerName, playerNameInputField.text);
            PlayerPrefs.SetInt("PlayerAuthenticated", 0);
            LoadNetBootstrap();
        }
        else
        {
            displayMessage.ShowText($"Player should be between {minPlayerNameLength} and {maxPlayerNameLength} length");
        }
    }

    public void StartLoading()
    {
        Debug.Log("Loading...");
        if(PlayerPrefs.GetString(PlayerName).Length <= minPlayerNameLength)
        {
            newAccountPanel.SetActive(true);
            PlayerNameValidation();
        }
        else
        {
            newAccountPanel.SetActive(false);
            LoadNetBootstrap();
        }
    }

    private void LoadNetBootstrap()
    {
        if(!IsInternetAvailable())
        {
            ConnectionLost();
            return;
        }

        AudioManager.Instance.PlayMusic(AudioID.Music);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayLogoSoundEffect()
    {
        AudioManager.Instance.Play(AudioID.Logo);
    }

    public void ReloadTheGame()
    {
        Debug.Log("Button Clicked");
        AudioManager.Instance.StopMusic();
        SceneManager.LoadScene(0);
    }

    private void ConnectionLost()
    {
        connectionLostPanel.SetActive(true);
    }

    private bool IsInternetAvailable()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
}
