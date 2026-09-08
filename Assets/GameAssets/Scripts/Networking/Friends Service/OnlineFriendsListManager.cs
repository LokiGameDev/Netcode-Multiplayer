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

public class OnlineFriendsListManager : NetworkBehaviour
{
    [SerializeField] private Transform onlineFriendsListContainer;
    [SerializeField] private GameObject onlineFriendItemPrefab;
    [SerializeField] private GameObject onlineListPanel;

    private List<Relationship> currentOnlineFriends = new List<Relationship>();

    private List<GameObject> onlineFriendItems = new List<GameObject>();

    private LobbyData currentLobbyData = new LobbyData();
    private void OnEnable()
    {
        onlineListPanel.SetActive(false);

        if(!IsHost) return;

        RefreshTheList();

        currentLobbyData.lobbyCode = HostSingleton.Instance.GameManager.GetJoinCode();
        currentLobbyData.lobbyName = HostSingleton.Instance.GameManager.currentLobbyName;
        currentLobbyData.lobbyOwnerName = HostSingleton.Instance.GameManager.hostName;
    }

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

    public void SetOnlinePanelState()
    {
        onlineListPanel.SetActive(!onlineListPanel.activeInHierarchy);
    }

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
