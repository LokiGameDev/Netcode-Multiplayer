using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOfflineManager : MonoBehaviour
{
    [Header("Player Identity")]
    [Tooltip("Text displaying the player's name.")]
    [SerializeField] private TMP_Text playerNameText;
    [Tooltip("Applies the player's selected skin.")]
    [SerializeField] private PlayerOfflineSkinManager playerSkinManager;
    [SerializeField] private GameObject footCircleEffect;
    [SerializeField] private CinemachineBasicMultiChannelPerlin cameraNoise;

    [SerializeField] private Color ownerFootColor = new Color32(0,0,0,25);

    public bool IsAlive = true;

    public string PlayerName = "Player";

    /// <summary>Loads the owner's profile data and subscribes to name changes.</summary>
    public void OnEnable()
    {
        playerNameText.text = PlayerName;
        playerSkinManager.Initialize(PlayerPrefs.GetString("PlayerSkinID", "Default"));
    }

    private void Start()
    {
        playerNameText.gameObject.SetActive(IsAlive);
        footCircleEffect.GetComponent<Image>().color = ownerFootColor;
    }
}
