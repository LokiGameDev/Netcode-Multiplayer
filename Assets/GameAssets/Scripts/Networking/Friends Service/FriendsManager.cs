using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Friends;
using Unity.Services.Friends.Models;
using Unity.Services.Friends.Notifications;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;

public class FriendsManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField friendNameInputField;
    [SerializeField] private TMP_InputField friendIdInputField;
    [SerializeField] private TMP_Text playerIDText;
    [SerializeField] private ShowNotification displayMessage;
    [SerializeField] private InvitationItem invitationItem;
    [SerializeField] private MainMenuManager mainMenuManager;
    [Header("Firends List Items")]
    [SerializeField] private Transform friendsListParent;
    [SerializeField] private FriendItem friendItemPrefab;
    [SerializeField] private GameObject noFriendsObject;
    [Header("Request List Items")]
    [SerializeField] private Transform friendsRequestListParent;
    [SerializeField] private FriendRequestItem friendRequestItemPrefab;
    [SerializeField] private GameObject noFriendsRequestObject;

    [SerializeField] private GameObject loadingPanel;

    private List<Relationship> friendsList = new List<Relationship>();

    private Dictionary<string, FriendItem> friendItems = new();
    private Dictionary<string, FriendRequestItem> friendRequestItems = new();

    private List<Relationship> friendRequests = new List<Relationship>();

    private LobbyData currentInvitedLobbyData;

    public UnityEvent FriendRequestSent;

    private void Start()
    {
        loadingPanel.SetActive(false);
    }

    private void OnEnable()
    {
        FriendsService.Instance.MessageReceived += OnMessageReceived;
        FriendsService.Instance.PresenceUpdated += OnPresenceUpdated;
        
        //playerIDText.text = AuthenticationService.Instance.PlayerId;

        Debug.Log("ID : " + AuthenticationService.Instance.PlayerId + ", Name : " + AuthenticationService.Instance.PlayerName);

        noFriendsObject.SetActive(true);
        noFriendsRequestObject.SetActive(true);

        RefreshLists();
    }

    private void OnDisable()
    {
        FriendsService.Instance.MessageReceived -= OnMessageReceived;
        FriendsService.Instance.PresenceUpdated -= OnPresenceUpdated;
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

            foreach(var frnd in friends)
            {
                if(friendsList.Contains(frnd)) continue;

                Debug.Log("Adding " + frnd.Member.Profile.Name + " to the friends list.");
                friendsList.Add(frnd);
                var friend = Instantiate(friendItemPrefab, friendsListParent);
                friend.Initialize(this, frnd);
                friendItems[frnd.Member.Id] = friend;
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

        //displayMessage.ShowText($"You have {requests.Count} requests");

        noFriendsRequestObject.SetActive(requests.Count==0);

        foreach (var request in requests)
        {
            if(friendRequests.Contains(request)) continue;

            friendRequests.Add(request);

            var friendRequest = Instantiate(friendRequestItemPrefab, friendsRequestListParent);

            friendRequest.Initialize(this, request);

            friendRequestItems[request.Member.Id] = friendRequest;

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

            FriendRequestSent?.Invoke();
        }
        catch (RequestFailedException e)
        {
            Debug.LogError(
                $"Failed to add [{name}] | " +
                $"ErrorCode: {e.ErrorCode} | " +
                $"Message: {e.Message}"
            );

            switch (e.ErrorCode)
            {
                case CommonErrorCodes.NotFound:
                    displayMessage.ShowText("Player not found.");
                    break;

                case CommonErrorCodes.Forbidden:
                    displayMessage.ShowText("You cannot send a request to this player.");
                    break;

                case CommonErrorCodes.TooManyRequests:
                    displayMessage.ShowText("Too many requests.\nTry again later.");
                    break;

                case CommonErrorCodes.Timeout:
                case CommonErrorCodes.TransportError:
                    displayMessage.ShowText("Check your internet connection.");
                    break;

                case CommonErrorCodes.ServiceUnavailable:
                    displayMessage.ShowText("Service is temporarily unavailable.");
                    break;

                case CommonErrorCodes.InvalidToken:
                case CommonErrorCodes.TokenExpired:
                    displayMessage.ShowText("Your session has expired.\nPlease reconnect.");
                    break;

                default:
                    displayMessage.ShowText("Unable to send friend request.");
                    break;
            }
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

    public async Task AcceptFriendRequest(Relationship request)
    {
        try
        {
            Relationship relationship =
            await FriendsService.Instance.AddFriendAsync(request.Member.Id);

            friendRequests.Remove(request);

            Debug.Log($"Relationship is now: {relationship.Type}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to accept friend request: {e}");
        }
    }

    private void OnPresenceUpdated(IPresenceUpdatedEvent @event)
    {
        if (friendItems.TryGetValue(@event.ID, out FriendItem friend))
        {
            friend.UpdatePresence(@event.Presence.Availability);
        }
        if (friendRequestItems.TryGetValue(@event.ID, out FriendRequestItem friendRequest))
        {
            friendRequest.UpdatePresence(@event.Presence.Availability);
        }
    }

    public List<Relationship> GetCurrentOnlineFriends()
    {
        List<Relationship> currentOnline = new List<Relationship>();
        foreach(var frnd in friendsList)
        {
            if(frnd.Member.Presence.Availability == Availability.Online)
            {
                currentOnline.Add(frnd);
            }
        }

        return currentOnline;
    }

    public void OnMessageReceived(IMessageReceivedEvent @event)
    {
        Debug.Log($"Message Received from {@event.UserId}");
        if (friendItems.ContainsKey(@event.UserId))
        {
            currentInvitedLobbyData = @event.GetAs<LobbyData>();
            Debug.Log($"Message Received from {currentInvitedLobbyData.lobbyCode}");
            invitationItem.ShowInvitation(currentInvitedLobbyData.lobbyOwnerName, currentInvitedLobbyData.lobbyName);
        }
    }

    public void AcceptCurrentInvitation(string OwnerName)
    {
        if(OwnerName == currentInvitedLobbyData.lobbyOwnerName)
        {
            Debug.Log($"Accepted invitation to join {currentInvitedLobbyData.lobbyCode}");
            mainMenuManager.StartClient(currentInvitedLobbyData.lobbyCode);
            StartTheLoadingPanel();
        }
    }

    private void StartTheLoadingPanel()
    {
        loadingPanel.SetActive(true);
    }

    private async void OnApplicationQuit()
    {
        try
        {
            await FriendsService.Instance.SetPresenceAvailabilityAsync(
                Availability.Offline
            );
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
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


public class LobbyData
{
    public string lobbyOwnerName;
    public string lobbyName;
    public string lobbyCode;
}