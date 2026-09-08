using TMPro;
using UnityEngine;


public class QualityManager : MonoBehaviour
{
    private static QualityManager instance;
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

    [SerializeField] private TMP_Text fpsText;
    [SerializeField] private GameObject fpsObject;

    [SerializeField] private bool debugFPS = false;

    public int maxDeviceFPS = 0;
    public int currentMaxFPSSelected = 0;

    private float currentFPS = 0;

    private float timer;
    private int frames;


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