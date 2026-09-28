using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class SpectateCamera : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform currentPlayerTarget;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private CinemachineCamera cameraObject;
    [SerializeField] private TMP_Text playerNameText;
    
    [Header("Look Settings")]
    [Tooltip("Sensitivity applied to look input.")]
    [SerializeField] private float lookSensitivity = 10f;
    [Tooltip("Minimum vertical camera angle.")]
    [SerializeField] private float minPitch = -30f;
    [Tooltip("Maximum vertical camera angle.")]
    [SerializeField] private float maxPitch = 10f;

    private float yaw;
    private float pitch;

    private int currentIndex;
    private List<Transform> targets = new();

    public void SetTargets(List<Transform> playerTargets)
    {
        targets = playerTargets;
        currentIndex = 0;
    }

    private void Start()
    {
        cameraObject.Priority = 0;
    }

    public void StartSpectating()
    {
        currentIndex = 0;
        cameraObject.Priority = 30;
        Transform localPlayer = GameStateManager.Instance.GetLocalPlayerDetails().transform;

        foreach(var player in targets)
        {
            if(player == localPlayer)
            {
                currentPlayerTarget = targets[currentIndex];
                playerNameText.text = currentPlayerTarget.GetComponent<PlayerManager>().PlayerName.Value.ToString();
                break;
            }
            currentIndex++;
        }

    }

    public void StopSpectating()
    {
        currentIndex = 0;
        cameraObject.Priority = 0;
    }

    public void SpectateNext()
    {
        if (targets.Count == 0)
            return;

        currentIndex++;

        if (currentIndex >= targets.Count)
            currentIndex = 0;

        currentPlayerTarget = targets[currentIndex];
        playerNameText.text = currentPlayerTarget.GetComponent<PlayerManager>().PlayerName.Value.ToString();
    }

    public void SpectatePrevious()
    {
        if (targets.Count == 0)
            return;

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = targets.Count - 1;

        currentPlayerTarget = targets[currentIndex];
    }

    /// <summary>Updates the local camera pivot from look input.</summary>
    private void LateUpdate()
    {
        if(currentPlayerTarget==null) return;

        yaw += inputReader.MouseInput.x * lookSensitivity * Time.deltaTime;
        pitch -= inputReader.MouseInput.y * lookSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraPivot.rotation = Quaternion.Euler(pitch, yaw, 0f);

        cameraPivot.position = currentPlayerTarget.position;
    }
}
