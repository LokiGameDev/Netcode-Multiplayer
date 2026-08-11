using System.Collections;
using TMPro;
using UnityEngine;

public class DebugMessage : MonoBehaviour
{
    public static DebugMessage Instance
    {
        get; private set;
    }

    public void Awake()
    {
        if(Instance==null) Instance = this;
        if(Instance!=this) enabled = false;
    }

    [SerializeField] private TMP_Text msgBox;
    private bool isMessageShowing = false;

    void Start()
    {
        msgBox.gameObject.SetActive(false);
    }

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

    private IEnumerator FadeOutMessageBox()
    {
        yield return new WaitForSeconds(2);
        msgBox.gameObject.SetActive(false);
        isMessageShowing = false;
    }
}
