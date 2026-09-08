using UnityEngine;

public class PlayerEffectsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] trailEffects;

    public void SetTrailEffectsState(bool state)
    {
        foreach(GameObject effects in trailEffects)
        {
            effects.SetActive(state);
        }
    }
}
