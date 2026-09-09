using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Selects one button from a configured group.</summary>
public class ButtonGroupSelector : MonoBehaviour
{
    [Tooltip("Buttons available for selection.")]
    [SerializeField] private Button[] groupOfButtons;

    private int selectedIndex = 0;

    /// <summary>Selects the first button when the group starts.</summary>
    private void Start()
    {
        SelectButton(0);
    }

    /// <summary>Selects a button by its array index.</summary>
    /// <param name="index">Index of the button to select.</param>
    public void SelectButton(int index)
    {
        selectedIndex = index;

        for(int i = 0; i < groupOfButtons.Length; i++)
        {
            bool select = i == selectedIndex;

            if(select) EventSystem.current.SetSelectedGameObject(groupOfButtons[i].gameObject);  
        }
    }
}
