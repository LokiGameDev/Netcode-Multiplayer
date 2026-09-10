using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>Synchronizes the player's display name and skin.</summary>
public class PlayerManager : NetworkBehaviour
{
    [Header("Player Identity")]
    [Tooltip("Text displaying the player's name.")]
    [SerializeField] private TMP_Text playerNameText;
    [Tooltip("Applies the player's selected skin.")]
    [SerializeField] private PlayerSkinManager playerSkinManager;
    [SerializeField] private PlayerAnimationManager playerAnimationManager;

    public NetworkVariable<bool> IsAlive = new();

    public NetworkVariable<FixedString32Bytes> PlayerName = new();

    /// <summary>Loads the owner's profile data and subscribes to name changes.</summary>
    public override void OnNetworkSpawn()
    {
        PlayerName.OnValueChanged += PlayerNameChanged;

        IsAlive.Value = true;

        if(IsServer)
        {
            UserData data = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            PlayerName.Value = data.userName;
            playerNameText.text = PlayerName.Value.ToString();
            playerSkinManager.Initialize(data.skinID);
        }

        PlayerNameChanged(PlayerName.Value, PlayerName.Value);
    }

    /// <summary>Unsubscribes from synchronized name changes.</summary>
    public override void OnNetworkDespawn()
    {
        PlayerName.OnValueChanged -= PlayerNameChanged;
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void PlayerGotAttackedRpc(RpcParams rpcParams = default)
    {
        if(!IsAlive.Value) return;

        playerNameText.gameObject.SetActive(false);
        IsAlive.Value = false;
        playerAnimationManager.PlayerStateChange(PlayerState.Dead);
    }

    /// <summary>Updates the displayed player name.</summary>
    private void PlayerNameChanged(FixedString32Bytes a, FixedString32Bytes b)
    {
        playerNameText.text = PlayerName.Value.ToString();
    }
}
