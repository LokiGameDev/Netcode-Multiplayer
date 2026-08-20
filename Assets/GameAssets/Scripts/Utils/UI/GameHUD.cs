using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : NetworkBehaviour
{
    [SerializeField] private GameObject[] enableOnStart;
    [SerializeField] private GameObject[] disableOnStart;

    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private GameObject joinCodeObject;

    private void OnEnable()
    {
        foreach(GameObject gameObject in enableOnStart) gameObject.SetActive(true);

        foreach(GameObject gameObject in disableOnStart) gameObject.SetActive(false);
    }

    public void LeaveGame()
    {
        if(NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.Dispose();
        }
        ClientSingleton.Instance.GameManager.Disconnect();
    }

    public void Start()
    {
        if(!IsHost)
        {
            joinCodeObject.SetActive(false);
        }

        string joinCode = HostSingleton.Instance.GameManager.GetJoinCode();
        joinCodeText.text = joinCode;
    }
}
