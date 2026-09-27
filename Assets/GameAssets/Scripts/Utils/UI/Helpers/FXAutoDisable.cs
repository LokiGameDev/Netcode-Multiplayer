using UnityEngine;

public class FXAutoDisable : MonoBehaviour
{
    [SerializeField] private float duration = 2f;

    private void OnEnable()
    {
        Invoke(nameof(DisableFX), duration);
    }

    private void DisableFX()
    {
        gameObject.SetActive(false);
    }
}