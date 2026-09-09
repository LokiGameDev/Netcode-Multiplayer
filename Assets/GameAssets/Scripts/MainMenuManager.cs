using UnityEngine;
using TMPro;
using System;
using UnityEditor;

/// <summary>Starts host and client sessions from the main menu.</summary>
public class MainMenuManager : MonoBehaviour
{
    [Tooltip("Lobby settings used when creating a custom host session.")]
    [SerializeField] private LobbySettingsManual lobbySettingsManual;
    [Tooltip("Input field containing the lobby join code.")]
    [SerializeField] private TMP_InputField joinCodeInputField;

    [Tooltip("Objects enabled when the menu becomes active.")]
    [SerializeField] private GameObject[] enableOnStart;
    [Tooltip("Objects disabled when the menu becomes active.")]
    [SerializeField] private GameObject[] disbleOnStart;

    /// <summary>Applies the menu's enabled and disabled object states.</summary>
    private void OnEnable()
    {
        Array.ForEach(enableOnStart, obj => obj.SetActive(true));
        Array.ForEach(disbleOnStart, obj => obj.SetActive(false));
    }

    /// <summary>Starts a host using the default lobby settings.</summary>
    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    /// <summary>Starts a host using the configured custom lobby settings.</summary>
    public async void StartHostCustom()
    {
        if(lobbySettingsManual.GetLobbySettingsToHost()!=null)
            await HostSingleton.Instance.GameManager.StartHostAsync(lobbySettingsManual.GetLobbySettingsToHost());
    }

    /// <summary>Starts a client using the join-code input field.</summary>
    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeInputField.text);
    }

    /// <summary>Starts a client using a supplied join code.</summary>
    public async void StartClient(string joinCode)
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
    }

    /// <summary>Exits play mode in the editor or quits the application.</summary>
    public void QuitTheGame()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
}
