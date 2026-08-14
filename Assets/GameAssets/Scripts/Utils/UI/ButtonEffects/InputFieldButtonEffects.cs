using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldButtonEffects : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private Button button;

    private void Start()
    {
        CheckForButton();
    }

    public void CheckForButton()
    {
        int length = joinCodeInputField.text.Length;

        if(length <= 0) button.interactable = false;
        else button.interactable = true;
    }
}
