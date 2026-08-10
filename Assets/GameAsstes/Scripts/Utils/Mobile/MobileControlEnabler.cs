using System;
using UnityEngine;

public class MobileControlEnabler : MonoBehaviour
{
    [SerializeField] private GameObject mobileControls;
    private void OnEnable()
    {
        mobileControls.SetActive(Application.isMobilePlatform);
    }
}
