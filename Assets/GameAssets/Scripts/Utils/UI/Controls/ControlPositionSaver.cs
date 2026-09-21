using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlPositionSaver : MonoBehaviour
{
    [SerializeField] private RectTransform moveRectTransform;
    [SerializeField] private RectTransform jumpRectTransform;
    [SerializeField] private RectTransform interactTransform;

    [SerializeField] private GameObject playingGamePanel;
    [SerializeField] private GameObject changingGamePanel;

    [SerializeField] private MobileControlEnabler mobileControlEnabler;

    string filePath;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "positions.json");
    }

    private void Start()
    {
        CheckPositions();
    }

    private void SavePositions(Vector2 pos1, Vector2 pos2, Vector2 pos3)
    {
        ControlSaveData data = new ControlSaveData
        {
            movePosition = new PositionData(pos1),
            jumpPosition = new PositionData(pos2),
            interactPosition = new PositionData(pos3)
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);

        Debug.Log("Saved to: " + filePath);

        mobileControlEnabler.PositionsChanged();
    }

    public ControlSaveData LoadPositions()
    {
        if (!File.Exists(filePath))
            return null;

        string json = File.ReadAllText(filePath);

        return JsonUtility.FromJson<ControlSaveData>(json);
    }

    public void SaveCurrentPositions()
    {
        SavePositions(moveRectTransform.position, jumpRectTransform.position, interactTransform.position);
    }

    public void ResetToDefaultPositions()
    {
        moveRectTransform.localPosition = new Vector2(-500, -180);
        jumpRectTransform.localPosition = new Vector2(705, 0);
        interactTransform.localPosition = new Vector2(540, -200);
    }

    private void CheckPositions()
    {
        ControlSaveData controlSaveData = LoadPositions();

        moveRectTransform.position = controlSaveData.movePosition.ToVector2();
        jumpRectTransform.position = controlSaveData.jumpPosition.ToVector2();
        interactTransform.position = controlSaveData.interactPosition.ToVector2();
    }

    public void ChangeToChangeMode()
    {
        playingGamePanel.SetActive(false);
        changingGamePanel.SetActive(true);
        CheckPositions();
    }

    public void ChangeToPlayTestMode()
    {
        playingGamePanel.SetActive(true);
        changingGamePanel.SetActive(false);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

[System.Serializable]
public class PositionData
{
    public float x;
    public float y;

    public PositionData(Vector2 position)
    {
        x = position.x;
        y = position.y;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
}

[System.Serializable]
public class ControlSaveData
{
    public PositionData movePosition;
    public PositionData jumpPosition;
    public PositionData interactPosition;
}