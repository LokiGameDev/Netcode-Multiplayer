using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkInteractor : NetworkBehaviour, IInteractable
{
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

    public void Interact(ulong clientID)
    {
        RequestServerInteractRPC(clientID);
    }

    public Transform GetInteractionPoint()
    {
        return gameObject.transform;
    }

    public string GetActionName()
    {
        return ActionName;
    }

    public void StateChange(bool state)
    {
        if(isActivated.Value!=state) RequestServerInteractRPC();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestServerInteractRPC(ulong clientID)
    {
        isActivated.Value = !isActivated.Value;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestServerInteractRPC()
    {
        isActivated.Value = !isActivated.Value;
    }

    private void OnEnable()
    {
        ActionName = isActivated.Value ? textToDeactivate : textToActivate;
        isActivated.OnValueChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        isActivated.OnValueChanged -= OnStateChanged;
    }

    private void OnStateChanged(bool prevValue, bool currentValue)
    {
        ActionName = currentValue ? textToDeactivate : textToActivate;
        if(currentValue) OnActivated?.Invoke();
        else OnDeactivated?.Invoke();
    }
}
