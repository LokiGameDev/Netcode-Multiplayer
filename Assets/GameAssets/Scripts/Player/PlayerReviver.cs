using Unity.Netcode;
using UnityEngine;

public class PlayerReviver : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameObject playerHealUI;
    /// <summary>Gets or sets the player-facing action label.</summary>
    public string ActionName { get; set; } = "Revive";
    public bool IsInteractable { get; set; } = false;

    public string GetActionName()
    {
        return ActionName;
    }

    public Transform GetInteractionPoint()
    {
        return gameObject.transform;
    }

    public void Interact(ulong clientID)
    {
        Debug.Log("Interacted");

        IsInteractable = false;

        playerManager.PlayerGotRevived();
    }

    public void InteractStateChange(bool state)
    {
        IsInteractable = state;
        playerHealUI.SetActive(state);
    }
}
