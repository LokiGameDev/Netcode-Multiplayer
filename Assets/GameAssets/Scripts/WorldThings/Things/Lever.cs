using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Lever : NetworkBehaviour, IInteractable
{
    public string Prompt { get; set;} = "Press E to Activate";
    public NetworkVariable<bool> isActivated = new NetworkVariable<bool>
    (
        false,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server
    );

    [SerializeField] private GameObject leverHandle;
    [SerializeField] private float leverActivationAngle = 45f;
    [SerializeField] private float rotationSpeed = 180;
    [SerializeField] private TMP_Text promptText;

    public UnityEvent OnLeverActivated;
    public UnityEvent OnLeverDeactivated;

    private Coroutine rotateRoutine;

    public void Interact(ulong clientID)
    {
        RequestServerInteractRPC(clientID);
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
        isActivated.OnValueChanged += OnLeverStateChanged;

        if(rotateRoutine!=null) StopCoroutine(rotateRoutine);

        rotateRoutine = StartCoroutine(RotateRoutine(Quaternion.Euler(isActivated.Value ? leverActivationAngle : leverActivationAngle*-1, 0, 0)));
    }

    private void OnDisable()
    {
        isActivated.OnValueChanged -= OnLeverStateChanged;
    }

    private void OnLeverStateChanged(bool prevValue, bool currentValue)
    {
        Prompt = isActivated.Value ? "Press E to Deactivate" : "Press E to Activate";

        if(rotateRoutine!=null) StopCoroutine(rotateRoutine);

        rotateRoutine = StartCoroutine(RotateRoutine(Quaternion.Euler(currentValue ? leverActivationAngle : leverActivationAngle*-1, 0, 0)));
    }

    private IEnumerator RotateRoutine(Quaternion targetRotation)
    {
        while (Quaternion.Angle(leverHandle.transform.localRotation, targetRotation) > 0.1f)
        {
            leverHandle.transform.localRotation = Quaternion.RotateTowards(
                leverHandle.transform.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);

            yield return null;
        }

        leverHandle.transform.localRotation = targetRotation;
        rotateRoutine = null;
        if(isActivated.Value) OnLeverActivated?.Invoke();
        else OnLeverDeactivated?.Invoke();
    }

    public void ShowPromptText(bool state)
    {
        promptText.text = Prompt;
        promptText.gameObject.SetActive(state);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            promptText.text = Prompt;
            promptText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            promptText.text = Prompt;
            promptText.gameObject.SetActive(false);
        }
    }
}
