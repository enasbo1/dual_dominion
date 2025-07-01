using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LobbyCustom
{
    public class LobbyCreateUI : MonoBehaviour {
        public static LobbyCreateUI Instance { get; private set; }
        
        [SerializeField] private Button createButton;
        [SerializeField] private Button lobbyNameButton;
        [SerializeField] private Button publicPrivateButton;
        [SerializeField] private Button maxPlayersButton;
        [SerializeField] private Button gameModeButton;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI publicPrivateText;
        [SerializeField] private TextMeshProUGUI maxPlayersText;
        [SerializeField] private TextMeshProUGUI gameModeText;
        
        private string _lobbyName;
        private bool _isPrivate;
        private int _maxPlayers;
        private LobbyManager.GameMode _gameMode;

        private void Awake() {
            Instance = this;

            createButton.onClick.AddListener(() => {
                LobbyManager.Instance.CreateLobby(
                    _lobbyName,
                    _maxPlayers,
                    _isPrivate,
                    _gameMode
                );
                Hide();
            });

            lobbyNameButton.onClick.AddListener(() => {
                UI_InputWindow.Show_Static("Lobby Name", _lobbyName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-1234567890", 20,
                    () => {
                        // Cancel
                    },
                    (string lobbyName) => {
                        this._lobbyName = lobbyName;
                        UpdateText();
                    });
            });

            publicPrivateButton.onClick.AddListener(() => {
                _isPrivate = !_isPrivate;
                UpdateText();
            });

            maxPlayersButton.onClick.AddListener(() => {
                UI_InputWindow.Show_Static("Max Players", _maxPlayers,
                    () => {
                        // Cancel
                    },
                    (int maxPlayers) => {
                        this._maxPlayers = maxPlayers;
                        UpdateText();
                    });
            });

            gameModeButton.onClick.AddListener(() => {
                switch (_gameMode) {
                    default:
                    case LobbyManager.GameMode.PvP:
                        _gameMode = LobbyManager.GameMode.PvP;
                        break;
                }
                UpdateText();
            });

            Hide();
        }

        private void UpdateText() {
            lobbyNameText.text = _lobbyName;
            publicPrivateText.text = _isPrivate ? "Private" : "Public";
            maxPlayersText.text = _maxPlayers.ToString();
            gameModeText.text = _gameMode.ToString();
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        public void Show() {
            gameObject.SetActive(true);

            _lobbyName = "MyLobby";
            _isPrivate = false;
            _maxPlayers = 2;
            _gameMode = LobbyManager.GameMode.PvP;

            UpdateText();
        }

    }
}