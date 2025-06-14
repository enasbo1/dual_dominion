using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{


    public static LobbyUI Instance { get; private set; }
    private bool isSurvivor = false;
    private bool isReady = false;


    [SerializeField] private Transform playerSingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI gameModeText;
    [Header("Menu UI Buttons")]
    [SerializeField] private Button changeMarineButton;
    [SerializeField] private Button changeNinjaButton;
    [SerializeField] private Button changeZombieButton;
    [SerializeField] private Button leaveLobbyButton;
    [SerializeField] private Button changeGameModeButton;
    [SerializeField] private Button launchGameButton;
     
    /*
    [Header("Scenes to Loads")]
    public SceneAsset multiPlayerScene;
    public SceneAsset monoPlayerScene;
    */
    
    private const string MONO_PLAYER_SCENE = "MonoPlayerScene";

    private void LoadNextScene()
    {
        if (playerCountText.text.StartsWith("1"))
        {
            SceneManager.LoadScene(MONO_PLAYER_SCENE);
        }
        else if (playerCountText.text.StartsWith("2"))
        {
            Lobby lobby = LobbyManager.Instance.GetJoinedLobby();

            foreach (Player player in lobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId && isSurvivor)
                {
                    GameMultiplayer.Instance.StartHost();
                }
            }

            foreach (Player player in lobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId && !isSurvivor)
                {
                    GameMultiplayer.Instance.StartClient();
                }
            }
        }
    }

    public void ReadyToPlay()
    {
        if (playerCountText.text.StartsWith("2")) if (LobbyManager.Instance.IsLobbyHost() && NetworkManager.Singleton.ConnectedClients.Count > 1) launchGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Launch";
        else launchGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Launch";
    }

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this.gameObject);

        playerSingleTemplate.gameObject.SetActive(false);

        changeMarineButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Marine);
        });
        changeNinjaButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Ninja);
            isSurvivor = true;
        });
        changeZombieButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Zombie);
            isSurvivor = false;
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
        isReady = !isReady;
        UpdateReadyButtonText();
    }

    private void UpdateReadyButtonText()
    {
        launchGameButton.GetComponentInChildren<TextMeshProUGUI>().text = isReady ? "Ready" : "Not Ready";
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
                Debug.LogWarning("playerSingleTemplate est null ou a �t� d�truit !");
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
        gameModeText.text = lobby.Data[LobbyManager.KEY_GAME_MODE].Value;

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