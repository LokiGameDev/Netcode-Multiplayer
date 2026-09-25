using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Controls the initial player-name and scene-loading menu.</summary>
public class BootstrapMenu : MonoBehaviour
{
    private const string PlayerName = "PlayerName";

    [Tooltip("Input field for the player's display name.")]
    [SerializeField] private TMP_InputField playerNameInputField;
    [Tooltip("Panel shown when a new player name is required.")]
    [SerializeField] private GameObject newAccountPanel;
    [Tooltip("Displays validation and connection messages.")]
    [SerializeField] private ShowNotification displayMessage;
    [Tooltip("Button used to enter the game.")]
    [SerializeField] private Button enterTheGameButton;
    [Tooltip("Loading panel shown while the game starts.")]
    [SerializeField] private GameObject logoLoadingPanel;
    [Tooltip("Panel shown when the connection is unavailable.")]
    [SerializeField] private GameObject connectionLostPanel;
    [SerializeField] private UpdateManager updateManager;

    [Tooltip("Minimum allowed player-name length.")]
    [SerializeField] private int minPlayerNameLength = 1;
    [Tooltip("Maximum allowed player-name length.")]
    [SerializeField] private int maxPlayerNameLength = 10;

    public UnityEvent OnStartEvent;

    /// <summary>Initializes the menu and restores the saved player state.</summary>
    private void Start()
    {
        Debug.Log(PlayerPrefs.GetString(PlayerName));

        if(!PlayerPrefs.HasKey("PlayerGems"))
        {
            PlayerPrefs.SetInt("PlayerGems", 0);
        }

        connectionLostPanel.SetActive(false);
        logoLoadingPanel.SetActive(true);
        newAccountPanel.SetActive(PlayerPrefs.GetString(PlayerName).Length < minPlayerNameLength);

        OnStartEvent?.Invoke();
    }

    /// <summary>Updates name validation feedback and enter-button state.</summary>
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

    /// <summary>Saves a valid player name and loads the game bootstrap scene.</summary>
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

    /// <summary>Starts loading the game or opens the name-entry panel.</summary>
    public void StartLoading()
    {
        Debug.Log("Loading...");

        StartCoroutine(WaitForUpdateCheck());
    }

    private IEnumerator WaitForUpdateCheck()
    {
        Debug.Log("Checking for update status");
        while(!updateManager.isCheckedForUpdate)
        {
            yield return new WaitForSeconds(1);
        }
        Debug.Log("Game is up-to date");

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

    /// <summary>Checks connectivity and loads the next scene.</summary>
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

    /// <summary>Plays the logo transition sound effect.</summary>
    public void PlayLogoSoundEffect()
    {
        AudioManager.Instance.Play(AudioID.Logo);
    }

    /// <summary>Stops the current music and reloads the initial scene.</summary>
    public void ReloadTheGame()
    {
        Debug.Log("Button Clicked");
        AudioManager.Instance.StopMusic();
        SceneManager.LoadScene(0);
    }

    /// <summary>Shows the connection-lost panel.</summary>
    private void ConnectionLost()
    {
        connectionLostPanel.SetActive(true);
    }

    /// <summary>Returns whether the device reports an available network.</summary>
    private bool IsInternetAvailable()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
}
