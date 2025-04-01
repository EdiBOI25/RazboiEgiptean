using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Card
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private CardData cardData;
        private bool isFaceUp = false;

        private GameObject frontQuad;
        private GameObject backQuad;

        public void Awake()
        {
            frontQuad = transform.GetChild(0).GameObject();
            backQuad = transform.GetChild(1).GameObject();
            
            UpdateTextures();
        }

        private void UpdateTextures()
        {
            frontQuad.GetComponent<MeshRenderer>().material.mainTexture = cardData.GetFrontTexture();
            backQuad.GetComponent<MeshRenderer>().material.mainTexture = cardData.GetBackTexture();
        }

        private void RefreshCard()
        {
            UpdateTextures();;
        }

        public void SetCardData(CardData newCardData)
        {
            this.cardData = newCardData;
            RefreshCard();
        }

        public CardData GetCardData()
        {
            return this.cardData;
        }

        public void Flip()
        {
            isFaceUp = !isFaceUp;
            float targetRotation = isFaceUp ? 0f : 180f;
            StartCoroutine(FlipAnimation(targetRotation));
        }
        
        public void SetVisibility(bool isVisible)
        {
            frontQuad.GetComponent<MeshRenderer>().enabled = isVisible;
            backQuad.GetComponent<MeshRenderer>().enabled = isVisible;
        }

        private System.Collections.IEnumerator FlipAnimation(float targetRotation)
        {
            float duration = 0.2f; // Flip speed
            float elapsed = 0f;
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = Quaternion.Euler(targetRotation, startRotation.y, startRotation.z);

            while (elapsed < duration)
            {
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.rotation = endRotation;
        }
    }
}
