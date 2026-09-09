using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>Displays temporary debug messages in a UI text element.</summary>
public class DebugMessage : MonoBehaviour
{
    /// <summary>Gets the active debug-message instance.</summary>
    public static DebugMessage Instance
    {
        get; private set;
    }

    /// <summary>Registers this component as the active instance.</summary>
    public void Awake()
    {
        if(Instance==null) Instance = this;
        if(Instance!=this) enabled = false;
    }

    [Tooltip("Text element used to display debug messages.")]
    [SerializeField] private TMP_Text msgBox;
    private bool isMessageShowing = false;

    /// <summary>Hides the message box when the component starts.</summary>
    void Start()
    {
        msgBox.gameObject.SetActive(false);
    }

    /// <summary>Displays a message and starts its fade-out timer.</summary>
    public void Display(string msg)
    {
        if(isMessageShowing)
        {
            msgBox.text = msg;
        }
        else
        {
            msgBox.gameObject.SetActive(true);
            msgBox.text = msg;
            isMessageShowing = true;
            StartCoroutine(FadeOutMessageBox());
        }
    }

    /// <summary>Waits briefly before hiding the message box.</summary>
    private IEnumerator FadeOutMessageBox()
    {
        yield return new WaitForSeconds(2);
        msgBox.gameObject.SetActive(false);
        isMessageShowing = false;
    }
}
