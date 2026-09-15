using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>Finds nearby interactables and forwards the player's interaction input.</summary>
public class PlayerInteractor : NetworkBehaviour
{
    [Header("Interaction")]
    [Tooltip("Input source used to trigger interactions.")]
    [SerializeField] private InputReader inputReader;
    [Tooltip("Maximum distance at which objects can be interacted with.")]
    [SerializeField] private float interactionRange;
    [Tooltip("UI shown for the nearby interaction target.")]
    [SerializeField] private InteractionUI interactionUI;
    [Tooltip("Tracks tasks assigned to this player.")]
    [SerializeField] PlayerTaskManager playerTaskManager;
    [SerializeField] PlayerManager playerManager;

    private IInteractable currentInteractable;

    private ITask currentTask;

    /// <summary>Subscribes the local player to interaction input.</summary>
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;

        playerTaskManager = GetComponent<PlayerTaskManager>();
        playerManager = GetComponent<PlayerManager>();
        inputReader.PlayerInteractEvent += PlayerInteract;
        playerManager.IsAlive.OnValueChanged += PlayerStateChanged;
    }

    /// <summary>Unsubscribes the local player from interaction input.</summary>
    public override void OnNetworkDespawn()
    {
        if(!IsOwner) return;

        inputReader.PlayerInteractEvent -= PlayerInteract;
        playerManager.IsAlive.OnValueChanged -= PlayerStateChanged;
    }

    /// <summary>Activates the current interactable or assigned task.</summary>
    private void PlayerInteract()
    {
        if(currentTask!=null)
        {
            currentTask?.Interact();
        }
        else if(currentInteractable!=null)
        {
            currentInteractable?.Interact(OwnerClientId);
        }
    }

    /// <summary>Refreshes nearby targets and interaction UI.</summary>
    private void Update()
    {
        if(!IsOwner) return;

        if(!playerManager.IsAlive.Value) return;

        FindInteractable();
        FindTask();

        if (currentInteractable != null)
        {
            interactionUI.Show(
                currentInteractable,
                currentInteractable.GetInteractionPoint()
            );
        }

        if (currentTask != null)
        {
            interactionUI.Show(
                currentTask,
                currentTask.GetInteractionPoint()
            );
        }
        
        if(currentInteractable==null && currentTask==null)
        {
            interactionUI.Hide();
        }
    }

    /// <summary>Finds the first nearby object implementing IInteractable.</summary>
    private void FindInteractable()
    {
        currentInteractable = null;

        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            interactionRange
        );

        foreach (Collider collider in colliders)
        {
            if(collider.transform.root == transform.root) continue;

            IInteractable interactable =
                collider.GetComponent<IInteractable>();

            if (interactable != null && interactable.IsInteractable)
            {
                Debug.Log("Current interactable: " + collider.gameObject.name);
                currentInteractable = interactable;
                break;
            }
        }
    }

    /// <summary>Finds the first nearby incomplete task assigned to this player.</summary>
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
                if(playerTaskManager.IsPlayerHaveThisTask(task.TaskId.Value) && !task.isCompleted.Value)
                {
                    currentTask = task;
                    break;
                }
            }
        }
    }

    private void PlayerStateChanged(bool oldValue, bool newValue)
    {
        if(playerManager.IsAlive.Value) return;

        currentInteractable = null;
        currentTask = null;
        interactionUI.Hide();
    }

    /// <summary>Draws the interaction range in the Scene view.</summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
