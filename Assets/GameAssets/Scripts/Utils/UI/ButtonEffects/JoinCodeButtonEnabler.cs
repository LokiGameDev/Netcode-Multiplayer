using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Enables the join button when a join code is entered.</summary>
public class JoinCodeButtonEnabler : MonoBehaviour
{
    [Tooltip("Input field containing the join code.")]
    [SerializeField] private TMP_InputField joinCodeInputField;
    [Tooltip("Button enabled when the join code is non-empty.")]
    [SerializeField] private Button button;

    /// <summary>Initializes the button state.</summary>
    private void Start()
    {
        CheckForButton();
    }

    /// <summary>Updates the button interactable state from the code length.</summary>
    public void CheckForButton()
    {
        int length = joinCodeInputField.text.Length;

        if(length <= 0) button.interactable = false;
        else button.interactable = true;
    }
}
