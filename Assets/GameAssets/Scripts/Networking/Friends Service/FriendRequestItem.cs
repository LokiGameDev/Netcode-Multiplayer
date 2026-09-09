using UnityEngine;
using TMPro;
using Unity.Services.Friends.Models;
using Unity.Services.Friends;
using Unity.Services.Friends.Notifications;
using System;

/// <summary>Displays and manages one incoming friend request.</summary>
public class FriendRequestItem : MonoBehaviour
{
    [Tooltip("Text displaying the request sender's name.")]
    [SerializeField] private TMP_Text friendNameText;
    [Tooltip("Text displaying the request sender's online state.")]
    [SerializeField] private TMP_Text friendStatusText;
    [Tooltip("Whether the request sender is currently online.")]
    [SerializeField] private bool friendOnlineStatus;
    [Tooltip("Color used for an online request sender.")]
    [SerializeField] private Color onlineStatusColor = Color.lawnGreen;
    [Tooltip("Color used for an offline request sender.")]
    [SerializeField] private Color offlineStatusColor = Color.softRed;

    private Relationship relationship;
    private FriendsManager friendsManager;

    /// <summary>Initializes the item with a relationship and manager.</summary>
    public void Initialize(FriendsManager friendsManager, Relationship relationship)
    {
        this.friendsManager = friendsManager;
        this.relationship = relationship;
        friendNameText.text = relationship.Member.Profile.Name;
        friendOnlineStatus = false;
    }

    /// <summary>Accepts the represented friend request.</summary>
    public async void Accept()
    {
        try
        {
            await friendsManager.AcceptFriendRequest(relationship);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
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

    /// <summary>Destroys this request item.</summary>
    public void DestroyItself()
    {
        Destroy(gameObject);
    }
}
