using UnityEngine;
using TMPro;
using Unity.Services.Friends.Models;
using Unity.Services.Friends;
using Unity.Services.Friends.Notifications;

/// <summary>Displays one friend and its current presence state.</summary>
public class FriendItem : MonoBehaviour
{
    [Tooltip("Text displaying the friend's name.")]
    [SerializeField] private TMP_Text friendNameText;
    [Tooltip("Text displaying the friend's online state.")]
    [SerializeField] private TMP_Text friendStatusText;
    [Tooltip("Whether the friend is currently online.")]
    [SerializeField] private bool friendOnlineStatus;

    [Tooltip("Color used for an online friend.")]
    [SerializeField] private Color onlineStatusColor = Color.lawnGreen;
    [Tooltip("Color used for an offline friend.")]
    [SerializeField] private Color offlineStatusColor = Color.softRed;

    private Relationship relationship;
    private FriendsManager friendsManager;

    /// <summary>Initializes the item with a relationship and manager.</summary>
    public void Initialize(FriendsManager friendsManager, Relationship relationship)
    {
        this.friendsManager = friendsManager;
        this.relationship = relationship;
        string name = relationship.Member.Profile.Name;
        friendNameText.text = name.Split('#')[0];
        friendOnlineStatus = false;
    }

    /// <summary>Updates the displayed presence state and color.</summary>
    public void UpdatePresence(Availability availability)
    {
        friendOnlineStatus = availability == Availability.Online;

        if (friendOnlineStatus)
        {
            friendStatusText.text = "Online";
            friendStatusText.color = onlineStatusColor;
        }
        else
        {
            friendStatusText.text = "Offline";
            friendStatusText.color = offlineStatusColor;
        }
    }

    /// <summary>Returns the relationship represented by this item.</summary>
    public Relationship GetRelationship()
    {
        return relationship;
    }

    /// <summary>Requests removal of the represented friend.</summary>
    public void DeleteFriend()
    {
        friendsManager.DeleteFriendRequest(relationship);
    }

    /// <summary>Destroys this list item.</summary>
    public void DestroyItself()
    {
        Destroy(gameObject);
    }
}
