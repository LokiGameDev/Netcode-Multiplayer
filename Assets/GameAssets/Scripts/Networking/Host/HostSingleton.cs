using System.Collections;
using UnityEngine;

/// <summary>Owns the host game manager across scene changes.</summary>
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

    /// <summary>Keeps the host singleton alive across scene changes.</summary>
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>Creates the host game manager.</summary>
    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }

    /// <summary>Disposes the current host game manager.</summary>
    public void Dispose()
    {
        GameManager?.Dispose();
    }
}
