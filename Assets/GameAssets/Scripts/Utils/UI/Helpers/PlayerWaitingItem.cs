using TMPro;
using UnityEngine;

public class PlayerWaitingItem : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;

    public void SetPlayerName(string name)
    {
        playerNameText.text = name;
    }
}
