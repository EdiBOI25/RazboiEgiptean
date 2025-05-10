using System;
using System.Collections.Generic;
using Mirror;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Card
{
    public class DeckManager : MonoBehaviour
    {
        [SerializeField] private List<CardData> allCards = new List<CardData>();
        [SerializeField] public HandManager player1HandManager;
        [SerializeField] public HandManager player2HandManager;
        [SerializeField] public HandManager centerPileHandManager;
        [SerializeField] private GameObject cardPrefab;
        
        private List<GameObject> _allCardsInScene = new List<GameObject>();

        private int _currentIndex = 0;
        
        public PlayerTurnManager playerTurnManager;

        private void Awake()
        {
            CardData[] cards = Resources.LoadAll<CardData>("Scriptable Objects/Cards");
            allCards.AddRange(cards);
            Debug.Log($"Loaded {allCards.Count} cards from Resources.");
            
            // playerTurnManager = new PlayerTurnManager();
            // playerTurnManager.Init(this, player1HandManager, player2HandManager);
        }

        private void Start()
        {
            // InstantiateCards();
            // DistributeCards();
            var players = ((CustomNetworkManager)NetworkManager.singleton).GamePlayers;

            PlayerObjectController localPlayer = null;
            PlayerObjectController opponentPlayer = null;

            foreach (var player in players)
            {
                if (player.isLocalPlayer)
                    localPlayer = player;
                else
                    opponentPlayer = player;
            }

            bool isLocalPlayerFirst = (localPlayer.playerIdNumber == 1);
            FindObjectOfType<DeckManager>().SetupHands(isLocalPlayerFirst);
        }
        
        public void SetupHands(bool isLocalPlayerBottom)
        {
            InstantiateCards(); // create all card objects

            if (isLocalPlayerBottom)
            {
                Debug.Log("Local player is Player 1 (bottom)");
                player1HandManager = GameObject.Find("BottomHandManager").GetComponent<HandManager>();
                player2HandManager = GameObject.Find("TopHandManager").GetComponent<HandManager>();
            }
            else
            {
                Debug.Log("Local player is Player 2 (bottom)");
                player1HandManager = GameObject.Find("TopHandManager").GetComponent<HandManager>();
                player2HandManager = GameObject.Find("BottomHandManager").GetComponent<HandManager>();
            }

            DistributeCards(); // now we know who is who
        }


        private void InstantiateCards()
        {
            for (int i = 0; i < allCards.Count; ++i)
            {
                GameObject card = Instantiate(cardPrefab, this.transform.position, Quaternion.identity, this.transform);
                card.GetComponent<Card>().SetCardData(allCards[i]);
                card.transform.localPosition = new Vector3(0, 0, 0);
                card.transform.localRotation = Quaternion.identity;
                // card.transform.localScale = new Vector3(1, 1, 1);
                _allCardsInScene.Add(card);
            }
        }

        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.UpArrow))
            // {
            //     PlayRound(player2HandManager);
            // }
            //
            // if (Input.GetKeyDown(KeyCode.DownArrow))
            // {
            //     PlayRound(player1HandManager);
            // }
            //
            // if (Input.GetKeyDown(KeyCode.LeftArrow))
            // {
            //     CheckForWinningCondition(player1HandManager);
            // }
            // if (Input.GetKeyDown(KeyCode.RightArrow))
            // {
            //     CheckForWinningCondition(player2HandManager);
            // }
        }

        // public void DrawCard(HandManager handManager)
        // {
        //     if (allCards.Count == 0)
        //     {
        //         return;
        //     }
        //
        //     CardData nextCard = allCards[_currentIndex];
        //     handManager.AddCardToHand(nextCard);
        //     _currentIndex = (_currentIndex + 1) % allCards.Count;
        // }

        public void RemoveCard(HandManager handManager)
        {
            if (allCards.Count == 0)
            {
                return;
            }

            handManager.RemoveCardFromHand();
            _currentIndex = (_currentIndex - 1 + allCards.Count) % allCards.Count;
        }
        
        public void DistributeCards()
        {
            List<GameObject> shuffledDeck = new List<GameObject>(_allCardsInScene);
            shuffledDeck = ShuffleDeck(shuffledDeck);

            // player1HandManager.ClearHand();
            // player2HandManager.ClearHand();

            for (int i = 0; i < shuffledDeck.Count; i++)
            {
                if (i % 2 == 0)
                    player1HandManager.AddCardToHand(shuffledDeck[i]);
                else
                    player2HandManager.AddCardToHand(shuffledDeck[i]);
            }
        }

        private List<GameObject> ShuffleDeck(List<GameObject> deck)
        {
            System.Random rng = new System.Random();
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (deck[n], deck[k]) = (deck[k], deck[n]);
            }
            return deck;
        }
        
        public void PlayRound(HandManager handManager)
        {
            GameObject card = handManager.PlayTopCard();

            if (card)
                MoveCardToPile(card);
        }

        private void MoveCardToPile(GameObject card)
        {
            // card.transform.SetParent(centerPileHandManager.transform); // Move card under the pile
            // card.transform.localPosition = Vector3.zero; // Set position properly
            // card.transform.rotation = Quaternion.identity; // Reset rotation
            centerPileHandManager.AddCardToHand(card);
        }
        
        public void CheckForWinningCondition(HandManager playerHandManager)
        {
            TakePile(playerHandManager);
        }

        private void TakePile(HandManager winnerHand)
        {
            Debug.Log($"Currently {centerPileHandManager.GetCardCount()} cards in center!");
            while (centerPileHandManager.GetCardCount() > 0)
            {
                GameObject card = centerPileHandManager.PlayTopCard();
                if (card)
                    winnerHand.AddCardToHand(card);
                Debug.Log($"Currently {centerPileHandManager.GetCardCount()} cards in center!");
            }
        }
    }
}
