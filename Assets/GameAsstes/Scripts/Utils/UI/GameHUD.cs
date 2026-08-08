using Unity.Netcode;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private GameObject[] enableOnStart;
    [SerializeField] private GameObject[] disableOnStart;

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
}
