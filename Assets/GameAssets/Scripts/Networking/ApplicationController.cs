using System.Collections;
using System.Threading.Tasks;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Bootstraps host/client services and displays loading progress.</summary>
public class ApplicationController : MonoBehaviour
{
    [Tooltip("Prefab used to create the host singleton.")]
    [SerializeField] private HostSingleton hostPrefab;
    [Tooltip("Prefab used to create the client singleton.")]
    [SerializeField] private ClientSingleton clientPrefab;
    [Tooltip("Loading progress slider.")]
    [SerializeField] private Slider loadingBar;
    [Tooltip("Speed at which the loading bar approaches its waiting value.")]
    [SerializeField] private float loadingSpeed = 0.2f;
    [Tooltip("Speed at which the loading bar completes after authentication.")]
    [SerializeField] private float finishSpeed = 1;

    private ClientSingleton clientSingleton1;

    private bool readyToFinish = false;

    /// <summary>Starts loading progress and launches the local networking mode.</summary>
    private async void Start()
    {
        DontDestroyOnLoad(this);
        loadingBar.value = 0;

        StartCoroutine(LoadingBarRoutine());

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    /// <summary>Animates the loading bar until the client is ready.</summary>
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

    /// <summary>Creates and initializes host and client services.</summary>
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
