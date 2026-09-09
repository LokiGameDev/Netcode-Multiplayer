using TMPro;
using UnityEngine;

/// <summary>Manages frame-rate limits and optional FPS diagnostics.</summary>
public class QualityManager : MonoBehaviour
{
    private static QualityManager instance;
    /// <summary>Gets the active quality manager instance.</summary>
    public static QualityManager Instance
    {
        get
        {
            if(instance==null)
            {
                Debug.LogError("Quality manager is null");
            }
            return instance;
        }
    }

    [Tooltip("Text element used to display the current FPS.")]
    [SerializeField] private TMP_Text fpsText;
    [Tooltip("Object containing the FPS display.")]
    [SerializeField] private GameObject fpsObject;

    [Tooltip("Whether the FPS display is enabled.")]
    [SerializeField] private bool debugFPS = false;

    public int maxDeviceFPS = 0;
    public int currentMaxFPSSelected = 0;

    private float currentFPS = 0;

    private float timer;
    private int frames;


    /// <summary>Updates the optional FPS display.</summary>
    private void Update()
    {
        ShowFPSText(debugFPS);

        if(!debugFPS) return;

        frames++;
        timer += Time.unscaledDeltaTime;

        if (timer >= 0.5f)
        {
            currentFPS = frames / timer;

            frames = 0;
            timer = 0f;

            //fpsText.text = $"{currentFPS}FPS / {currentMaxFPSSelected}\nMaxDevice: {maxDeviceFPS}";
            fpsText.text = $"{(int)currentFPS} FPS";
        }
    }

    /// <summary>Initializes the singleton and default menu frame rate.</summary>
    private void Awake()
    {
        if(instance!=null && instance!=this) Destroy(this);

        if(instance==null)
        {
            instance=this;
            DontDestroyOnLoad(this);
        }

        SetGameFPS(CurrentGameScene.Menu);

        if(!debugFPS)
        {
            ShowFPSText(false);
            return;
        }

        ShowFPSText(true);
        
        maxDeviceFPS = (int)Screen.currentResolution.refreshRateRatio.value;

        //fpsText.text = $"{currentFPS}FPS / {currentMaxFPSSelected}\nMaxDevice: {maxDeviceFPS}";
        fpsText.text = $"{currentFPS} FPS";
    }

    /// <summary>Sets the target frame rate for the current scene type.</summary>
    public void SetGameFPS(CurrentGameScene currentGameScene)
    {
        switch(currentGameScene)
        {
            case CurrentGameScene.Menu:
                Application.targetFrameRate = 30;
                currentMaxFPSSelected = 30;
                break;
            case CurrentGameScene.Game:
                Application.targetFrameRate = 60;
                currentMaxFPSSelected = 60;
                break;
        }
    }

    /// <summary>Shows or hides the FPS display.</summary>
    public void ShowFPSText(bool state)
    {
        fpsObject.SetActive(state);
    }
}

public enum CurrentGameScene
{
    Menu,
    Game
}