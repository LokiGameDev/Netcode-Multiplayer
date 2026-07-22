using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System;
using System.Threading.Tasks;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInputField;
    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeInputField.text);
    }
}
