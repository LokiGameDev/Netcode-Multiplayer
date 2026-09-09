using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

/// <summary>Configures the local player's networked camera priority.</summary>
public class PlayerCharacter : NetworkBehaviour 
{

    [Tooltip("Camera controlled by the local player.")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [Tooltip("Priority assigned to the local player's camera.")]
    [SerializeField] private int palyerPriorityValue = 15;

    /// <summary>Assigns camera priority when this object spawns for its owner.</summary>
    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            cinemachineCamera.Priority = palyerPriorityValue;
        }
    }
}
