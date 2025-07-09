using network;
using TMPro;
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
        [FormerlySerializedAs("changeMarineButton")][SerializeField] private Button randomButton;
        [FormerlySerializedAs("changeNinjaButton")][SerializeField] private Button mageButton;
        [FormerlySerializedAs("changeZombieButton")][SerializeField] private Button godButton;
        [SerializeField] private Button leaveLobbyButton;
        [SerializeField] private Button changeGameModeButton;
        [SerializeField] private Button launchGameButton;

        private TextMeshProUGUI _launchText;

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

                if (LobbyManager.Instance.IsLobbyHost())
                {
                    GameMultiplayer.Instance.StartHost();
                }
                else
                {
                    GameMultiplayer.Instance.StartClient();
                }
            }
        }

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            _launchText = launchGameButton.GetComponentInChildren<TextMeshProUGUI>();

            playerSingleTemplate.gameObject.SetActive(false);

            randomButton.onClick.AddListener(() =>
            {
                PlayerManager.Instance.UpdatePlayerCharacter(PlayerCharacter.Random);
            });
            mageButton.onClick.AddListener(() =>
            {
                PlayerManager.Instance.UpdatePlayerCharacter(PlayerCharacter.Survivor);
            });
            godButton.onClick.AddListener(() =>
            {
                PlayerManager.Instance.UpdatePlayerCharacter(PlayerCharacter.God);
            });

            leaveLobbyButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.LeaveLobby();
            });

            launchGameButton.onClick.AddListener(() =>
            {
                var players = LobbyManager.Instance.GetJoinedLobby().Players;
                if (players.Count != 2)
                {
                    launchGameButton.interactable = false;
                    return;
                }

                var player1 = players[0];
                var player2 = players[1];
                string role1 = player1.Data[PlayerKey.PLAYER_CHARACTER].Value;
                string role2 = player2.Data[PlayerKey.PLAYER_CHARACTER].Value;
                string localPlayerId = AuthenticationService.Instance.PlayerId;

                bool isSurvivorHost =
                    (role1 == "Survivor" && player1.Id == localPlayerId) ||
                    (role2 == "Survivor" && player2.Id == localPlayerId);

                Debug.Log($"Player 1: {player1.Id} - Role: {role1}");
                Debug.Log($"Player 2: {player2.Id} - Role: {role2}");
                if (role1 == "Random" && role2 == "Random")
                {
                    bool flip = UnityEngine.Random.value < 0.5f;
                    string newRole1 = flip ? "God" : "Survivor";
                    string newRole2 = flip ? "Survivor" : "God";

                    Debug.Log($"Assigning roles: Player 1: {newRole1}, Player 2: {newRole2}");
                    PlayerManager.Instance.UpdatePlayerCharacter(
                        player1.Id == localPlayerId ?
                        (newRole1 == "God" ? PlayerCharacter.God : PlayerCharacter.Survivor) :
                        (newRole2 == "God" ? PlayerCharacter.God : PlayerCharacter.Survivor)
                    );


                    Debug.Log("Roles assigned, updating lobby status.");
                    if (isSurvivorHost)
                    {
                        Debug.Log("Host is Survivor, setting relay host connection.");
                        if (LobbyManager.Instance.ArePlayersReady())
                        {
                            Debug.Log("setRelayHostData");
                            NetworkConnection.SetRelayHostConnection();
                        }

                        return;
                    }

                    PlayerManager.Instance.UpdateReadyStatus(false);
                    _launchText.text = PlayerManager.Instance.GetPlayerStatus();
                    return;
                }

                if (role1 == "Random" && (role2 == "God" || role2 == "Survivor"))
                {
                    string newRole1 = role2 == "God" ? "Survivor" : "God";

                    if (player1.Id == localPlayerId)
                        PlayerManager.Instance.UpdatePlayerCharacter(newRole1 == "God" ? PlayerCharacter.God : PlayerCharacter.Survivor);

                    if (isSurvivorHost)
                    {
                        if (LobbyManager.Instance.ArePlayersReady())
                        {
                            Debug.Log("setRelayHostData");
                            NetworkConnection.SetRelayHostConnection();
                        }

                        return;
                    }

                    PlayerManager.Instance.UpdateReadyStatus(false);
                    _launchText.text = PlayerManager.Instance.GetPlayerStatus();
                    return;
                }

                if (role2 == "Random" && (role1 == "God" || role1 == "Survivor"))
                {
                    string newRole2 = role1 == "God" ? "Survivor" : "God";

                    if (player2.Id == localPlayerId)
                        PlayerManager.Instance.UpdatePlayerCharacter(newRole2 == "God" ? PlayerCharacter.God : PlayerCharacter.Survivor);

                    if (isSurvivorHost)
                    {
                        if (LobbyManager.Instance.ArePlayersReady())
                        {
                            Debug.Log("setRelayHostData");
                            NetworkConnection.SetRelayHostConnection();
                        }

                        return;
                    }

                    PlayerManager.Instance.UpdateReadyStatus(false);
                    _launchText.text = PlayerManager.Instance.GetPlayerStatus();
                    return;
                }

                if (role1 == role2)
                {
                    launchGameButton.interactable = false;
                    PlayerManager.Instance.UpdateReadyStatus(false);
                    return;
                }

                launchGameButton.interactable = true;

                if (isSurvivorHost)
                {
                    if (LobbyManager.Instance.ArePlayersReady())
                    {
                        Debug.Log("setRelayHostData");
                        NetworkConnection.SetRelayHostConnection();
                    }

                    return;
                }

                PlayerManager.Instance.UpdateReadyStatus(false);
                _launchText.text = PlayerManager.Instance.GetPlayerStatus();
            });

            changeGameModeButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.ChangeGameMode();
            });
        }

        private void Start()
        {
            LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
            LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
            LobbyManager.Instance.OnLobbyGameModeChanged += UpdateLobby_Event;
            LobbyManager.Instance.OnRelayCodeGiven += JoinIfClient_Event;
            LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
            LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnLeftLobby;

            Hide();
        }

        private void OnDestroy()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.OnJoinedLobby -= UpdateLobby_Event;
                LobbyManager.Instance.OnJoinedLobbyUpdate -= UpdateLobby_Event;
                LobbyManager.Instance.OnLobbyGameModeChanged -= UpdateLobby_Event;
                LobbyManager.Instance.OnRelayCodeGiven -= JoinIfClient_Event;
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

        private void JoinIfClient_Event(object sender, LobbyManager.LobbyEventArgs e)
        {
            JoinIfClient();
        }
        private void JoinIfClient()
        {
            Debug.Log("New Relay Code : " + LobbyManager.Instance.GetRelayCode.Value);
            if (!LobbyManager.Instance.IsLobbyHost() && LobbyManager.Instance.GetRelayCode.Value != "")
            {
                NetworkConnection.SetRelayClientConnection();
            }
        }
        private void UpdateLobby()
        {
            UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
        }

        private void UpdateLobby(Lobby lobby)
        {
            if (lobby == null) return;

            if (!LobbyManager.Instance.IsLobbyHost() && LobbyManager.Instance.GetRelayCode.Value != "")
            {
                JoinIfClient();
                return;
            }

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

                lobbyPlayerSingleUI.UpdatePlayerUI(player);

                if (LobbyManager.Instance.IsLobbyHost())
                {
                    _launchText.text = LobbyManager.Instance.ArePlayersReady() ? "Launch" : "Waiting Players";
                    _launchText.color = LobbyManager.Instance.ArePlayersReady() ? Color.black : Color.gray;
                }
            }

            changeGameModeButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());

            lobbyNameText.text = lobby.Name;
            playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
            gameModeText.text = lobby.Data[LobbyKey.GAME_MODE].Value;

            Show();
        }

        private void ClearLobby()
        {
            foreach (Transform child in container)
            {
                if (child == playerSingleTemplate) continue;
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