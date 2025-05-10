using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Card
{
    public enum HandForm
    {
        Fan,
        Pile
    }
    public class HandManager : MonoBehaviour
    {
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private Transform handTransform; // root of hand position
        
        [SerializeField] private float fanSpread = 5f; // spread of cards in hand
        [SerializeField] private float fanCardSpacing = 5f;
        [SerializeField] private float pileCardSpacing = 0.5f;
        [SerializeField] private float verticalSpacing = 1.5f;
        
        [SerializeField] private int maxHandSize = 26;
        [SerializeField] private bool isCenterPile = false;
        private float _flipRotation = 180f; // rotation to flip card
        
        private HandForm handForm = HandForm.Fan; // change form from fan to pile
        [SerializeField] private List<GameObject> cardsInHand = new List<GameObject>();

        [SerializeField] private TextMeshProUGUI cardCountText;
    
        void Start()
        {
            
        }
        
        void Update()
        {
            
            
            // UpdateHandVisuals();
        }

        public int GetCardCount()
        {
            return this.cardsInHand.Count;
        }

        private void Awake()
        {
            if (isCenterPile == false)
            {
                _flipRotation = 180f;
            }
            else
            {
                _flipRotation = 0f;
            }
        }

        public void AddCardToHand(GameObject card)
        {
            // GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
            // newCard.GetComponent<Card>().SetCardData(cardData);
            cardsInHand.Add(card);
            card.transform.position = handTransform.position;
            card.transform.parent = handTransform;
            card.transform.rotation = Quaternion.identity;
            card.transform.localPosition = new Vector3(0f, 0f, 0f);
            card.transform.localRotation = Quaternion.identity;

            UpdateHandVisuals();
        }

        public void RemoveCardFromHand()
        {
            if (cardsInHand.Count == 0)
            {
                return;
            }
            GameObject cardToRemove = cardsInHand[^1];
            cardsInHand.RemoveAt(cardsInHand.Count - 1);
            Destroy(cardToRemove);
            
            UpdateHandVisuals();
        }

        private void UpdateHandVisuals()
        {
            cardCountText.text = cardsInHand.Count.ToString();
            
            if (isCenterPile == true)
            {
                handForm = HandForm.Pile;
                CreatePile();
                return;
            }
            int cardCount = cardsInHand.Count;
            
            if (cardCount <= maxHandSize)
            {
                handForm = HandForm.Fan;
                CreateFan();
            }
            else
            {
                handForm = HandForm.Pile;
                CreatePile();
            }
        }

        private void CreateFan()
        {
            int cardCount = cardsInHand.Count;
            if (cardCount == 0)
                return;

            if (cardCount == 1)
            {
                cardsInHand[0].transform.localRotation = Quaternion.Euler(0f, _flipRotation, 0f);
                cardsInHand[0].transform.localPosition = Vector3.zero;
                // cardsInHand[0].GetComponent<Card>().SetVisibility(true);
                return;
            }
            
            for (int i = 0; i < cardCount; ++i)
            {
                cardsInHand[i].GetComponent<Card>().SetVisibility(true);
                float rotationAngle = fanSpread * (i - (cardCount - 1) / 2f);
                cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, _flipRotation, rotationAngle); // 180 to be flipped

                float horizontalOffset = fanCardSpacing * (i - (cardCount - 1) / 2f);
                
                float normalizedPosition = 2f * i / (cardCount - 1) - 1f;
                float verticalOffset = verticalSpacing * (1 - normalizedPosition * normalizedPosition);
                cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, verticalOffset, (cardCount - i) * 0.2f);
            }
        }

        private void CreatePile()
        {
            int cardCount = cardsInHand.Count;
            if (cardCount == 0)
                return;
            
            if (cardCount <= maxHandSize)
            {
                for (int i = 0; i < cardCount; ++i)
                {
                    int i2 = i;
                
                    cardsInHand[i].GetComponent<Card>().SetVisibility(true);
                    cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, _flipRotation, 0f); // 180 to be flipped

                    float horizontalOffset = pileCardSpacing * (i2 - (maxHandSize - 1) / 2f);
                    cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, 0f, (maxHandSize - i2) * 0.3f);
                }

                if (isCenterPile)
                {
                    cardsInHand[^1].transform.localPosition += new Vector3(3f, 0f, 0f);
                }

                return;
            }

            for (int i = 0; i < cardCount - maxHandSize; ++i)
            {
                cardsInHand[i].GetComponent<Card>().SetVisibility(false);
            }
            
            for (int i = cardCount - maxHandSize; i < cardCount; ++i)
            {
                int i2 = i - (cardCount - maxHandSize);
                
                cardsInHand[i].GetComponent<Card>().SetVisibility(true);
                cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, _flipRotation, 0f); // 180 to be flipped

                float horizontalOffset = pileCardSpacing * (i2 - (maxHandSize - 1) / 2f);
                cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, 0f, (maxHandSize - i2) * 0.3f);
            }

            if (isCenterPile)
            {
                cardsInHand[^1].transform.localPosition += new Vector3(3f, 0f, 0f);
            }
        }
        
        public GameObject PlayTopCard()
        {
            if (cardsInHand.Count == 0)
                return null;

            GameObject cardToPlay = cardsInHand[0];
            cardsInHand.RemoveAt(0);
            UpdateHandVisuals();
    
            // Instead of destroying, move it to a new position
            return cardToPlay; // Return the card GameObject so we can use it in DeckManager
        }
    }
}
