using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ClientSingleton clientPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(this);
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if(isDedicatedServer)
        {
            return;
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost();
            
            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool isAuthenticated = await clientSingleton.CreateClient();

            if(isAuthenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }
}
