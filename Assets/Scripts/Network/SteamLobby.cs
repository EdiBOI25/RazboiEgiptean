using System;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{   
    public static SteamLobby Instance; // Singleton
    
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> LobbyEnter;

    public ulong currentLobbyId;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager _manager;

    public Button hostButton;
    public Text lobbyNameText;

    private void Start()
    {
        // check if steam is initialized
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steamworks not initialized");
            return;
        }

        if (Instance == null)
        {
            Instance = this;
        }
        
        _manager = GetComponent<CustomNetworkManager>();
        
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequested);
        LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        
        // hostButton.SetActive(true);
        hostButton.interactable = true;
        lobbyNameText.gameObject.SetActive(false);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _manager.maxConnections);
    }

    private void OnApplicationQuit()
    {
        if (SteamManager.Initialized)
        {
            SteamAPI.Shutdown();
            Debug.Log("Steam Shutdown");
        }
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log($"OnLobbyCreated callback result {callback.m_eResult}");
            return;
        }
        
        Debug.Log("OnLobbyCreated callback result " + callback.m_eResult);
        
        _manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey,
            SteamUser.GetSteamID().ToString());

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name",
            SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    private void OnJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to join lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        // Everyone (clients + hosts)
        Debug.Log("OnLobbyEntered callback");
        // hostButton.SetActive(false);
        hostButton.interactable = false;
        currentLobbyId = callback.m_ulSteamIDLobby;
        lobbyNameText.gameObject.SetActive(true);
        lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(currentLobbyId), "name");
        
        // Clients only
        if (NetworkServer.active)
        {
            return;
        }
        
        _manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(currentLobbyId), HostAddressKey);
        _manager.StartClient();
    }
}
