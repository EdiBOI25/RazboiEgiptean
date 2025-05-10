using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;


namespace Card
{
    public class InGamePlayer : MonoBehaviour
    {
        public string playerName;
        public int connectionId;
        public ulong playerSteamId;
        private bool _avatarReceived = false;

        public TextMeshProUGUI playerNameText;
        public RawImage playerIcon;
        
        protected Callback<AvatarImageLoaded_t> AvatarImageLoaded;

        private void Start()
        {
            AvatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
        
        }

        void GetPlayerIcon()
        {
            int imageId = SteamFriends.GetLargeFriendAvatar((CSteamID)playerSteamId);
            if (imageId == -1)
            {
                Debug.Log("Failed to get avatar");
                return;
            }

            playerIcon.texture = GetSteamImageAsTexture(imageId);
        }

        public void SetPlayerValues()
        {
            playerNameText.text = playerName;
            if (!_avatarReceived)
            {
                GetPlayerIcon();
            }
        }

        private Texture2D GetSteamImageAsTexture(int iImage)
        {
            Texture2D texture = null;
            bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

            if (!isValid)
            {
                Debug.Log("Failed to get image size");
                return null;
            }
        
            byte[] image = new byte[4 * width * height];
            isValid = SteamUtils.GetImageRGBA(iImage, image, 4 * (int)width * (int)height);
        
            if (!isValid)
            {
                Debug.Log("Failed to get image data");
                return null;
            }
        
            texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, false);
            texture.LoadRawTextureData(image);
            texture.Apply();
        
            _avatarReceived = true;
            return texture;
        }
    
        private void OnImageLoaded(AvatarImageLoaded_t param)
        {
            if (param.m_steamID.m_SteamID == playerSteamId)
            {
                playerIcon.texture = GetSteamImageAsTexture(param.m_iImage);
            }
            else
            {
                return;
            }
        }
        
        // void PlayTopCard(){}
        // void ReceiveCards(List<Card>){}
    }
}
