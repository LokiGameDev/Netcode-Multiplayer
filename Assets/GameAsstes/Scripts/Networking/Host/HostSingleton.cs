using System.Collections;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;
    public static HostSingleton Instance
    {
        get
        {
            if(instance != null) return instance;

            instance = FindAnyObjectByType<HostSingleton>();

            if(instance==null) Debug.LogError("Host Singleton is null");

            return instance;
        }
    }

    public HostGameManager GameManager;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }

    public void Dispose()
    {
        GameManager?.Dispose();
    }
}
