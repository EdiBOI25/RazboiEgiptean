using System;
using Card;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
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

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        
        private void Update()
        {
            if (!isLocalPlayer) return; // Only local player should process input
            if (SceneManager.GetActiveScene().name != "TheGame") return;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                CmdPlayCard();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CmdClaimPile();
            }
        }
        
        [Command]
        private void CmdPlayCard()
        {
            // Get DeckManager reference
            DeckManager deck = FindObjectOfType<DeckManager>();

            // Find correct HandManager based on player ID or Steam ID
            HandManager myHand = (playerIdNumber == 1) ? deck.player1HandManager : deck.player2HandManager;

            deck.PlayRound(myHand);
        }

        [Command]
        private void CmdClaimPile()
        {
            DeckManager deck = FindObjectOfType<DeckManager>();
            HandManager myHand = (playerIdNumber == 1) ? deck.player1HandManager : deck.player2HandManager;

            // Equivalent of slapping pile or checking win condition
            deck.CheckForWinningCondition(myHand);
        }
    }
}
