using System.Collections;
using System.Threading.Tasks;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private float loadingSpeed = 0.2f;
    [SerializeField] private float finishSpeed = 1;

    private ClientSingleton clientSingleton1;

    private bool readyToFinish = false;

    private async void Start()
    {
        DontDestroyOnLoad(this);
        loadingBar.value = 0;

        StartCoroutine(LoadingBarRoutine());

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private IEnumerator LoadingBarRoutine()
    {
        while (!readyToFinish)
        {
            loadingBar.value = Mathf.MoveTowards(
                loadingBar.value,
                0.9f,
                loadingSpeed * Time.deltaTime
            );

            yield return null;
        }

        // Bool became true → finish to 100%
        while (loadingBar.value < 1f)
        {
            loadingBar.value = Mathf.MoveTowards(
                loadingBar.value,
                1f,
                finishSpeed * Time.deltaTime
            );

            yield return null;
        }

        clientSingleton1?.GameManager.GoToMenu();
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
                clientSingleton1 = clientSingleton;
                readyToFinish = true;
            }
        }
    }
}
