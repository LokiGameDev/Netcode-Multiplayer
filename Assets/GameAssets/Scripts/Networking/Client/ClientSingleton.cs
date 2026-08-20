using System.Threading.Tasks;
using UnityEngine;
public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;
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

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();
        return await GameManager.InitAsync();
    }

    public void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
