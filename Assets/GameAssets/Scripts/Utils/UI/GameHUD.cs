using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>Controls the in-game heads-up display.</summary>
public class GameHUD : NetworkBehaviour
{
    [Header("HUD State")]
    [Tooltip("Objects enabled when the HUD starts.")]
    [SerializeField] private GameObject[] enableOnStart;
    [Tooltip("Objects disabled when the HUD starts.")]
    [SerializeField] private GameObject[] disableOnStart;
    [SerializeField] private GameObject[] playerStateObjects;

    [Header("Join Code")]
    [Tooltip("Text displaying the host join code.")]
    [SerializeField] private TMP_Text joinCodeText;
    [Tooltip("Object containing the join code display.")]
    [SerializeField] private GameObject joinCodeObject;

    /// <summary>Applies the configured HUD object visibility.</summary>
    private void OnEnable()
    {
        foreach(GameObject gameObject in enableOnStart) gameObject.SetActive(true);

        foreach(GameObject gameObject in disableOnStart) gameObject.SetActive(false);
    }

    /// <summary>Leaves the current host or client session.</summary>
    public void LeaveGame()
    {
        if(NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.Dispose();
        }
        ClientSingleton.Instance.GameManager.Disconnect();
    }

    /// <summary>Displays the host join code when this client is the host.</summary>
    public void Start()
    {
        if(!IsHost)
        {
            joinCodeObject.SetActive(false);
        }

        string joinCode = HostSingleton.Instance.GameManager.GetJoinCode();
        joinCodeText.text = joinCode;
    }

    public void PlayerStateChanged(bool state)
    {
        foreach(GameObject gameObject in playerStateObjects) gameObject.SetActive(state);
    }
}
