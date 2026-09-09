using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
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

    private void Awake()
    {
        if(instance!=null && instance!=this) Destroy(this);

        if(instance==null) instance = this;
    }

    [SerializeField] private GemsPurchaseManager gemsPurchaseManager;
    [SerializeField] private GemsBoughtShower gemsBoughtShower;

    public UnityEvent PlayerGemsAmountChanged;

    public GemsPurchaseManager GetGemsPurchaseManager()
    {
        return gemsPurchaseManager;
    }

    public void PlayerBoughtGems(int amount)
    {
        PlayerGemsAmountChanged?.Invoke();
        gemsBoughtShower.PlayerBoughtGems(amount);
    }

    public void PlayerSpentGems()
    {
        PlayerGemsAmountChanged?.Invoke();
    }

    public void PlayerSpentGems(int amount)
    {
        PlayerPrefs.SetInt("PlayerGems", PlayerPrefs.GetInt("PlayerGems") - amount);
        PlayerGemsAmountChanged?.Invoke();
    }
}
