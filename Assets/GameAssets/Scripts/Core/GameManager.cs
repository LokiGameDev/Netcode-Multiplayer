using UnityEngine;
using UnityEngine.Events;

/// <summary>Coordinates player gem purchases and balance-change notifications.</summary>
public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    /// <summary>Gets the active game manager instance.</summary>
    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                Debug.Log("Game manager is null");
            }
            return instance;
        }
    }

    /// <summary>Registers this component as the active instance.</summary>
    private void Awake()
    {
        if(instance!=null && instance!=this) Destroy(this);

        if(instance==null) instance = this;
    }

    [Tooltip("Manager used to process gem purchases.")]
    [SerializeField] private GemsPurchaseManager gemsPurchaseManager;
    [Tooltip("UI component that displays purchased gems.")]
    [SerializeField] private GemsBoughtShower gemsBoughtShower;

    public UnityEvent PlayerGemsAmountChanged;

    /// <summary>Returns the component responsible for gem purchases.</summary>
    public GemsPurchaseManager GetGemsPurchaseManager()
    {
        return gemsPurchaseManager;
    }

    /// <summary>Notifies listeners and updates the UI after a gem purchase.</summary>
    public void PlayerBoughtGems(int amount)
    {
        PlayerGemsAmountChanged?.Invoke();
        gemsBoughtShower.PlayerBoughtGems(amount);
    }

    /// <summary>Notifies listeners that the player spent gems.</summary>
    public void PlayerSpentGems()
    {
        PlayerGemsAmountChanged?.Invoke();
    }

    /// <summary>Subtracts a gem amount and notifies listeners.</summary>
    public void PlayerSpentGems(int amount)
    {
        PlayerPrefs.SetInt("PlayerGems", PlayerPrefs.GetInt("PlayerGems") - amount);
        PlayerGemsAmountChanged?.Invoke();
    }
}
