using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
/// <summary>Reads player input actions and exposes them as game events.</summary>
public class InputReader : ScriptableObject, IPlayerActions
{
    private Controls controls;

    /// <summary>Creates and enables the generated input actions.</summary>
    private void OnEnable()
    {
        if(controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }
        controls.Player.Enable();
    }
    /// <summary>Disables the input actions when the asset is unloaded.</summary>
    private void OnDisable()
    {
        controls.Player.Disable();
    }

    public event Action<Vector2> PlayerMovementEvent;
    public event Action PlayerJumpEvent;
    public event Action PlayerInteractEvent;
    public Vector2 movementValue;
    public Vector2 MouseInput;

    /// <summary>Publishes movement input and stores its current value.</summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        PlayerMovementEvent?.Invoke(context.ReadValue<Vector2>());
        movementValue = context.ReadValue<Vector2>();
    }

    /// <summary>Stores the current look input.</summary>
    public void OnLook(InputAction.CallbackContext context)
    {
        MouseInput = context.ReadValue<Vector2>();
    }

    /// <summary>Handles the attack action when one is configured.</summary>
    public void OnAttack(InputAction.CallbackContext context)
    {
        
    }

    /// <summary>Publishes an interaction event when the action is performed.</summary>
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.performed)
            PlayerInteractEvent?.Invoke();
    }

    /// <summary>Handles the crouch action when one is configured.</summary>
    public void OnCrouch(InputAction.CallbackContext context)
    {
        
    }

    /// <summary>Publishes a jump event when the action is performed.</summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed)
            PlayerJumpEvent?.Invoke();
    }

    /// <summary>Handles the sprint action when one is configured.</summary>
    public void OnSprint(InputAction.CallbackContext context)
    {
        
    }

    /// <summary>Sets the look input directly, such as from a touch control.</summary>
    /// <param name="lookInput">Look delta to store.</param>
    public void SetLook(Vector2 lookInput)
    {
        MouseInput = lookInput;
    }
}