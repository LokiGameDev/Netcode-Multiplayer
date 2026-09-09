using TMPro;
using UnityEngine;

/// <summary>Displays a player name in the waiting-room list.</summary>
public class PlayerWaitingItem : MonoBehaviour
{
    [Tooltip("Text displaying the waiting player's name.")]
    [SerializeField] private TMP_Text playerNameText;

    /// <summary>Sets the displayed player name.</summary>
    /// <param name="name">Player name to display.</param>
    public void SetPlayerName(string name)
    {
        playerNameText.text = name;
    }
}
