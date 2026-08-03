using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuPlayerDetailFiller : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Image profilePhoto;
    [SerializeField] private Sprite defaultProfilePhoto;
    [SerializeField] private Sprite playerProfilePhoto;

    [SerializeField] private GameObject loadingPanel;

    private void OnEnable()
    {
        playerNameText.text = PlayerPrefs.GetString("PlayerName", "No name");
        profilePhoto.sprite = defaultProfilePhoto;
        loadingPanel.SetActive(false);
    }
}
