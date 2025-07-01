using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LobbyCustom
{
    public class GameMultiplayer : NetworkBehaviour
    {
        private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";
        
        public const int MAX_PLAYER_AMOUNT = 2;
        [SerializeField] private NetworkTransport networkTransport;
    
        public static GameMultiplayer Instance { get; private set; }

        public event EventHandler OnTryingToJoinGame;
        public event EventHandler OnFailedToJoinGame;
        public event EventHandler OnPlayerDataNetworkListChanged;

        private NetworkList<PlayerData> _playerDataNetworkList;
        private string _playerName;
        
        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            _playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(100, 1000));

            _playerDataNetworkList = new NetworkList<PlayerData>();
            _playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
        }

        public string GetPlayerName()
        {
            return _playerName;
        }

        public void SetPlayerName(string playerName)
        {
            this._playerName = playerName;

            PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
        }

        private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
        }

        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartHost();
        
            NetworkManager.Singleton.SceneManager.LoadScene(SceneList.SceneNames[SceneName.Multiplayer], LoadSceneMode.Single);
        }


        private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
        {
            for (int i = 0; i < _playerDataNetworkList.Count; i++)
            {
                PlayerData playerData = _playerDataNetworkList[i];
                if (playerData.clientId == clientId)
                {
                    _playerDataNetworkList.RemoveAt(i);
                }
            }
        }

        private void NetworkManager_OnClientConnectedCallback(ulong clientId)
        {
            _playerDataNetworkList.Add(new PlayerData
            {
                clientId = clientId
            });
            SetPlayerNameServerRpc(GetPlayerName());
            SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        }

        private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
        {
            if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
            {
                connectionApprovalResponse.Approved = false;
                connectionApprovalResponse.Reason = "Game is full";
                return;
            }

            connectionApprovalResponse.Approved = true;
        }

        public void StartClient()
        {
            OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback; 
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;


            //((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).SetConnectionData(default);
        
            Debug.Log("0");
            NetworkManager.Singleton.StartClient();
            Debug.Log("1");
        }

        private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
        {
            SetPlayerNameServerRpc(GetPlayerName());
            SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
        {
            int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

            PlayerData playerData = _playerDataNetworkList[playerDataIndex];

            playerData.playerName = playerName;

            _playerDataNetworkList[playerDataIndex] = playerData;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
        {
            int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

            PlayerData playerData = _playerDataNetworkList[playerDataIndex];

            playerData.playerId = playerId;

            _playerDataNetworkList[playerDataIndex] = playerData;
        }

        private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
        {
            OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
        }

        public bool IsPlayerIndexConnected(int playerIndex)
        {
            return playerIndex < _playerDataNetworkList.Count;
        }

        public int GetPlayerDataIndexFromClientId(ulong clientId)
        {
            for (int i = 0; i < _playerDataNetworkList.Count; i++)
            {
                if (_playerDataNetworkList[i].clientId == clientId)
                {
                    return i;
                }
            }
            return -1;
        }

        public PlayerData GetPlayerDataFromClientId(ulong clientId)
        {
            foreach (PlayerData playerData in _playerDataNetworkList)
            {
                if (playerData.clientId == clientId)
                {
                    return playerData;
                }
            }
            return default;
        }

        public PlayerData GetPlayerData()
        {
            return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
        }

        public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
        {
            return _playerDataNetworkList[playerIndex];
        }

        public void KickPlayer(ulong clientId)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            NetworkManager_Server_OnClientDisconnectCallback(clientId);
        }
    }
}
