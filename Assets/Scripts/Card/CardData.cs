using UnityEngine;

namespace Card
{
    public enum CardType
    {
        Number,
        Jack,
        Queen,
        King,
        Ace
    }

    [CreateAssetMenu (fileName = "NewCard", menuName = "Card")]
    public class CardData : ScriptableObject
    {
        [SerializeField] private string cardName;
        [SerializeField] private CardType cardType;
        [SerializeField] private int value;
        [SerializeField] private Texture2D frontTexture;
        [SerializeField] private Texture2D backTexture;
    
        private int _cardsToPlace;

        public CardData()
        {
            switch (cardType)
            {
                case CardType.Number:
                    _cardsToPlace = 0;
                    break;
                case CardType.Jack:
                    _cardsToPlace = 1;
                    break;
                case CardType.Queen:
                    _cardsToPlace = 2;
                    break;
                case CardType.King:
                    _cardsToPlace = 3;
                    break;
                case CardType.Ace:
                    _cardsToPlace = 4;
                    break;
            }
        }
        
        public Texture2D GetFrontTexture()
        {
            return frontTexture;
        }
        
        public Texture2D GetBackTexture()
        {
            return backTexture;
        }
    }
}