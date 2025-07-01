using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LobbyCustom
{
    public class LobbyUI : MonoBehaviour
    {
        public static LobbyUI Instance { get; private set; }
        [SerializeField] private Transform playerSingleTemplate;
        [SerializeField] private Transform container;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI playerCountText;
        [SerializeField] private TextMeshProUGUI gameModeText;
    
        [Header("Menu UI Buttons")]
        [FormerlySerializedAs("changeMarineButton")] [SerializeField] private Button randomButton;
        [FormerlySerializedAs("changeNinjaButton")] [SerializeField] private Button mageButton;
        [FormerlySerializedAs("changeZombieButton")] [SerializeField] private Button godButton;
        [SerializeField] private Button leaveLobbyButton;
        [SerializeField] private Button changeGameModeButton;
        [SerializeField] private Button launchGameButton;
    
        private bool _isSurvivor;
        private bool _isReady;
        /*
    [Header("Scenes to Loads")]
    public SceneAsset multiPlayerScene;
    public SceneAsset monoPlayerScene;
    */
    
        private void LoadNextScene()
        {
            if (playerCountText.text.StartsWith("1"))
            {
                SceneManagerScript.ChangeToScene(SceneName.MonoPlayer);
                return;
            }
            Lobby lobby = LobbyManager.Instance.GetJoinedLobby();
        
            foreach (Player player in lobby.Players)
            {
                if (player.Id != AuthenticationService.Instance.PlayerId) continue;
            
                if (_isSurvivor)
                {
                    GameMultiplayer.Instance.StartHost();
                }
                else
                {
                    GameMultiplayer.Instance.StartClient();
                }
            }
        }
    
        private void CheckPlayerReady()
        {
            Lobby lobby = LobbyManager.Instance.GetJoinedLobby();
        
            foreach (Player player in lobby.Players)
            {
                Debug.Log($"Id {player.Id}, Data {player.Data}, ConnectionInfo {player.ConnectionInfo}, AllocationId {player.AllocationId}");
            
                if (player.Id != AuthenticationService.Instance.PlayerId) continue;
            
                if (_isSurvivor)
                {
                    GameMultiplayer.Instance.StartHost();
                }
                else
                {
                    GameMultiplayer.Instance.StartClient();
                }
            }
        }

        public void ReadyToPlay()
        {
            if (playerCountText.text.StartsWith("2")) 
                if (LobbyManager.Instance.IsLobbyHost() && NetworkManager.Singleton.ConnectedClients.Count > 1) launchGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Launch";
                else launchGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Launch";
        }

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(this.gameObject);

            playerSingleTemplate.gameObject.SetActive(false);

            randomButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Random);
            });
            mageButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Survivor);
                _isSurvivor = true;
            });
            godButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.God);
                _isSurvivor = false;
            });

            leaveLobbyButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.LeaveLobby();
            });

            launchGameButton.onClick.AddListener(() =>
            {
                //if (LobbyManager.Instance.IsLobbyHost() && launchGameButton.GetComponentInChildren<TextMeshProUGUI>().text == "Launch") LoadNextScene();
                //ToggleReadyState();
                //ReadyToPlay();

                LoadNextScene();
            });

            changeGameModeButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.ChangeGameMode();
            });
        }

        private void ToggleReadyState()
        {
            _isReady = !_isReady;
            UpdateReadyButtonText();
        }

        private void UpdateReadyButtonText()
        {
        
        }

        private void Start()
        {
            LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
            LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
            LobbyManager.Instance.OnLobbyGameModeChanged += UpdateLobby_Event;
            LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
            LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnLeftLobby;

            UpdateReadyButtonText();
            Hide();
        }

        private void OnDestroy()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.OnJoinedLobby -= UpdateLobby_Event;
                LobbyManager.Instance.OnJoinedLobbyUpdate -= UpdateLobby_Event;
                LobbyManager.Instance.OnLobbyGameModeChanged -= UpdateLobby_Event;
                LobbyManager.Instance.OnLeftLobby -= LobbyManager_OnLeftLobby;
                LobbyManager.Instance.OnKickedFromLobby -= LobbyManager_OnLeftLobby;
            }
        }


        private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e)
        {
            ClearLobby();
            Hide();
        }

        private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e)
        {
            UpdateLobby();
        }

        private void UpdateLobby()
        {
            UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
        }

        private void UpdateLobby(Lobby lobby)
        {
            ClearLobby();

            foreach (Player player in lobby.Players)
            {
                if (playerSingleTemplate == null)
                {
                    Debug.LogWarning("playerSingleTemplate is null or have been destroyed !");
                    return;
                }

                Transform playerSingleTransform = Instantiate(playerSingleTemplate, container);
                playerSingleTransform.gameObject.SetActive(true);
                LobbyPlayerSingleUI lobbyPlayerSingleUI = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

                lobbyPlayerSingleUI.SetKickPlayerButtonVisible(
                    LobbyManager.Instance.IsLobbyHost() &&
                    player.Id != AuthenticationService.Instance.PlayerId // Don't allow kick self
                );

                lobbyPlayerSingleUI.UpdatePlayer(player);
            }   

            changeGameModeButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());

            lobbyNameText.text = lobby.Name;
            playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
            gameModeText.text = lobby.Data[LobbyManager.PLAYER_KEYS.KEY_GAME_MODE].Value;

            Show();
        }

        private void ClearLobby()
        {
            if (container == null) return;

            foreach (Transform child in container)
            {
                if (child == null || child == playerSingleTemplate) continue;
                Destroy(child.gameObject);
            }
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }
    }
}