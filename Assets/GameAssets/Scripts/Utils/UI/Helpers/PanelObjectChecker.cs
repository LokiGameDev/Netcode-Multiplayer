using UnityEngine;
using System;

/// <summary>Sets configured objects active or inactive when enabled.</summary>
public class PanelObjectChecker : MonoBehaviour
{
    [Tooltip("Objects enabled when this component is enabled.")]
    [SerializeField] private GameObject[] enableOnStart;
    [Tooltip("Objects disabled when this component is enabled.")]
    [SerializeField] private GameObject[] disbleOnStart;

    /// <summary>Applies the configured active states.</summary>
    private void OnEnable()
    {
        Array.ForEach(enableOnStart, obj => obj.SetActive(true));
        Array.ForEach(disbleOnStart, obj => obj.SetActive(false));
    }
}
