using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;
    
    public Text lobbyNameText;

    public GameObject playerListViewContent;
    public GameObject playerListItemPrefab;
    public GameObject localPlayerObject;

    public ulong currentLobbyId;
    public bool playerItemCreated = false;
    private List<PlayerListItem> _playerListItems = new List<PlayerListItem>();

    public PlayerObjectController localPlayerController;
    private CustomNetworkManager _manager;
    
    private CustomNetworkManager Manager
    {
        get
        {
            if (_manager is not null) { return _manager; }
            return _manager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void UpdateLobbyName()
    {
        currentLobbyId = Manager.GetComponent<SteamLobby>().currentLobbyId;
        lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(currentLobbyId), "name");
    }
    
    public void UpdatePlayerList()
    {
        if (!playerItemCreated)
        {
            CreateHostPlayerItem();
        }

        if (_playerListItems.Count < Manager.GamePlayers.Count)
        {
            CreateClientPlayerItem();
        }
        
        if (_playerListItems.Count > Manager.GamePlayers.Count)
        {
            RemovePlayerItem();
        }

        if (_playerListItems.Count == Manager.GamePlayers.Count)
        {
            UpdatePlayerItem();
        }
    }

    public void FindLocalPlayer()
    {
        localPlayerObject = GameObject.Find("LocalGamePlayer");
        localPlayerController = localPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach (var player in Manager.GamePlayers)
        {
            GameObject newPlayerItem = Instantiate(playerListItemPrefab,
                playerListViewContent.transform, true) as GameObject;
            PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

            newPlayerItemScript.playerName = player.playerName;
            newPlayerItemScript.playerSteamId = player.playerSteamId;
            newPlayerItemScript.connectionId = player.connectionId;
            newPlayerItemScript.SetPlayerValues();

            newPlayerItem.transform.localScale = Vector3.one;
            // newPlayerItem.transform.localPosition = new Vector3(transform.position.x, transform.position.y, -10);
            
            _playerListItems.Add(newPlayerItemScript);
        }

        playerItemCreated = true;
    }
    
    public void CreateClientPlayerItem()
    {
        foreach (var player in Manager.GamePlayers)
        {
            if (_playerListItems.All(b => b.connectionId != player.connectionId))
            {
                GameObject newPlayerItem = Instantiate(playerListItemPrefab,
                    playerListViewContent.transform, true) as GameObject;
                PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

                newPlayerItemScript.playerName = player.playerName;
                newPlayerItemScript.playerSteamId = player.playerSteamId;
                newPlayerItemScript.connectionId = player.connectionId;
                newPlayerItemScript.SetPlayerValues();

                newPlayerItem.transform.localScale = Vector3.one;
                // newPlayerItem.transform.localPosition = new Vector3(transform.position.x, transform.position.y, -10);

                _playerListItems.Add(newPlayerItemScript);
            }
        }
    }
    
    public void UpdatePlayerItem()
    {
        foreach(PlayerObjectController player in Manager.GamePlayers)
        {
            foreach(PlayerListItem playerListItemScript in _playerListItems)
            {
                if (player.connectionId == playerListItemScript.connectionId)
                {
                    playerListItemScript.playerName = player.playerName;
                    playerListItemScript.SetPlayerValues();
                }
            }
        }
    }
    
    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemsToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem playerListItem in _playerListItems)
        {
            if(Manager.GamePlayers.All(b => b.connectionId != playerListItem.connectionId))
            {
                playerListItemsToRemove.Add(playerListItem);
            }
        }

        if (playerListItemsToRemove.Count > 0)
        {
            foreach (PlayerListItem playerListItemToRemove in playerListItemsToRemove)
            {
                GameObject objectToRemove = playerListItemToRemove.gameObject;
                _playerListItems.Remove(playerListItemToRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }
}
