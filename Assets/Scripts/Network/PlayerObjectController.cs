using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    // Player Data
    [SyncVar] public int connectionId;
    [SyncVar] public int playerIdNumber;
    [SyncVar] public ulong playerSteamId;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string playerName;

    private CustomNetworkManager _manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (_manager) { return _manager; }
            return _manager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }
    
    [Command]
    private void CmdSetPlayerName(string newName)
    {
        this.PlayerNameUpdate(this.playerName, newName);
    }

    public void PlayerNameUpdate(string oldName, string newName)
    {
        if (isServer)
        {
            this.playerName = newName;
        }
        
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }
}
