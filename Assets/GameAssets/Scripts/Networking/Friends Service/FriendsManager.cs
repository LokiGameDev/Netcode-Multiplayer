using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Friends;
using Unity.Services.Friends.Models;
using UnityEngine;

public class FriendsManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField friendNameInputField;
    [SerializeField] private TMP_InputField friendIdInputField;
    [SerializeField] private TMP_Text playerIDText;
    [SerializeField] private ShowNotification displayMessage;
    [Header("Firends List Items")]
    [SerializeField] private Transform friendsListParent;
    [SerializeField] private FriendItem friendItemPrefab;
    [SerializeField] private GameObject noFriendsObject;
    [Header("Request List Items")]
    [SerializeField] private Transform friendsRequestListParent;
    [SerializeField] private FriendItem friendRequestItemPrefab;
    [SerializeField] private GameObject noFriendsRequestObject;

    private List<Relationship> friendsList = new List<Relationship>();

    private void Start()
    {
        noFriendsObject.SetActive(true);
        noFriendsRequestObject.SetActive(true);
        StartInitialize();
    }

    private async void StartInitialize()
    {
        try
        {
            await FriendsService.Instance.InitializeAsync();
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }

        //playerIDText.text = AuthenticationService.Instance.PlayerId;

        Debug.Log("ID : " + AuthenticationService.Instance.PlayerId + ", Name : " + AuthenticationService.Instance.PlayerName);

        RefreshFriendsList();
    }

    public void RefreshLists()
    {
        RefreshFriendsList();
        RefreshRequestList();
    }

    public void RefreshFriendsList()
    {
        try
        {
            var friends = FriendsService.Instance.Friends;

            Debug.Log(friends.Count);
            noFriendsObject.SetActive(friends.Count==0);

            friendsList.Clear();
            foreach(Transform child in friendsListParent)
            {
                Destroy(child.gameObject);
            }

            foreach(var frnd in friends)
            {
                Debug.Log("Adding " + frnd.Member.Profile.Name + " to the friends list.");
                friendsList.Add(frnd);
                var friend = Instantiate(friendItemPrefab, friendsListParent);
                friend.Initialize(this, frnd);
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }
    }

    public void RefreshRequestList()
    {
        var requests = FriendsService.Instance.IncomingFriendRequests;

        Debug.Log($"Incoming requests: {requests.Count}");
        displayMessage.ShowText($"{requests.Count}");
        noFriendsRequestObject.SetActive(requests.Count==0);

        foreach (var request in requests)
        {
            var friendRequest = Instantiate(friendRequestItemPrefab, friendsRequestListParent);

            Debug.Log(
                $"Role: {request.Member.Role}, " +
                $"ID: {request.Member.Id}, " +
                $"Name: {request.Member.Profile.Name}"
            );
        }
    }

    public async void AddFriendByName()
    {
        string name = friendNameInputField.text.Trim();

        if (string.IsNullOrEmpty(name))
            return;
        
        await AddFriendByNameAsync(name);
    }

    public async void AddFriendById()
    {
        string memberId = friendIdInputField.text.Trim();

        if (string.IsNullOrEmpty(memberId))
            return;

        if (HasRelationship(memberId))
        {
            Debug.Log("A relationship already exists with this player.");
            return;
        }
        else
        {
            await AddFriendByIdAsync(memberId);
        }
    }

    private async Task AddFriendByIdAsync(string memberId)
    {
        try
        {
            Debug.Log($"Trying to add friend: [{memberId}]");

            Relationship relationship =
                await FriendsService.Instance.AddFriendAsync(memberId);

            Debug.Log($"Friend request created: {relationship}");
            displayMessage.ShowText("Success");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to add [{memberId}]\n{e}");
        }
    }

    private async Task AddFriendByNameAsync(string name)
    {
        try
        {
            name = name.Trim();

            Debug.Log($"Trying to add friend: [{name}]");

            Relationship relationship =
                await FriendsService.Instance.AddFriendByNameAsync(name);

            Debug.Log($"Friend request created: {relationship}");
        }
        catch (Exception e)
        {
            displayMessage.ShowText("Something went wrong Try again");
            Debug.LogError($"Failed to add [{name}]\n{e}");
        }
    }

    private bool HasRelationship(string memberId)
    {
        // Check friends
        if (FriendsService.Instance.Friends
            .Any(r => r.Member.Id == memberId))
        {
            displayMessage.ShowText("You are already friends");
            return true;
        }

        // Check incoming requests
        if (FriendsService.Instance.IncomingFriendRequests
            .Any(r => r.Member.Id == memberId))
        {
            displayMessage.ShowText("They already sent a request");
            return true;
        }

        // Check outgoing requests
        if (FriendsService.Instance.OutgoingFriendRequests
            .Any(r => r.Member.Id == memberId))
        {
            displayMessage.ShowText("You already sent a request");
            return true;
        }

        return false;
    }

    public void MessageFriend()
    {
        displayMessage.ShowText("Not yet implemented");
    }

    public async void DeleteAllFriends()
    {
        var friends = friendsList.ToList();

        foreach (Relationship friend in friends)
        {
            try
            {
                await FriendsService.Instance.DeleteFriendAsync(friend.Member.Id);

                Debug.Log($"Deleted {friend.Member.Id}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete {friend.Member.Id}: {e}");
            }
        }

        friendsList.Clear();
    }
}
