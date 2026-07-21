using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System;

public class ConnectionButtons : MonoBehaviour
{
    private bool isClientSearching = false;

    public void Joingame()
    {
        if(isClientSearching) return;
        bool clientJoined = NetworkManager.Singleton.StartClient();

        if(!clientJoined)
        {
            isClientSearching = true;
            DebugMessage.Instance.Display("Client not joined");
            StartCoroutine(MatchmakingTimeOut());
        }
    }

    IEnumerator MatchmakingTimeOut()
    {
        yield return new WaitForSeconds(5);
        DebugMessage.Instance.Display("Client looking is stopped");
        NetworkManager.Singleton.Shutdown();
        isClientSearching = false;
    }

    public void HostGame()
    {
        NetworkManager.Singleton.StartHost();
    }
}
