using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Displays one assigned task and its current selection state.</summary>
public class PlayerUIListItem : MonoBehaviour
{
    [Header("Player Display")]
    [Tooltip("Text displaying the player name.")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private RectTransform strikeLine;
    [SerializeField] private Color aliveColor;
    [Tooltip("Text color used after the player is dead.")]
    [SerializeField] private Color deadColor;
    [Tooltip("Icon showing the player alive state.")]
    [SerializeField] private Image playerIconImage;
    [Tooltip("Border shown around the owner player.")]
    [SerializeField] private GameObject ownerBorder;

    [SerializeField] private Sprite playerAliveIcon;
    [SerializeField] private Sprite playerDeadIcon;

    [SerializeField] private float strikeDuration = 1f;

    public string PlayerName = "";

    /// <summary>Resets the item to its initial unselected state.</summary>
    private void Start()
    {
        
    }

    /// <summary>Initializes the task text, ID, and owning UI manager.</summary>
    /// <param name="name">Player-facing task name.</param>
    /// <param name="taskID">Unique task identifier.</param>
    /// <param name="uITaskManager">Manager that owns this task item.</param>
    public void Initialize(string name, bool isOwner)
    {
        PlayerName = name;
        playerNameText.text = name;
        playerNameText.color = aliveColor;
        ownerBorder.SetActive(isOwner);
    }

    /// <summary>Marks the task as completed in the UI.</summary>
    public void PlayerStateChanged(bool state, bool isOwner)
    {
        playerIconImage.sprite = state ? playerAliveIcon : playerDeadIcon;
        playerNameText.color = state ? aliveColor : deadColor;
        if(isOwner) ownerBorder.GetComponent<Image>().color = state ? aliveColor : deadColor;
        AnimateStrike(state);
    }

    public void PlayerWentOffline()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator AnimateStrike(bool state)
    {
        float time = 0f;

        strikeLine.localScale = new Vector3(state ? 1 : 0, 1f, 1f);

        while (time < strikeDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / strikeDuration);
            t = Mathf.SmoothStep(state ? 1 : 0, state ? 0 : 1, t);

            strikeLine.localScale = new Vector3(t, 1f, 1f);

            yield return null;
        }

        strikeLine.localScale = state ? Vector3.zero : Vector3.one;
    }
}
