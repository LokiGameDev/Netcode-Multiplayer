using UnityEngine;
using TMPro;
using Unity.Services.Friends.Models;
using Unity.Services.Friends;
using Unity.Services.Friends.Notifications;

public class FriendItem : MonoBehaviour
{
    [SerializeField] private TMP_Text friendNameText;
    [SerializeField] private TMP_Text friendStatusText;
    [SerializeField] private bool friendOnlineStatus;

    [SerializeField] private Color onlineStatusColor = Color.lawnGreen;
    [SerializeField] private Color offlineStatusColor = Color.softRed;

    private Relationship relationship;
    private FriendsManager friendsManager;

    public void Initialize(FriendsManager friendsManager, Relationship relationship)
    {
        this.friendsManager = friendsManager;
        this.relationship = relationship;
        string name = relationship.Member.Profile.Name;
        friendNameText.text = name.Split('#')[0];
        friendOnlineStatus = false;
    }

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

    public Relationship GetRelationship()
    {
        return relationship;
    }

    public void DeleteFriend()
    {
        friendsManager.DeleteFriendRequest(relationship);
    }

    public void DestroyItself()
    {
        Destroy(gameObject);
    }
}
