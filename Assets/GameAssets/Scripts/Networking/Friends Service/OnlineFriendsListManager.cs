using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Friends;
using Unity.Services.Friends.Models;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

/// <summary>Maintains the list of online friends available for invitation.</summary>
public class OnlineFriendsListManager : NetworkBehaviour
{
    [Tooltip("Parent transform for online friend items.")]
    [SerializeField] private Transform onlineFriendsListContainer;
    [Tooltip("Prefab used for each online friend item.")]
    [SerializeField] private GameObject onlineFriendItemPrefab;
    [Tooltip("Panel containing the online friends list.")]
    [SerializeField] private GameObject onlineListPanel;

    private List<Relationship> currentOnlineFriends = new List<Relationship>();

    private List<GameObject> onlineFriendItems = new List<GameObject>();

    private LobbyData currentLobbyData = new LobbyData();
    /// <summary>Initializes the online friends panel for the host.</summary>
    private void OnEnable()
    {
        onlineListPanel.SetActive(false);

        if(!IsHost) return;

        RefreshTheList();

        currentLobbyData.lobbyCode = HostSingleton.Instance.GameManager.GetJoinCode();
        currentLobbyData.lobbyName = HostSingleton.Instance.GameManager.currentLobbyName;
        currentLobbyData.lobbyOwnerName = HostSingleton.Instance.GameManager.hostName;
    }

    /// <summary>Refreshes displayed online friends and removes stale entries.</summary>
    public void RefreshTheList()
    {
        List<Relationship> friendsList = GetOnlineFriends();

        foreach(var frnd in friendsList)
        {
            if(!currentOnlineFriends.Contains(frnd))
            {
                currentOnlineFriends.Add(frnd);
                var frndItem = Instantiate(onlineFriendItemPrefab, onlineFriendsListContainer);
                onlineFriendItems.Add(frndItem);
                frndItem.GetComponent<OnlineFriendItem>().SetUp(frnd, this);
            }
        }

        foreach(var frnd in onlineFriendItems)
        {
            frnd.GetComponent<OnlineFriendItem>().CheckForPlayerPresence();
            
            if(!friendsList.Contains(frnd.GetComponent<OnlineFriendItem>().relationship))
            {
                currentOnlineFriends.Remove(frnd.GetComponent<OnlineFriendItem>().relationship);
                onlineFriendItems.Remove(frnd);
                Destroy(frnd);
            }
        }
    }

    /// <summary>Checks whether a friend is already in the current lobby.</summary>
    public async Task<bool> DoPlayerExistInLobby(string memberId)
    {
        Lobby currentLobby = await LobbyService.Instance.GetLobbyAsync(HostSingleton.Instance.GameManager.currentLobby.Id);

        bool isAlreadyInLobby = currentLobby.Players.Any(player => player.Id == memberId);

        foreach(Player player in currentLobby.Players)
        {
            Debug.Log(player.Id);
        }

        Debug.Log($"Checked presence of {memberId}: {isAlreadyInLobby}");

        return isAlreadyInLobby;
    }

    /// <summary>Toggles the online friends panel.</summary>
    public void SetOnlinePanelState()
    {
        onlineListPanel.SetActive(!onlineListPanel.activeInHierarchy);
    }

    /// <summary>Sends the current lobby invitation to a friend.</summary>
    public async void SendInvite(Relationship friend)
    {
        try
        {
            await FriendsService.Instance.MessageAsync(friend.Member.Id, currentLobbyData);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>Returns the relationships whose presence is online.</summary>
    private List<Relationship> GetOnlineFriends()
    {
        List<Relationship> friends = new List<Relationship>();
        try
        {
            var friendsItems = FriendsService.Instance.Friends;

            foreach(var frnd in friendsItems)
            {
                if(frnd.Member.Presence.Availability == Availability.Online)
                {
                    friends.Add(frnd);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return null;   
        }

        return friends;
    }
}
