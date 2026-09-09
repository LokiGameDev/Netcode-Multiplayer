using System.Threading.Tasks;
using UnityEngine;

/// <summary>Owns the client game manager for the current application.</summary>
public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;
    /// <summary>Gets the active client singleton.</summary>
    public static ClientSingleton Instance
    {
        get
        {
            if(instance != null) return instance;

            instance = FindAnyObjectByType<ClientSingleton>();

            if(instance==null) Debug.LogError("Client Singleton is null");

            return instance;
        }
    }

    public ClientGameManager GameManager;

    /// <summary>Keeps the client singleton across scene loads.</summary>
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>Creates and initializes the client game manager.</summary>
    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();
        return await GameManager.InitAsync();
    }

    /// <summary>Disposes the client game manager when destroyed.</summary>
    public void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
