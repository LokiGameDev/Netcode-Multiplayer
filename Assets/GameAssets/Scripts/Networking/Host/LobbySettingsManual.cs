using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySettingsManual : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Slider numberOfPlayersSlider;
    [SerializeField] private Toggle lobbyTypeToggle;
    [SerializeField] private TMP_Text lobbyCurrentNumberText;

    [SerializeField] private int MINNUMBEROFPLAYERS = 2;
    [SerializeField] private int MAXNUMBEROFPLAYERS = 20;
    [SerializeField] private int MAXLOBBYNAMELENGTH = 10;

    [SerializeField] private ShowNotification showNotification;

    private LobbySettings lobbySettingsToCreate;

    private void OnEnable()
    {
        lobbySettingsToCreate = new LobbySettings();
    }

    public void LoadCurrentSettings()
    {
        lobbyNameInputField.characterLimit = MAXLOBBYNAMELENGTH;
        lobbyNameInputField.text = PlayerPrefs.GetString("PlayerName");
        lobbySettingsToCreate.LobbyName = lobbyNameInputField.text;
        numberOfPlayersSlider.minValue = MINNUMBEROFPLAYERS;
        numberOfPlayersSlider.maxValue = MAXNUMBEROFPLAYERS;
        numberOfPlayersSlider.value = lobbySettingsToCreate.NumberOfPlayers;
        lobbyCurrentNumberText.text = $"{lobbySettingsToCreate.NumberOfPlayers}";
        lobbyTypeToggle.isOn = lobbySettingsToCreate.PublicLobby;
    }

    public LobbySettings GetLobbySettingsToHost()
    {
        if(ValidateLobbySettings())
            return lobbySettingsToCreate;
        return null;
    }

    public void ChangeInLobbySettings()
    {
        ValidateLobbySettings();

        lobbySettingsToCreate.LobbyName = lobbyNameInputField.text;
        lobbySettingsToCreate.NumberOfPlayers = (int)numberOfPlayersSlider.value;
        lobbySettingsToCreate.PublicLobby = lobbyTypeToggle.isOn;
    }

    private bool ValidateLobbySettings()
    {
        if(lobbyNameInputField.text.Length > 0 && lobbyNameInputField.text.Length < MAXLOBBYNAMELENGTH)
        {
            return true;
        }
        ShowNameInvalidMessage();
        return false;
    }

    private void ShowNameInvalidMessage()
    {
        if(lobbyNameInputField.text.Length <= 0) showNotification.ShowText("Name cannot be empty");
        else showNotification.ShowText("Name is invalid! Try another");
    }

    public void OnPlayerCountChanged()
    {
        lobbyCurrentNumberText.text = $"{(int)numberOfPlayersSlider.value}";
    }

    public void OnLobbyNameChanged()
    {
        ValidateLobbySettings();
    }
}


public class LobbySettings
{
    public string LobbyName = "Default";
    public int NumberOfPlayers = 10;
    public bool PublicLobby = true;
}