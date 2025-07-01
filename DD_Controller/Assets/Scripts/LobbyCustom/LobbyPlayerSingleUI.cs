using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace LobbyCustom
{
    public class LobbyPlayerSingleUI : MonoBehaviour {
        
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI playerStatusText;
        [SerializeField] private Image characterImage;
        [SerializeField] private Button kickPlayerButton;
        
        private Player _player;

        private void Awake()
        {
            // Debug.Log("test");
            // Debug.Log(playerUI);
            // Debug.Log(_playerNameText);
            // Debug.Log(_playerStatusText);
            // Debug.Log(_characterImage);
            // Debug.Log(_kickPlayerButton);

            if (kickPlayerButton == null) return;
            kickPlayerButton.onClick.AddListener(KickPlayer);
        }

        public void SetKickPlayerButtonVisible(bool visible) {
            kickPlayerButton.gameObject.SetActive(visible);
        }

        public void UpdatePlayer(Player player) {
            this._player = player;
        
            playerNameText.text = player.Data[LobbyManager.PLAYER_KEYS.KEY_PLAYER_NAME].Value;
            playerStatusText.text = player.Data[LobbyManager.PLAYER_KEYS.KEY_READY].Value;
            LobbyManager.PlayerCharacter playerCharacter = 
                System.Enum.Parse<LobbyManager.PlayerCharacter>(player.Data[LobbyManager.PLAYER_KEYS.KEY_PLAYER_CHARACTER].Value);
            characterImage.sprite = LobbyAssets.Instance.GetSprite(playerCharacter);
        }

        private void KickPlayer() {
            if (_player != null) {
                LobbyManager.Instance.KickPlayer(_player.Id);
            }
        }
    }
}