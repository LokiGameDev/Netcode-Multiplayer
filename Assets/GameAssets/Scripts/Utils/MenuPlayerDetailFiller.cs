using System;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class MenuPlayerDetailFiller : MonoBehaviour
{
    [SerializeField] private TMP_Text[] playerNameText;
    [SerializeField] private TMP_Text playerGems_Text;
    [SerializeField] private Image profilePhoto;
    [SerializeField] private Sprite defaultProfilePhoto;
    [SerializeField] private Sprite playerProfilePhoto;

    [SerializeField] private GameObject loadingPanel;

    private void OnEnable()
    {
        string playerName = AuthenticationService.Instance.PlayerName;
        playerGems_Text.text = PlayerPrefs.GetInt("PlayerGems").ToString();
        Array.ForEach(playerNameText, name => name.text = playerName);
        profilePhoto.sprite = defaultProfilePhoto;
        GameManager.Instance.PlayerGemsAmountChanged.AddListener(PlayerGemsChanged);
        loadingPanel.SetActive(false);
    }

    private void OnDisable()
    {
        GameManager.Instance.PlayerGemsAmountChanged.RemoveListener(PlayerGemsChanged);
    }

    private void PlayerGemsChanged()
    {
        playerGems_Text.text = PlayerPrefs.GetInt("PlayerGems").ToString();
    }
}
