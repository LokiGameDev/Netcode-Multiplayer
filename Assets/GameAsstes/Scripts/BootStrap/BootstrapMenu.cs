using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootstrapMenu : MonoBehaviour
{
    private const string PlayerName = "PlayerName";

    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private GameObject newAccountPanel;
    [SerializeField] private DisplayMessage displayMessage;
    [SerializeField] private Button enterTheGameButton;

    [SerializeField] private int minPlayerNameLength = 1;
    [SerializeField] private int maxPlayerNameLength = 15;

    private void Start()
    {
        Debug.Log(PlayerPrefs.GetString(PlayerName));

        if(PlayerPrefs.GetString(PlayerName).Length < minPlayerNameLength)
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

    public void PlayerNameValidation()
    {
        if(playerNameInputField.text.Length < minPlayerNameLength || playerNameInputField.text.Length > maxPlayerNameLength)
        {
            enterTheGameButton.interactable = false;
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
            LoadNetBootstrap();
        }
        else
        {
            displayMessage.ShowText($"Player should be between {minPlayerNameLength} and {maxPlayerNameLength} length");
        }
    }

    private void LoadNetBootstrap()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
