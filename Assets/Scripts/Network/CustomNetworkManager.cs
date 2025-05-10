using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class CustomNetworkManager : NetworkManager
    {
        [SerializeField] private PlayerObjectController gamePlayerPrefab;
        public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

        public override void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // base.OnServerAddPlayer(conn);
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                PlayerObjectController player = Instantiate(gamePlayerPrefab);
                player.connectionId = conn.connectionId;
                player.playerIdNumber = GamePlayers.Count + 1;
                player.playerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(
                    (CSteamID)SteamLobby.Instance.currentLobbyId, GamePlayers.Count);
            
                NetworkServer.AddPlayerForConnection(conn, player.gameObject);
            }
        }
    }
}
