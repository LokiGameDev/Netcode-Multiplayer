using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Enables a button when its input field contains text.</summary>
public class InputFieldButtonEffects : MonoBehaviour
{
    [Tooltip("Input field checked for text.")]
    [SerializeField] private TMP_InputField joinCodeInputField;
    [Tooltip("Button enabled when the input is non-empty.")]
    [SerializeField] private Button button;

    /// <summary>Initializes the button state.</summary>
    private void Start()
    {
        CheckForButton();
    }

    /// <summary>Updates the button interactable state from the input length.</summary>
    public void CheckForButton()
    {
        int length = joinCodeInputField.text.Length;

        if(length <= 0) button.interactable = false;
        else button.interactable = true;
    }
}
