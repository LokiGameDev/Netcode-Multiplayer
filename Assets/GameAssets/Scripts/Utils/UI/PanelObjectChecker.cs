using UnityEngine;
using System;

public class PanelObjectChecker : MonoBehaviour
{
    [SerializeField] private GameObject[] enableOnStart;
    [SerializeField] private GameObject[] disbleOnStart;

    private void OnEnable()
    {
        Array.ForEach(enableOnStart, obj => obj.SetActive(true));
        Array.ForEach(disbleOnStart, obj => obj.SetActive(false));
    }
}
