using UnityEngine;
using TMPro;
using Unity.Services.Friends.Models;
using Unity.Services.Friends;
using Unity.Services.Friends.Notifications;

public class FriendRequestItem : MonoBehaviour
{
    [SerializeField] private TMP_Text friendNameText;
    [SerializeField] private TMP_Text friendStatusText;
    [SerializeField] private bool friendOnlineStatus;

    private Relationship relationship;
    private FriendsManager friendsManager;

    public void Initialize(FriendsManager friendsManager, Relationship relationship)
    {
        this.friendsManager = friendsManager;
        this.relationship = relationship;
        friendNameText.text = relationship.Member.Profile.Name;
        friendOnlineStatus = relationship.Member.Presence.Availability == Availability.Online;

        if(friendOnlineStatus) friendStatusText.text = "Online";
        else friendStatusText.text = "Offline";
    }

    public void Accept()
    {
        friendsManager.AcceptFriendRequest();
    }

    private void OnEnable()
    {
        FriendsService.Instance.PresenceUpdated += OnPresenceUpdated;
    }

    private void OnDisable()
    {
        FriendsService.Instance.PresenceUpdated -= OnPresenceUpdated;
    }

    private void OnPresenceUpdated(IPresenceUpdatedEvent @event)
    {
        friendOnlineStatus = relationship.Member.Presence.Availability == Availability.Online;

        if(friendOnlineStatus) friendStatusText.text = "Online";
        else friendStatusText.text = "Offline";
    }
}
