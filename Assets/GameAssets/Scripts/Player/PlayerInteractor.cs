using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractor : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float radius;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;

        inputReader.PlayerInteractEvent += PlayerInteract;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) return;

        inputReader.PlayerInteractEvent -= PlayerInteract;
    }

    private void PlayerInteract()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach(var hit in colliders)
        {
            if(hit.TryGetComponent(out IInteractable component))
            {
                component?.Interact(OwnerClientId);
                break;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
