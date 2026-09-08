using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private PlayerSkinManager playerSkinManager;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        PlayerName.OnValueChanged += PlayerNameChanged;

        if(IsServer)
        {
            UserData data = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            PlayerName.Value = data.userName;
            playerNameText.text = PlayerName.Value.ToString();
            playerSkinManager.Initialize(data.skinID);
        }

        PlayerNameChanged(PlayerName.Value, PlayerName.Value);
    }

    public override void OnNetworkDespawn()
    {
        PlayerName.OnValueChanged -= PlayerNameChanged;
    }

    private void PlayerNameChanged(FixedString32Bytes a, FixedString32Bytes b)
    {
        playerNameText.text = PlayerName.Value.ToString();
    }
}
