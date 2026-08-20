using UnityEngine;

public interface IInteractable
{
    public string ActionName { get; set; }

    public string GetActionName();
    public Transform GetInteractionPoint();
    public void Interact(ulong clientID);
}
