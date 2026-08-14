using TMPro;
using Unity.Services.Authentication;
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
        playerNameText.text = AuthenticationService.Instance.PlayerName;
        profilePhoto.sprite = defaultProfilePhoto;
        loadingPanel.SetActive(false);
    }
}
