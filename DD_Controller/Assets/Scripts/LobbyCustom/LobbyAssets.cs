using UnityEngine;

namespace LobbyCustom
{
    public class LobbyAssets : MonoBehaviour {
        public static LobbyAssets Instance { get; private set; }
        
        [SerializeField] private Sprite marineSprite;
        [SerializeField] private Sprite ninjaSprite;
        [SerializeField] private Sprite zombieSprite;
        
        private void Awake() {
            Instance = this;
        }

        public Sprite GetSprite(PlayerCharacter playerCharacter) {
            switch (playerCharacter) {
                default:
                case PlayerCharacter.Random:   return marineSprite;
                case PlayerCharacter.Survivor:    return ninjaSprite;
                case PlayerCharacter.God:   return zombieSprite;
            }
        }

    }
}