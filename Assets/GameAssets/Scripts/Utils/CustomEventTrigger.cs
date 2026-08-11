using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomEventTrigger : MonoBehaviour
{
    public Toggle toggle;
    public GameObject target;

    public void SetObjectActive()
    {
        target.SetActive(!toggle.isOn);
    }
}