using System;
using UnityEngine;

/// <summary>Shows mobile controls only on mobile platforms.</summary>
public class MobileControlEnabler : MonoBehaviour
{
    [Tooltip("Mobile controls object shown on supported platforms.")]
    [SerializeField] private GameObject mobileControls;
    /// <summary>Applies the platform-specific mobile control visibility.</summary>
    private void OnEnable()
    {
        mobileControls.SetActive(Application.isMobilePlatform);
    }

    public void PlayerStateChanged(bool state)
    {
        if(!Application.isMobilePlatform) return;

        mobileControls.SetActive(state);
    }
}
