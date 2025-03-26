using System.Collections.Generic;
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
        
        [SerializeField] private int maxHandSize = 5;
        
        private HandForm handForm = HandForm.Fan; // change form from fan to pile
        [SerializeField] private List<GameObject> cardsInHand = new List<GameObject>();
    
        void Start()
        {
            AddCardToHand();
            AddCardToHand();
            AddCardToHand();
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RemoveCardFromHand();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                AddCardToHand();
            }
            UpdateHandVisuals();
        }

        private void AddCardToHand()
        {
            GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
            cardsInHand.Add(newCard);

            UpdateHandVisuals();
        }

        private void RemoveCardFromHand()
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

            if (cardCount == 1)
            {
                cardsInHand[0].transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                cardsInHand[0].transform.localPosition = Vector3.zero;
                return;
            }
            
            for (int i = 0; i < cardCount; ++i)
            {
                cardsInHand[i].GetComponent<Card>().SetVisibility(true);
                float rotationAngle = fanSpread * (i - (cardCount - 1) / 2f);
                cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 180f, rotationAngle); // 180 to be flipped

                float horizontalOffset = fanCardSpacing * (i - (cardCount - 1) / 2f);
                
                float normalizedPosition = 2f * i / (cardCount - 1) - 1f;
                float verticalOffset = verticalSpacing * (1 - normalizedPosition * normalizedPosition);
                cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, verticalOffset, (cardCount - i) * 0.2f);
            }
        }

        private void CreatePile()
        {
            int cardCount = cardsInHand.Count;

            for (int i = 0; i < cardCount - maxHandSize; ++i)
            {
                cardsInHand[i].GetComponent<Card>().SetVisibility(false);
            }
            
            for (int i = cardCount - maxHandSize; i < cardCount; ++i)
            {
                int i2 = i - (cardCount - maxHandSize);
                
                cardsInHand[i].GetComponent<Card>().SetVisibility(true);
                cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 180f, 0f); // 180 to be flipped

                float horizontalOffset = pileCardSpacing * (i2 - (maxHandSize - 1) / 2f);
                cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, 0f, (maxHandSize - i2) * 0.2f);
            }
        }
    }
}
