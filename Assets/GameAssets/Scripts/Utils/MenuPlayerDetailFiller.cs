using System;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Fills menu player details from the local profile.</summary>
public class MenuPlayerDetailFiller : MonoBehaviour
{
    [Header("Player Details")]
    [Tooltip("Text elements displaying the player name.")]
    [SerializeField] private TMP_Text[] playerNameText;
    [Tooltip("Text displaying the player's gem balance.")]
    [SerializeField] private TMP_Text playerGems_Text;
    [Tooltip("Image displaying the player's profile photo.")]
    [SerializeField] private Image profilePhoto;
    [Tooltip("Fallback profile photo.")]
    [SerializeField] private Sprite defaultProfilePhoto;
    [Tooltip("Optional player profile photo.")]
    [SerializeField] private Sprite playerProfilePhoto;
    [SerializeField] private TMP_Text versionText;

    [Tooltip("Panel hidden after player details load.")]
    [SerializeField] private GameObject loadingPanel;

    /// <summary>Loads profile details and subscribes to gem changes.</summary>
    private void OnEnable()
    {
        string playerName = AuthenticationService.Instance.PlayerName;
        playerGems_Text.text = PlayerPrefs.GetInt("PlayerGems").ToString();
        versionText.text = "v1.0.2";
        Array.ForEach(playerNameText, name => name.text = playerName);
        profilePhoto.sprite = defaultProfilePhoto;
        GameManager.Instance.PlayerGemsAmountChanged.AddListener(PlayerGemsChanged);
        loadingPanel.SetActive(false);
    }

    /// <summary>Unsubscribes from gem balance changes.</summary>
    private void OnDisable()
    {
        GameManager.Instance.PlayerGemsAmountChanged.RemoveListener(PlayerGemsChanged);
    }

    /// <summary>Refreshes the displayed gem balance.</summary>
    private void PlayerGemsChanged()
    {
        playerGems_Text.text = PlayerPrefs.GetInt("PlayerGems").ToString();
    }
}
