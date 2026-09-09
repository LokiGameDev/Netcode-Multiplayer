using UnityEngine;

/// <summary>Defines an object that can be activated by a network player.</summary>
public interface IInteractable
{
    /// <summary>Gets or sets the action label shown to the player.</summary>
    public string ActionName { get; set; }

    /// <summary>Returns the current interaction label.</summary>
    public string GetActionName();
    /// <summary>Returns the world point used for interaction.</summary>
    public Transform GetInteractionPoint();
    /// <summary>Requests interaction for a client.</summary>
    public void Interact(ulong clientID);
}
