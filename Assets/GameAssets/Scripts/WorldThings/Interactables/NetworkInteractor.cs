using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

/// <summary>Provides a server-authoritative networked interaction toggle.</summary>
public class NetworkInteractor : NetworkBehaviour, IInteractable
{
    /// <summary>Gets or sets the current interaction label.</summary>
    public string ActionName { get; set;} = "Interact";
    public string textToActivate;
    public string textToDeactivate;

    public NetworkVariable<bool> isActivated = new NetworkVariable<bool>
    (
        false,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server
    );

    public UnityEvent OnActivated;
    public UnityEvent OnDeactivated;

    /// <summary>Requests a state change for the interacting client.</summary>
    public void Interact(ulong clientID)
    {
        RequestServerInteractRPC(clientID);
    }

    /// <summary>Returns this object's interaction point.</summary>
    public Transform GetInteractionPoint()
    {
        return gameObject.transform;
    }

    /// <summary>Returns the current interaction label.</summary>
    public string GetActionName()
    {
        return ActionName;
    }

    /// <summary>Requests a state change when the desired state differs.</summary>
    public void StateChange(bool state)
    {
        if(isActivated.Value!=state) RequestServerInteractRPC();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    /// <summary>Toggles the interaction state on the server.</summary>
    private void RequestServerInteractRPC(ulong clientID)
    {
        isActivated.Value = !isActivated.Value;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    /// <summary>Toggles the interaction state on the server.</summary>
    private void RequestServerInteractRPC()
    {
        isActivated.Value = !isActivated.Value;
    }

    /// <summary>Subscribes to network state changes.</summary>
    private void OnEnable()
    {
        ActionName = isActivated.Value ? textToDeactivate : textToActivate;
        isActivated.OnValueChanged += OnStateChanged;
    }

    /// <summary>Unsubscribes from network state changes.</summary>
    private void OnDisable()
    {
        isActivated.OnValueChanged -= OnStateChanged;
    }

    /// <summary>Updates the action label and invokes the state event.</summary>
    private void OnStateChanged(bool prevValue, bool currentValue)
    {
        ActionName = currentValue ? textToDeactivate : textToActivate;
        if(currentValue) OnActivated?.Invoke();
        else OnDeactivated?.Invoke();
    }
}
