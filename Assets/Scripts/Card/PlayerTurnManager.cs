using Network;
using UnityEngine;

namespace Card
{
    public class PlayerTurnManager : MonoBehaviour
    {
        private PlayerObjectController localPlayer;
        private PlayerObjectController remotePlayer;
        private PlayerObjectController currentPlayer;
        private PlayerObjectController lastFaceCardPlayer;

        private DeckManager deckManager;
        
        int faceCardCountdown = 0;
        private bool isSlapWindowOpen = true;
        
        enum GameState
        {
            WaitingForPlay,
            Countdown,
            SlapWindow,
            ResolvingPile,
            GameOver
        }
        GameState currentGameState = GameState.WaitingForPlay;

        public void Init(DeckManager deck, PlayerObjectController local, PlayerObjectController remote)
        {
        }

        void OnCardPlayed(Card card)
        {
        }

        void OnSlapAttempt(PlayerObjectController slapper)
        {
        }

        void Update()
        {
        }

        void NextTurn()
        {
        }

        void HandleFaceCard(Card faceCard)
        {
        }

        void ResolveCountdown(Card playedCard)
        {
        }

        void AwardPileTo(PlayerObjectController player)
        {
        }

        void CheckWinCondition()
        {
        }
    }
}
