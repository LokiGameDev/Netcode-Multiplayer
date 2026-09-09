using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>Selects a button when its object becomes enabled.</summary>
public class ButtonSelectorOnEnable : MonoBehaviour
{
    [Tooltip("Button selected when this object is enabled.")]
    [SerializeField] private Button button;
    /// <summary>Selects the configured button in the current event system.</summary>
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(button.gameObject);  
    }
}
