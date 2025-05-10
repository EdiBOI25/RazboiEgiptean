using Mirror;
using Network;
using UnityEngine;

namespace Card
{
    public class GameManager : MonoBehaviour
    {
        public PlayerListItem player1Ui;
        public PlayerListItem player2Ui;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var players = ((CustomNetworkManager)NetworkManager.singleton).GamePlayers;
            // debug that there are currently 2 players
            Debug.Log($"Number of players: {players.Count}");
            PlayerObjectController localPlayer = null;
            PlayerObjectController opponentPlayer = null;

            foreach (var player in players)
            {
                if (player.isLocalPlayer)
                    localPlayer = player;
                else
                    opponentPlayer = player;
            }
            
            SetPlayerUi(localPlayer, player1Ui);
            SetPlayerUi(opponentPlayer, player2Ui);
        }
        
        private void SetPlayerUi(PlayerObjectController player, PlayerListItem playerUi)
        {
            // if (!player)
            //     return;
            
            PlayerListItem playerItemScript = playerUi.GetComponent<PlayerListItem>();

            playerItemScript.playerName = player.playerName;
            playerItemScript.playerSteamId = player.playerSteamId;
            playerItemScript.connectionId = player.connectionId;
            playerItemScript.SetPlayerValues();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
