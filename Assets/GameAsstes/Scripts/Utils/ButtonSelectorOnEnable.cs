using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSelectorOnEnable : MonoBehaviour
{
    [SerializeField] private Button button;
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(button.gameObject);  
    }
}
