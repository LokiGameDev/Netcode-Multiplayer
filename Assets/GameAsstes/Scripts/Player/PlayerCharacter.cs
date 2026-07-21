using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour 
{

    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private int palyerPriorityValue = 15;

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            cinemachineCamera.Priority = palyerPriorityValue;
        }
    }
}
