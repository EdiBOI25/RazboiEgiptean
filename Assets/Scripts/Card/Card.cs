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

        public void Start()
        {
            frontQuad = transform.GetChild(0).GameObject();
            backQuad = transform.GetChild(1).GameObject();
            
            frontQuad.GetComponent<MeshRenderer>().material.mainTexture = cardData.GetFrontTexture();
            backQuad.GetComponent<MeshRenderer>().material.mainTexture = cardData.GetBackTexture();
        }

        public void Flip()
        {
            isFaceUp = !isFaceUp;
            float targetRotation = isFaceUp ? 0f : 180f;
            StartCoroutine(FlipAnimation(targetRotation));
        }

        private System.Collections.IEnumerator FlipAnimation(float targetRotation)
        {
            float duration = 1f; // Flip speed
            float elapsed = 0f;
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = Quaternion.Euler(targetRotation, 0, 0);

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
