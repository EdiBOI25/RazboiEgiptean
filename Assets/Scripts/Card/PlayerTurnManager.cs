using UnityEngine;

namespace Card
{
    public class PlayerTurnManager : MonoBehaviour
    {
        private InGamePlayer localPlayer;
        private InGamePlayer remotePlayer;
        private InGamePlayer currentPlayer;
        private InGamePlayer lastFaceCardPlayer;

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

        public void Init(DeckManager deck, InGamePlayer local, InGamePlayer remote)
        {
        }

        void OnCardPlayed(Card card)
        {
        }

        void OnSlapAttempt(InGamePlayer slapper)
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

        void AwardPileTo(InGamePlayer player)
        {
        }

        void CheckWinCondition()
        {
        }
    }
}
