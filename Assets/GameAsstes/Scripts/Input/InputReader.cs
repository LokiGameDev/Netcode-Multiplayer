using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    private Controls controls;

    void OnEnable()
    {
        if(controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }
        controls.Player.Enable();
    }
    void OnDisable()
    {
        controls.Player.Disable();
    }

    public event Action<Vector2> PlayerMovementEvent;
    public event Action PlayerJumpEvent;
    public Vector2 movementValue;


    public void OnMove(InputAction.CallbackContext context)
    {
        PlayerMovementEvent?.Invoke(context.ReadValue<Vector2>());
        movementValue = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed)
            PlayerJumpEvent?.Invoke();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        
    }
}