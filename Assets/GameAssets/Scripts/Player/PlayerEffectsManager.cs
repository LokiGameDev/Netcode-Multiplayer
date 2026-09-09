using UnityEngine;

/// <summary>Controls visual effects attached to the player.</summary>
public class PlayerEffectsManager : MonoBehaviour
{
    [Tooltip("Trail effect objects toggled with the player's movement state.")]
    [SerializeField] private GameObject[] trailEffects;

    /// <summary>Shows or hides all configured trail effects.</summary>
    public void SetTrailEffectsState(bool state)
    {
        foreach(GameObject effects in trailEffects)
        {
            effects.SetActive(state);
        }
    }
}
