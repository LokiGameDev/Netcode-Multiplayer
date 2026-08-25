using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerNameText;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            UserData data = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            PlayerName.Value = data.userName;
            playerNameText.text = PlayerName.Value.ToString();
        }
    }

    public override void OnNetworkDespawn()
    {
        
    }
}
