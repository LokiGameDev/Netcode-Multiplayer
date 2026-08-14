using UnityEngine;
using TMPro;
using Unity.Services.Friends.Models;

public class FriendItem : MonoBehaviour
{
    [SerializeField] private TMP_Text friendNameText;
    [SerializeField] private TMP_Text friendStatusText;
    [SerializeField] private bool friendOnlineStatus;

    private FriendsManager friendsManager;

    public void Initialize(FriendsManager friendsManager, Relationship relationship)
    {
        this.friendsManager = friendsManager;
        friendNameText.text = relationship.Member.Profile.Name;
        friendOnlineStatus = relationship.Member.Presence.Availability == Availability.Online;

        if(friendOnlineStatus) friendStatusText.text = "Online";
        else friendStatusText.text = "Offline";
    }

    public void Message()
    {
        friendsManager.MessageFriend();
    }
}
