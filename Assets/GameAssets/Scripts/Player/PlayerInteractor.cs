using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractor : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float interactionRange;
    [SerializeField] private InteractionUI interactionUI;

    private IInteractable currentInteractable;

    private ITask currentTask;

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
        if(currentInteractable!=null)
        {
            currentInteractable?.Interact(OwnerClientId);
        }
        if(currentTask!=null)
        {
            currentTask?.Interact(GetComponent<PlayerTaskManager>());
        }
    }

    private void Update()
    {
        FindInteractable();
        FindTask();

        if (currentInteractable != null)
        {
            interactionUI.Show(
                currentInteractable,
                currentInteractable.GetInteractionPoint()
            );
        }
        else
        {
            interactionUI.Hide();
        }

        if (currentTask != null)
        {
            interactionUI.Show(
                currentTask,
                currentTask.GetInteractionPoint()
            );
        }
        else
        {
            interactionUI.Hide();
        }
    }

    private void FindInteractable()
    {
        currentInteractable = null;

        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            interactionRange
        );

        foreach (Collider collider in colliders)
        {
            IInteractable interactable =
                collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                break;
            }
        }
    }

    private void FindTask()
    {
        currentTask = null;

        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            interactionRange
        );

        foreach (Collider collider in colliders)
        {
            ITask task =
                collider.GetComponent<ITask>();

            if (task != null)
            {
                currentTask = task;
                break;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
