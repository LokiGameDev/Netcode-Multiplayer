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

/// <summary>
/// Manages friend relationships, friend requests, and lobby invitations.
/// </summary>
public class FriendsManager : MonoBehaviour
{
    [Tooltip("Input used to find a player by display name.")]
    [SerializeField] private TMP_InputField friendNameInputField;
    [Tooltip("Input used to find a player by ID.")]
    [SerializeField] private TMP_InputField friendIdInputField;
    [Tooltip("Text element that displays the local player ID.")]
    [SerializeField] private TMP_Text playerIDText;
    [Tooltip("Displays status messages to the player.")]
    [SerializeField] private ShowNotification displayMessage;
    [Tooltip("UI item used to display an incoming lobby invitation.")]
    [SerializeField] private InvitationItem invitationItem;
    [Tooltip("Main menu controller used to join an invited lobby.")]
    [SerializeField] private MainMenuManager mainMenuManager;
    [Header("Friends List Items")]
    [Tooltip("Parent transform for friend list entries.")]
    [SerializeField] private Transform friendsListParent;
    [Tooltip("Prefab used for a friend list entry.")]
    [SerializeField] private FriendItem friendItemPrefab;
    [Tooltip("Object shown when the friend list is empty.")]
    [SerializeField] private GameObject noFriendsObject;
    [Header("Request List Items")]
    [Tooltip("Parent transform for incoming friend request entries.")]
    [SerializeField] private Transform friendsRequestListParent;
    [Tooltip("Prefab used for an incoming friend request entry.")]
    [SerializeField] private FriendRequestItem friendRequestItemPrefab;
    [Tooltip("Object shown when there are no incoming requests.")]
    [SerializeField] private GameObject noFriendsRequestObject;

    [Tooltip("Confirmation panel shown before removing a friend.")]
    [SerializeField] private GameObject DeleteFriendConfirmationPanel;
    [Tooltip("Displays the name of the friend being removed.")]
    [SerializeField] private TMP_Text DeleteFriendNameText;

    [Tooltip("Panel shown while the friends service is loading.")]
    [SerializeField] private GameObject loadingPanel;

    private Dictionary<string, FriendItem> friendItems = new();
    private Dictionary<string, FriendRequestItem> friendRequestItems = new();

    private List<Relationship> friendsList = new List<Relationship>();
    private List<Relationship> friendRequests = new List<Relationship>();

    private List<string> CurrentlyNotFriends = new List<string>();

    private Relationship currentFriendOnDeleteRequest;

    private LobbyData currentInvitedLobbyData;

    public UnityEvent FriendRequestSent;

    /// <summary>Initializes the loading panel state.</summary>
    private void Start()
    {
        loadingPanel.SetActive(false);
    }

    /// <summary>Subscribes to friends service events and refreshes the lists.</summary>
    private void OnEnable()
    {
        FriendsService.Instance.MessageReceived += OnMessageReceived;
        FriendsService.Instance.PresenceUpdated += OnPresenceUpdated;
        FriendsService.Instance.RelationshipAdded += OnRelationShipAdded;
        FriendsService.Instance.RelationshipDeleted += OnRelationShipRemoved;
        
        //playerIDText.text = AuthenticationService.Instance.PlayerId;

        Debug.Log("ID : " + AuthenticationService.Instance.PlayerId + ", Name : " + AuthenticationService.Instance.PlayerName);

        noFriendsObject.SetActive(true);
        noFriendsRequestObject.SetActive(true);
        DeleteFriendConfirmationPanel.SetActive(false);

        RefreshLists();
    }

    /// <summary>Unsubscribes from friends service events.</summary>
    private void OnDisable()
    {
        FriendsService.Instance.MessageReceived -= OnMessageReceived;
        FriendsService.Instance.PresenceUpdated -= OnPresenceUpdated;
        FriendsService.Instance.RelationshipAdded -= OnRelationShipAdded;
        FriendsService.Instance.RelationshipDeleted -= OnRelationShipRemoved;
    }

    /// <summary>Refreshes both friends and incoming request lists.</summary>
    public void RefreshLists()
    {
        RefreshFriendsList();
        RefreshRequestList();
    }

    /// <summary>Synchronizes the displayed friends with the friends service.</summary>
    public void RefreshFriendsList()
    {
        try
        {
            var friends = FriendsService.Instance.Friends;

            Debug.Log($"Friends count: {friends.Count}");

            noFriendsObject.SetActive(friends.Count==0);

            CurrentlyNotFriends.Clear();

            foreach(var frnd in friendItems)
            {
                if(!friends.Contains(frnd.Value.GetRelationship()))
                {
                    CurrentlyNotFriends.Add(frnd.Key);
                }
            }

            RemoveNonFriends();

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

    /// <summary>Synchronizes the displayed requests with the friends service.</summary>
    public void RefreshRequestList()
    {
        try
        {
            var requests = FriendsService.Instance.IncomingFriendRequests;

            Debug.Log($"Incoming requests: {requests.Count}");

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
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }
    }

    /// <summary>Removes displayed entries that are no longer friends.</summary>
    public void RemoveNonFriends()
    {
        foreach(var frnd in CurrentlyNotFriends)
        {
            Debug.Log($"Trying to remove {frnd} from friends list");

            var obj = friendItems[frnd];
            friendItems.Remove(frnd);
            obj.DestroyItself();
        }
    }

    /// <summary>Validates the entered name and sends a friend request.</summary>
    public async void AddFriendByName()
    {
        string name = friendNameInputField.text.Trim();

        if (string.IsNullOrEmpty(name))
            return;

        if (HasRelationship(name))
        {
            Debug.Log("A relationship already exists with this player.");
            return;
        }
        else
        {
            await AddFriendByNameAsync(name);
        }
    }

    /// <summary>Sends a friend request and displays the service result.</summary>
    private async Task AddFriendByNameAsync(string name)
    {
        try
        {
            name = name.Trim();

            Debug.Log($"Trying to add friend: [{name}]");

            Relationship relationship =
                await FriendsService.Instance.AddFriendByNameAsync(name);

            Debug.Log($"Friend request created: {relationship}");

            displayMessage.ShowText("Request sent successfully!");

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

    /// <summary>Checks whether a relationship already exists for the given name.</summary>
    private bool HasRelationship(string name)
    {
        // Check friends
        if (FriendsService.Instance.Friends
            .Any(r => r.Member.Profile.Name == name))
        {
            displayMessage.ShowText("You are already friends");
            return true;
        }

        // Check incoming requests
        if (FriendsService.Instance.IncomingFriendRequests
            .Any(r => r.Member.Profile.Name == name))
        {
            displayMessage.ShowText("They already sent a request");
            return true;
        }

        // Check outgoing requests
        if (FriendsService.Instance.OutgoingFriendRequests
            .Any(r => r.Member.Profile.Name == name))
        {
            displayMessage.ShowText("You already sent a request");
            return true;
        }

        return false;
    }

    /// <summary>Accepts an incoming friend request and updates the request list.</summary>
    public async Task AcceptFriendRequest(Relationship request)
    {
        try
        {
            Relationship relationship =
            await FriendsService.Instance.AddFriendAsync(request.Member.Id);

            friendRequests.Remove(request);

            if (friendRequestItems.TryGetValue(request.Member.Id, out FriendRequestItem friendRequestItem))
            {
                Debug.Log("Trying to delete request item");
                friendRequestItems.Remove(request.Member.Id);
                friendRequestItem.DestroyItself();
            }

            displayMessage.ShowText("Friend request Accepted");

            Debug.Log($"Relationship is now: {relationship.Type}");

            RefreshLists();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to accept friend request: {e}");
        }
    }

    /// <summary>Updates displayed presence for the affected friend.</summary>
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

    /// <summary>Refreshes the friends list after a relationship is added.</summary>
    private void OnRelationShipAdded(IRelationshipAddedEvent @event)
    {
        RefreshFriendsList();
    }

    /// <summary>Refreshes the friends list after a relationship is removed.</summary>
    private void OnRelationShipRemoved(IRelationshipDeletedEvent @event)
    {
        RefreshFriendsList();
    }

    /// <summary>Returns the friends currently marked as online.</summary>
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

    /// <summary>Displays an invitation received from a friend.</summary>
    public void OnMessageReceived(IMessageReceivedEvent @event)
    {
        Debug.Log($"Message Received from {@event.UserId}");
        if (friendItems.ContainsKey(@event.UserId))
        {
            currentInvitedLobbyData = @event.GetAs<LobbyData>();
            Debug.Log($"Message Received. Lobby code: {currentInvitedLobbyData.lobbyCode}");
            invitationItem.ShowInvitation(currentInvitedLobbyData.lobbyOwnerName, currentInvitedLobbyData.lobbyName);
        }
    }

    /// <summary>Accepts the currently displayed invitation from the specified owner.</summary>
    public void AcceptCurrentInvitation(string OwnerName)
    {
        if(OwnerName == currentInvitedLobbyData.lobbyOwnerName)
        {
            Debug.Log($"Accepted invitation to join {currentInvitedLobbyData.lobbyCode}");
            mainMenuManager.StartClient(currentInvitedLobbyData.lobbyCode);
            StartTheLoadingPanel();
        }
    }

    /// <summary>Shows the loading panel while joining an invited lobby.</summary>
    private void StartTheLoadingPanel()
    {
        loadingPanel.SetActive(true);
    }

    /// <summary>Marks the local player as offline before application exit.</summary>
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

    /// <summary>Opens the confirmation panel for the selected friend.</summary>
    public void DeleteFriendRequest(Relationship relationship)
    {
        currentFriendOnDeleteRequest = relationship;

        DeleteFriendNameText.text = currentFriendOnDeleteRequest.Member.Profile.Name.Split('#')[0];;

        DeleteFriendConfirmationPanel.SetActive(true);
    }

    /// <summary>Removes the selected friend after confirmation.</summary>
    public async void DeleteFriendConfirmation()
    {
        try
        {
            await FriendsService.Instance.DeleteFriendAsync(currentFriendOnDeleteRequest.Member.Id);

            Debug.Log($"Deleted {currentFriendOnDeleteRequest.Member.Id}");

            displayMessage.ShowText($"{currentFriendOnDeleteRequest.Member.Profile.Name} removed successfully");

            friendsList.Remove(currentFriendOnDeleteRequest);

            if (friendItems.TryGetValue(currentFriendOnDeleteRequest.Member.Id, out FriendItem friendItem))
            {
                friendItems.Remove(currentFriendOnDeleteRequest.Member.Id);
                Destroy(friendItem.gameObject);
            }

            DeleteFriendConfirmationPanel.SetActive(false);

            currentFriendOnDeleteRequest = null;

            RefreshFriendsList();
        }
        catch (Exception e)
        {
            displayMessage.ShowText($"Something went wrong!");

            Debug.LogError($"Failed to delete {currentFriendOnDeleteRequest.Member.Id}: {e}");
        }
    }

    /// <summary>Removes all current friends for testing purposes.</summary>
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


/// <summary>Stores the lobby details shared in a friend invitation.</summary>
public class LobbyData
{
    public string lobbyOwnerName;
    public string lobbyName;
    public string lobbyCode;
}