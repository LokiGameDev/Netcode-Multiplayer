using System;
using System.IO;
using UnityEngine;

/// <summary>Shows mobile controls only on mobile platforms.</summary>
public class MobileControlEnabler : MonoBehaviour
{
    [Tooltip("Mobile controls object shown on supported platforms.")]
    [SerializeField] private GameObject mobileControls;

    [SerializeField] private RectTransform moveRectTransform;
    [SerializeField] private RectTransform jumpRectTransform;
    [SerializeField] private RectTransform interactTransform;

    string filePath;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "positions.json");
    }

    /// <summary>Applies the platform-specific mobile control visibility.</summary>
    private void OnEnable()
    {
        #if UNITY_EDITOR
            mobileControls.SetActive(true);
        #else
            mobileControls.SetActive(Application.isMobilePlatform);
        #endif
    }

    private void Start()
    {
        CheckPositions();
    }

    public void PlayerStateChanged(bool state)
    {
        if(!Application.isMobilePlatform) return;

        mobileControls.SetActive(state);
    }

    public ControlSaveData LoadPositions()
    {
        if (!File.Exists(filePath))
            return null;

        string json = File.ReadAllText(filePath);

        return JsonUtility.FromJson<ControlSaveData>(json);
    }

    public void PositionsChanged()
    {
        CheckPositions();
    }

    private void CheckPositions()
    {
        ControlSaveData controlSaveData = LoadPositions();

        moveRectTransform.position = controlSaveData.movePosition.ToVector2();
        jumpRectTransform.position = controlSaveData.jumpPosition.ToVector2();
        interactTransform.position = controlSaveData.interactPosition.ToVector2();
    }
}
