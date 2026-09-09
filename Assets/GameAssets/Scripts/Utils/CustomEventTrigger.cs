using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>Activates a target object based on a toggle state.</summary>
public class CustomEventTrigger : MonoBehaviour
{
    [Tooltip("Toggle that controls the target object's state.")]
    public Toggle toggle;
    [Tooltip("Object whose active state is controlled.")]
    public GameObject target;

    /// <summary>Updates the target active state from the toggle.</summary>
    public void SetObjectActive()
    {
        target.SetActive(!toggle.isOn);
    }
}