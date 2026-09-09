using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Collects and validates lobby settings from menu controls.</summary>
public class LobbySettingsManual : MonoBehaviour
{
    [Header("Lobby Controls")]
    [Tooltip("Input field containing the lobby name.")]
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [Tooltip("Slider controlling the maximum player count.")]
    [SerializeField] private Slider numberOfPlayersSlider;
    [Tooltip("Toggle controlling whether the lobby is public.")]
    [SerializeField] private Toggle lobbyTypeToggle;
    [Tooltip("Text displaying the selected player count.")]
    [SerializeField] private TMP_Text lobbyCurrentNumberText;

    [Header("Lobby Limits")]
    [Tooltip("Minimum number of players allowed.")]
    [SerializeField] private int MINNUMBEROFPLAYERS = 2;
    [Tooltip("Maximum number of players allowed.")]
    [SerializeField] private int MAXNUMBEROFPLAYERS = 20;
    [Tooltip("Maximum lobby name length.")]
    [SerializeField] private int MAXLOBBYNAMELENGTH = 10;

    [Tooltip("Displays validation messages.")]
    [SerializeField] private ShowNotification showNotification;

    private LobbySettings lobbySettingsToCreate;

    /// <summary>Creates default settings when the panel is enabled.</summary>
    private void OnEnable()
    {
        lobbySettingsToCreate = new LobbySettings();
    }

    /// <summary>Loads the current player and lobby settings into the controls.</summary>
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

    /// <summary>Returns validated settings for creating a lobby.</summary>
    /// <returns>Lobby settings, or null when validation fails.</returns>
    public LobbySettings GetLobbySettingsToHost()
    {
        if(ValidateLobbySettings())
            return lobbySettingsToCreate;
        return null;
    }

    /// <summary>Copies the current control values into the lobby settings.</summary>
    public void ChangeInLobbySettings()
    {
        ValidateLobbySettings();

        lobbySettingsToCreate.LobbyName = lobbyNameInputField.text;
        lobbySettingsToCreate.NumberOfPlayers = (int)numberOfPlayersSlider.value;
        lobbySettingsToCreate.PublicLobby = lobbyTypeToggle.isOn;
    }

    /// <summary>Validates the configured lobby name.</summary>
    /// <returns>True when the lobby name is valid.</returns>
    private bool ValidateLobbySettings()
    {
        if(lobbyNameInputField.text.Length > 0 && lobbyNameInputField.text.Length < MAXLOBBYNAMELENGTH)
        {
            return true;
        }
        ShowNameInvalidMessage();
        return false;
    }

    /// <summary>Displays the appropriate lobby-name validation message.</summary>
    private void ShowNameInvalidMessage()
    {
        if(lobbyNameInputField.text.Length <= 0) showNotification.ShowText("Name cannot be empty");
        else showNotification.ShowText("Name is invalid! Try another");
    }

    /// <summary>Updates the visible player count after slider changes.</summary>
    public void OnPlayerCountChanged()
    {
        lobbyCurrentNumberText.text = $"{(int)numberOfPlayersSlider.value}";
    }

    /// <summary>Validates the lobby name after text changes.</summary>
    public void OnLobbyNameChanged()
    {
        ValidateLobbySettings();
    }
}


/// <summary>Settings used to create a multiplayer lobby.</summary>
public class LobbySettings
{
    /// <summary>Name displayed for the lobby.</summary>
    public string LobbyName = "Default";
    /// <summary>Maximum number of players allowed.</summary>
    public int NumberOfPlayers = 6;
    /// <summary>Whether the lobby is visible to other players.</summary>
    public bool PublicLobby = true;
}