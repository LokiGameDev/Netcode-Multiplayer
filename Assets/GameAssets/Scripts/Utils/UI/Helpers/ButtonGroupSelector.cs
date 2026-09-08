using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonGroupSelector : MonoBehaviour
{
    [SerializeField] private Button[] groupOfButtons;

    private int selectedIndex = 0;

    private void Start()
    {
        SelectButton(0);
    }

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
