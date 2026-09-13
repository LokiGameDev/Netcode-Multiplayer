using System;
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
    [SerializeField] private Rigidbody playerRigidBody;
    [SerializeField] private PlayerReviver playerReviver;

    public NetworkVariable<bool> IsAlive = new(
        true,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner
    );

    public NetworkVariable<FixedString32Bytes> PlayerName = new();

    /// <summary>Loads the owner's profile data and subscribes to name changes.</summary>
    public override void OnNetworkSpawn()
    {
        PlayerName.OnValueChanged += PlayerNameChanged;
        IsAlive.OnValueChanged += PlayerStateChanged;
        
        if(IsOwner) IsAlive.Value = true;

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

    private void Start()
    {
        playerNameText.gameObject.SetActive(IsAlive.Value);
        playerReviver.InteractStateChange(!IsAlive.Value);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void PlayerGotAttackedRpc(RpcParams rpcParams = default)
    {
        if(!IsAlive.Value) return;

        Debug.Log("[Cleint] Player got killed");
        playerNameText.gameObject.SetActive(false);
        IsAlive.Value = false;
        playerAnimationManager.PlayerStateChange(PlayerState.Dead);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void PlayerGotRevivedRpc(RpcParams rpcParams = default)
    {
        Debug.Log("[Cleint] Player received revive call");
        
        if(IsAlive.Value) return;

        Debug.Log("[Cleint] Player got revived");

        playerNameText.gameObject.SetActive(true);
        IsAlive.Value = true;
        playerAnimationManager.PlayerStateChange(PlayerState.Idle);
    }

    private void PlayerStateChanged(bool previousValue, bool newValue)
    {
        playerNameText.gameObject.SetActive(IsAlive.Value);
        playerReviver.InteractStateChange(!IsAlive.Value);
        //playerRigidBody.isKinematic = IsAlive.Value;
    }

    public void PlayerGotRevived()
    {
        Debug.Log("[Client] Player got revivied");
        GameStateManager.Instance.PlayerGotRevivedRpc(GetComponent<NetworkObject>().OwnerClientId);
    }

    /// <summary>Updates the displayed player name.</summary>
    private void PlayerNameChanged(FixedString32Bytes a, FixedString32Bytes b)
    {
        playerNameText.text = PlayerName.Value.ToString();
    }
}
