using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpdateManager : MonoBehaviour
{
    [System.Serializable]
    public class VersionData
    {
        public string version;
        public string apkUrl;
    }

    [Header("Version")]
    [SerializeField] private string currentVersion = "1.0.2";

    [SerializeField]
    private string versionJsonUrl =
        "https://raw.githubusercontent.com/LokiGameDev/Netcode-Multiplayer/main/version.json";

    [Header("UI")]
    [SerializeField] private GameObject updatePanel;
    [SerializeField] private TMP_Text updateText;
    [SerializeField] private Button installButton;

    private string apkUrl;

    public bool isCheckedForUpdate = false;

    public void StartChecking()
    {
        isCheckedForUpdate = false;
        #if UNITY_ANDROID && !UNITY_EDITOR
                StartCoroutine(CheckForUpdate());
        #endif
    }

    private IEnumerator CheckForUpdate()
    {
        Debug.Log("Checking for Grave Shift update...");

        using UnityWebRequest request =
            UnityWebRequest.Get(versionJsonUrl);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Update check failed: " + request.error);
            isCheckedForUpdate = true;
            yield break;
        }

        Debug.Log("Version JSON:");
        Debug.Log(request.downloadHandler.text);

        VersionData data =
            JsonUtility.FromJson<VersionData>(
                request.downloadHandler.text);

        if (data == null || string.IsNullOrEmpty(data.version) || string.IsNullOrEmpty(data.apkUrl))
        {
            Debug.LogError("Malformed version.json response.");
            isCheckedForUpdate = true;
            yield break;
        }

        Debug.Log("Installed: " + currentVersion);
        Debug.Log("Latest: " + data.version);

        if (IsNewerVersion(data.version, currentVersion))
        {
            apkUrl = data.apkUrl;

            updatePanel.SetActive(true);

            updateText.text =
                "Update Available!\n\nVersion " + data.version;

            installButton.onClick.RemoveAllListeners();

            installButton.onClick.AddListener(StartUpdate);

            Debug.Log("Update available!");
        }
        else
        {
            Debug.Log("Grave Shift is up to date.");

            isCheckedForUpdate = true;
        }
    }

    private void StartUpdate()
    {
    #if UNITY_ANDROID && !UNITY_EDITOR

        installButton.interactable = false;

        updateText.text = "Downloading update...";

        StartCoroutine(StartAndroidDownload());

    #endif
    }

    private IEnumerator StartAndroidDownload()
    {
        Debug.Log("Starting Android APK download...");

        yield return new WaitForSeconds(0.2f);

        using (AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity =
                unityPlayer.GetStatic<AndroidJavaObject>(
                    "currentActivity");

            AndroidJavaClass downloadManagerClass =
                new AndroidJavaClass(
                    "android.app.DownloadManager");

            AndroidJavaObject downloadManager =
                activity.Call<AndroidJavaObject>(
                    "getSystemService",
                    "download");

            AndroidJavaObject uri =
                new AndroidJavaClass(
                    "android.net.Uri")
                .CallStatic<AndroidJavaObject>(
                    "parse",
                    apkUrl);

            AndroidJavaObject downloadRequest =
                new AndroidJavaObject(
                    "android.app.DownloadManager$Request",
                    uri);

            downloadRequest.Call<AndroidJavaObject>(
                "setTitle",
                "Grave Shift Update");

            downloadRequest.Call<AndroidJavaObject>(
                "setDescription",
                "Downloading Grave Shift update...");

            downloadRequest.Call<AndroidJavaObject>(
                "setMimeType",
                "application/vnd.android.package-archive");

            downloadRequest.Call<AndroidJavaObject>(
                "setNotificationVisibility",
                1); // visible notification

            long downloadId =
                downloadManager.Call<long>(
                    "enqueue",
                    downloadRequest);

            Debug.Log(
                "Download started. ID: " + downloadId);

            StartCoroutine(
                MonitorDownload(
                    downloadManager,
                    downloadId));
        }
    }

    private IEnumerator MonitorDownload(
    AndroidJavaObject downloadManager,
    long downloadId)
    {
        bool downloading = true;

        while (downloading)
        {
            yield return new WaitForSeconds(0.5f);

            AndroidJavaObject query =
                new AndroidJavaObject(
                    "android.app.DownloadManager$Query");

            query.Call<AndroidJavaObject>(
                "setFilterById",
                new object[]
                {
                    new long[] { downloadId }
                });

            AndroidJavaObject cursor =
                downloadManager.Call<AndroidJavaObject>(
                    "query",
                    query);

            if (cursor == null)
            {
                Debug.Log("DownloadManager returned null cursor.");
                continue;
            }

            bool hasRow =
                cursor.Call<bool>("moveToFirst");

            if (!hasRow)
            {
                Debug.Log("No download row found.");
                cursor.Call("close");
                continue;
            }

            int statusColumn =
                cursor.Call<int>(
                    "getColumnIndex",
                    "status");

            int status =
                cursor.Call<int>(
                    "getInt",
                    statusColumn);

            int progressColumn =
                cursor.Call<int>(
                    "getColumnIndex",
                    "bytes_so_far");

            int totalColumn =
                cursor.Call<int>(
                    "getColumnIndex",
                    "total_size");

            long downloaded =
                cursor.Call<long>(
                    "getLong",
                    progressColumn);

            long total =
                cursor.Call<long>(
                    "getLong",
                    totalColumn);

            cursor.Call("close");
            cursor.Dispose();

            Debug.Log("DownloadManager status = " + status);

            if (total > 0)
            {
                float progress =
                    (float)downloaded / total;

                updateText.text =
                    $"Downloading...\n{progress * 100f:0}%";
            }

            const int STATUS_SUCCESSFUL = 8;
            const int STATUS_FAILED = 16;

            Debug.Log("DownloadManager status = " + status);

            if (status == STATUS_SUCCESSFUL)
            {
                downloading = false;

                Debug.Log("APK download completed!");

                isCheckedForUpdate = true;

                InstallDownloadedAPK(
                    downloadManager,
                    downloadId);
            }
            else if (status == STATUS_FAILED)
            {
                downloading = false;

                Debug.LogError("APK download failed.");

                updateText.text =
                    "Download failed.";

                installButton.interactable = true;
            }
        }
    }

    private void InstallDownloadedAPK(
    AndroidJavaObject downloadManager,
    long downloadId)
    {
    Debug.Log("Opening Android installer...");
    updateText.text = "Opening installer...";

    AndroidJavaObject apkUri =
        downloadManager.Call<AndroidJavaObject>(
            "getUriForDownloadedFile",
            downloadId);

    if (apkUri == null)
    {
        Debug.LogError(
            "Could not get APK URI.");

        updateText.text =
            "Could not open installer.";

        installButton.interactable = true;

        return;
    }

    AndroidJavaObject intent =
        new AndroidJavaObject(
            "android.content.Intent");

    AndroidJavaClass intentClass =
        new AndroidJavaClass(
            "android.content.Intent");

    intent.Call<AndroidJavaObject>(
        "setAction",
        intentClass.GetStatic<string>(
            "ACTION_INSTALL_PACKAGE"));

    intent.Call<AndroidJavaObject>(
        "setDataAndType",
        apkUri,
        "application/vnd.android.package-archive");

    int FLAG_GRANT_READ_URI_PERMISSION = 1 << 0;
    int FLAG_ACTIVITY_NEW_TASK = 1 << 28;

    intent.Call<AndroidJavaObject>(
        "addFlags",
        FLAG_GRANT_READ_URI_PERMISSION);

    intent.Call<AndroidJavaObject>(
        "addFlags",
        FLAG_ACTIVITY_NEW_TASK);

    AndroidJavaClass unityPlayer =
        new AndroidJavaClass(
            "com.unity3d.player.UnityPlayer");

    AndroidJavaObject activity =
        unityPlayer.GetStatic<AndroidJavaObject>(
            "currentActivity");

    activity.Call(
        "startActivity",
        intent);

    activity.Call("finishAndRemoveTask");
    Application.Quit();
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