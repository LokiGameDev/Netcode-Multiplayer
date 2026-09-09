using System.Threading.Tasks;
using TMPro;
using Unity.Services.Friends.Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Displays an online friend and sends lobby invitations.</summary>
public class OnlineFriendItem : MonoBehaviour
{
    [Tooltip("Text displaying the friend's name.")]
    [SerializeField] TMP_Text friendNameText;
    [Tooltip("Button used to invite the friend.")]
    [SerializeField] Button inviteButton;

    private OnlineFriendsListManager manager;

    private bool isInviteSent = false;

    /// <summary>Gets the relationship represented by this item.</summary>
    public Relationship relationship { get; private set; }
    private bool isChecking = false;

    /// <summary>Initializes the item and checks the friend's lobby presence.</summary>
    public void SetUp(Relationship relationship, OnlineFriendsListManager onlineFriendsListManager)
    {
        this.relationship = relationship;
        manager = onlineFriendsListManager;
        friendNameText.text = relationship.Member.Profile.Name.Split("#")[0];
        CheckForPlayerPresence();
    }

    /// <summary>Sends a lobby invitation when one has not already been sent.</summary>
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

    /// <summary>Checks whether the friend is already in the current lobby.</summary>
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
