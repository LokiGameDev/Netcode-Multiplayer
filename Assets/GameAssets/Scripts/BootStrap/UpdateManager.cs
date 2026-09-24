using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateManager : MonoBehaviour
{
    [System.Serializable]
    public class VersionData
    {
        public string version;
        public string apkUrl;
    }

    [SerializeField]
    private string currentVersion = "1.0.0";

    [SerializeField]
    private string versionJsonUrl =
        "https://raw.githubusercontent.com/LokiGameDev/Netcode-Multiplayer/main/version.json";

    private void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(CheckForUpdate());
#endif
    }

    private IEnumerator CheckForUpdate()
    {
        using UnityWebRequest request =
            UnityWebRequest.Get(versionJsonUrl);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(
                "Update check failed: " + request.error);

            yield break;
        }

        VersionData data =
            JsonUtility.FromJson<VersionData>(
                request.downloadHandler.text);

        Debug.Log(
            $"Current Version: {currentVersion}");

        Debug.Log(
            $"Latest Version: {data.version}");

        if (IsNewerVersion(data.version, currentVersion))
        {
            Debug.Log("Update available!");

            Application.OpenURL(data.apkUrl);
        }
        else
        {
            Debug.Log("Game is up to date.");
        }
    }

    private bool IsNewerVersion(
        string onlineVersion,
        string installedVersion)
    {
        System.Version online =
            new System.Version(onlineVersion);

        System.Version installed =
            new System.Version(installedVersion);

        return online > installed;
    }
}