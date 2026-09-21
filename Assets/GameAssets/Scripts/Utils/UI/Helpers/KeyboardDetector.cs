using TMPro;
using UnityEngine;

public class KeyboardDetector : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text friendNameTextCopy;
    [SerializeField] private GameObject friendNameCopy;

    private bool keyboardWasOpen;

    void Update()
    {
        bool keyboardOpen = TouchScreenKeyboard.visible;

        // Keyboard just opened
        if (keyboardOpen && !keyboardWasOpen)
        {
            OnKeyboardOpened();
        }

        // Keyboard just closed
        if (!keyboardOpen && keyboardWasOpen)
        {
            OnKeyboardClosed();
        }

        keyboardWasOpen = keyboardOpen;

        friendNameTextCopy.text = inputField.text;
    }

    void OnKeyboardOpened()
    {
        friendNameCopy.SetActive(true);
    }

    void OnKeyboardClosed()
    {
        friendNameCopy.SetActive(false);
    }
}
