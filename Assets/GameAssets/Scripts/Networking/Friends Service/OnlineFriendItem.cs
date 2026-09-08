using System.Threading.Tasks;
using TMPro;
using Unity.Services.Friends.Models;
using UnityEngine;
using UnityEngine.UI;

public class OnlineFriendItem : MonoBehaviour
{
    [SerializeField] TMP_Text friendNameText;
    [SerializeField] Button inviteButton;

    private OnlineFriendsListManager manager;

    private bool isInviteSent = false;

    public Relationship relationship { get; private set; }
    private bool isChecking = false;

    public void SetUp(Relationship relationship, OnlineFriendsListManager onlineFriendsListManager)
    {
        this.relationship = relationship;
        manager = onlineFriendsListManager;
        friendNameText.text = relationship.Member.Profile.Name.Split("#")[0];
        CheckForPlayerPresence();
    }

    public void Invite()
    {
        CheckForPlayerPresence();

        if(!isInviteSent)
        {
            isInviteSent = true;
            inviteButton.interactable = false;
            manager.SendInvite(relationship);
            Debug.Log("Sent Invitation from button");
        }
    }

    public async void CheckForPlayerPresence()
    {
        Debug.Log("Checked");

        if(isChecking) return;

        isChecking = true;

        bool isExist = await manager.DoPlayerExistInLobby(relationship.Member.Id);

        if(isExist)
        {
            isInviteSent = true;
            inviteButton.interactable = false;
        }

        isChecking = false;
    }
}
