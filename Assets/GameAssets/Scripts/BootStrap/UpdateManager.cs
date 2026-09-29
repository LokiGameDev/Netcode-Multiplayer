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

    [Header("URL")]
    [SerializeField]
    private string versionJsonUrl =
        "https://raw.githubusercontent.com/LokiGameDev/Netcode-Multiplayer/main/version.json";

    [Header("Loading UI details")]
    [SerializeField] private TMP_Text loadingBarText;
    [SerializeField] private Slider loadingBarSlider;

    [Header("Permission Panel references")]
    [SerializeField] private GameObject permissionPanel;
    [SerializeField] private TMP_Text permissionText;
    [SerializeField] private Button allowButton;

    [Header("Public values")]
    private string apkUrl;
    public bool isCheckedForUpdate = false;
    private bool installPermissionRequestPending;

    public void StartChecking()
    {
        isCheckedForUpdate = false;

        #if UNITY_ANDROID && !UNITY_EDITOR
            if (!CanInstallPackages())
            {
                ShowInstallPermissionOverlay();
                return;
            }
            StartCoroutine(CheckForUpdate());
        #else
            isCheckedForUpdate = true;
        #endif
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            if (!hasFocus || !installPermissionRequestPending) return;

            installPermissionRequestPending = false;

            if (CanInstallPackages())
            {
                permissionPanel.SetActive(false);
                StartCoroutine(CheckForUpdate());
            }
            else
            {
                ShowInstallPermissionOverlay();
            }
        #endif
    }

    private void ShowInstallPermissionOverlay()
    {
        permissionPanel.SetActive(true);
        permissionText.text =
            "Permission required to install updates.\n\nTap Allow to continue.";

        allowButton.interactable = true;
        allowButton.onClick.RemoveAllListeners();
        allowButton.onClick.AddListener(RequestInstallPermission);
    }

    private void RequestInstallPermission()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            installPermissionRequestPending = true;
            permissionText.text = "Enable installation permission, then return to the game.";
            OpenInstallPermissionSettings();
        #endif
    }

    private bool CanInstallPackages()
    {
        using (AndroidJavaClass versionClass =
            new AndroidJavaClass("android.os.Build$VERSION"))
        {
            int sdkVersion = versionClass.GetStatic<int>("SDK_INT");

            if (sdkVersion < 26)
            {
                return true;
            }
        }

        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject packageManager = activity.Call<AndroidJavaObject>("getPackageManager"))
        {
            return packageManager.Call<bool>("canRequestPackageInstalls");
        }
    }

    private void OpenInstallPermissionSettings()
    {
        using (AndroidJavaClass settingsClass =
            new AndroidJavaClass("android.provider.Settings"))
        using (AndroidJavaObject intent =
            new AndroidJavaObject(
                "android.content.Intent",
                settingsClass.GetStatic<string>(
                    "ACTION_MANAGE_UNKNOWN_APP_SOURCES")))
        using (AndroidJavaClass uriClass =
            new AndroidJavaClass("android.net.Uri"))
        using (AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject activity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            AndroidJavaObject packageUri =
                uriClass.CallStatic<AndroidJavaObject>(
                    "parse",
                    "package:" + Application.identifier);

            intent.Call<AndroidJavaObject>("setData", packageUri);
            activity.Call("startActivity", intent);
        }
    }

    private IEnumerator CheckForUpdate()
    {
        Debug.Log("[UPDATE MANAGER] Checking for Grave Shift update...");

        using UnityWebRequest request = UnityWebRequest.Get(versionJsonUrl);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[UPDATE MANAGER] Update check failed: " + request.error);
            isCheckedForUpdate = true;
            yield break;
        }

        Debug.Log("[UPDATE MANAGER] Version JSON:" + request.downloadHandler.text);

        VersionData data =
            JsonUtility.FromJson<VersionData>(
                request.downloadHandler.text);

        if (data == null || string.IsNullOrEmpty(data.version) || string.IsNullOrEmpty(data.apkUrl))
        {
            Debug.LogError("[UPDATE MANAGER] Malformed version.json response.");
            isCheckedForUpdate = true;
            yield break;
        }

        string installedVersion = Application.version;

        Debug.Log("[UPDATE MANAGER] Installed: " + installedVersion + "Latest: " + data.version);

        if (IsNewerVersion(data.version, installedVersion))
        {
            apkUrl = data.apkUrl;

            loadingBarText.text = "Downloading update...";

            Debug.Log("[UPDATE MANAGER] Update available!" + data.version);

            StartUpdate();
        }
        else
        {
            Debug.Log("[UPDATE MANAGER] Grave Shift is up to date.");

            isCheckedForUpdate = true;
        }
    }

    private void StartUpdate()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            StartCoroutine(StartAndroidDownload());
        #endif
    }

    private IEnumerator StartAndroidDownload()
    {
        Debug.Log("[UPDATE MANAGER] Starting Android APK download...");

        yield return new WaitForSeconds(0.2f);

        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaClass downloadManagerClass = new AndroidJavaClass("android.app.DownloadManager");

            AndroidJavaObject downloadManager = activity.Call<AndroidJavaObject>("getSystemService", "download");

            AndroidJavaObject uri = new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("parse",apkUrl);

            AndroidJavaObject downloadRequest = new AndroidJavaObject("android.app.DownloadManager$Request",uri);

            downloadRequest.Call<AndroidJavaObject>("setTitle","Grave Shift Update");

            downloadRequest.Call<AndroidJavaObject>("setDescription","Downloading Grave Shift update...");

            downloadRequest.Call<AndroidJavaObject>("setMimeType","application/vnd.android.package-archive");

            downloadRequest.Call<AndroidJavaObject>("setNotificationVisibility",1); // visible notification

            long downloadId = downloadManager.Call<long>("enqueue", downloadRequest);

            Debug.Log("[UPDATE MANAGER] Download started. ID: " + downloadId);

            StartCoroutine(MonitorDownload(downloadManager, downloadId));
        }
    }

    private IEnumerator MonitorDownload(AndroidJavaObject downloadManager, long downloadId)
    {
        bool downloading = true;

        while (downloading)
        {
            yield return new WaitForSeconds(0.5f);

            AndroidJavaObject query = new AndroidJavaObject("android.app.DownloadManager$Query");

            query.Call<AndroidJavaObject>("setFilterById", new object[]{new long[] { downloadId }});

            AndroidJavaObject cursor = downloadManager.Call<AndroidJavaObject>("query",query);

            if (cursor == null)
            {
                Debug.Log("[UPDATE MANAGER] DownloadManager returned null cursor.");
                continue;
            }

            bool hasRow = cursor.Call<bool>("moveToFirst");

            if (!hasRow)
            {
                Debug.Log("[UPDATE MANAGER] No download row found.");
                cursor.Call("close");
                continue;
            }

            int statusColumn = cursor.Call<int>("getColumnIndex", "status");

            int status = cursor.Call<int>("getInt", statusColumn);

            int progressColumn = cursor.Call<int>("getColumnIndex", "bytes_so_far");

            int totalColumn = cursor.Call<int>("getColumnIndex", "total_size");

            long downloaded = cursor.Call<long>("getLong", progressColumn);

            long total = cursor.Call<long>("getLong", totalColumn);

            Debug.Log("[UPDATE MANAGER] DownloadManager status = " + status);

            if (total > 0)
            {
                float progress = (float)downloaded / total;
                loadingBarSlider.value = progress;
            }

            const int STATUS_SUCCESSFUL = 8;
            const int STATUS_FAILED = 16;

            Debug.Log("[UPDATE MANAGER] DownloadManager status = " + status);

            if (status == STATUS_SUCCESSFUL)
            {
                downloading = false;

                Debug.Log("[UPDATE MANAGER] APK download completed!");

                isCheckedForUpdate = true;

                InstallDownloadedAPK(downloadManager, downloadId);
            }
            else if (status == STATUS_FAILED)
            {
                int reasonColumn = cursor.Call<int>(
                    "getColumnIndex",
                    "reason"
                );

                int reason = cursor.Call<int>(
                    "getInt",
                    reasonColumn
                );  

                downloading = false;

                Debug.LogError("[UPDATE MANAGER] APK download failed.");

                Debug.LogError(
                    "[UPDATE MANAGER] APK download failed. Reason = " +
                    GetDownloadFailureReason(reason)
                );

                loadingBarText.text = "Download failed.";
            }
            
            cursor.Call("close");
            cursor.Dispose();
        }
    }

    private string GetDownloadFailureReason(int reason)
    {
        switch (reason)
        {
            case 1001:
                return "Insufficient storage";

            case 1002:
                return "File already exists";

            case 1004:
                return "HTTP data error";

            case 1005:
                return "Network error";

            case 1006:
                return "HTTP error";

            case 1007:
                return "Too many redirects";

            case 1008:
                return "Unacceptable URI";

            case 1009:
                return "Unknown HTTP error";

            case 1000:
                return "Unknown error";

            default:
                return "Unknown reason (" + reason + ")";
        }
    }

    private void InstallDownloadedAPK(AndroidJavaObject downloadManager, long downloadId)
    {
        Debug.Log("[UPDATE MANAGER] Opening Android installer...");
        loadingBarText.text = "Opening installer...";

        AndroidJavaObject apkUri = downloadManager.Call<AndroidJavaObject>("getUriForDownloadedFile",downloadId);

        if (apkUri == null)
        {
            Debug.LogError("[UPDATE MANAGER] Could not get APK URI.");

            loadingBarText.text = "Could not open installer.";

            return;
        }

        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");

        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");

        intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_INSTALL_PACKAGE"));

        intent.Call<AndroidJavaObject>("setDataAndType",apkUri,"application/vnd.android.package-archive");

        int FLAG_GRANT_READ_URI_PERMISSION = 1 << 0;
        int FLAG_ACTIVITY_NEW_TASK = 1 << 28;

        intent.Call<AndroidJavaObject>("addFlags",FLAG_GRANT_READ_URI_PERMISSION);

        intent.Call<AndroidJavaObject>("addFlags",FLAG_ACTIVITY_NEW_TASK);

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        activity.Call("startActivity",intent);

        activity.Call("finishAndRemoveTask");
        //Application.Quit();
    }

    private bool IsNewerVersion(string onlineVersion, string installedVersion)
    {
        System.Version online = new System.Version(onlineVersion);

        System.Version installed = new System.Version(installedVersion);

        return online > installed;
    }
}