using UnityEngine;
using TMPro;
using System;
using UnityEditor;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private LobbySettingsManual lobbySettingsManual;
    [SerializeField] private TMP_InputField joinCodeInputField;

    [SerializeField] private GameObject[] enableOnStart;
    [SerializeField] private GameObject[] disbleOnStart;

    private void OnEnable()
    {
        Array.ForEach(enableOnStart, obj => obj.SetActive(true));
        Array.ForEach(disbleOnStart, obj => obj.SetActive(false));
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public async void StartHostCustom()
    {
        if(lobbySettingsManual.GetLobbySettingsToHost()!=null)
            await HostSingleton.Instance.GameManager.StartHostAsync(lobbySettingsManual.GetLobbySettingsToHost());
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeInputField.text);
    }

    public void QuitTheGame()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
}
