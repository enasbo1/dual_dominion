using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace LobbyCustom
{
    public enum GameMode {
        PvP
    }
    
    public class LobbyManager : MonoBehaviour {
        public static LobbyManager Instance { get; private set; }
        
        [CanBeNull] public Lobby joinedLobby;
        
        public event EventHandler OnLeftLobby;
        public event EventHandler<LobbyEventArgs> OnJoinedLobby;
        public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
        public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
        public event EventHandler<LobbyEventArgs> OnLobbyGameModeChanged;
        public class LobbyEventArgs : EventArgs {
            public Lobby lobby;
        }

        public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
        public class OnLobbyListChangedEventArgs : EventArgs {
            public List<Lobby> lobbyList;
        }

        private float _heartbeatTimer;
        private float _lobbyPollTimer;
        private float _refreshLobbyListTimer = 5f;


        private void Awake() {
            Instance = this;
        }

        private void Update() {
            //HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds
            HandleLobbyHeartbeat();
            HandleLobbyPolling();
        }

        private void HandleRefreshLobbyList() {
            if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn) {
                _refreshLobbyListTimer -= Time.deltaTime;
                if (_refreshLobbyListTimer < 0f) {
                    float refreshLobbyListTimerMax = 5f;
                    _refreshLobbyListTimer = refreshLobbyListTimerMax;

                    RefreshLobbyList();
                }
            }
        }

        private async void HandleLobbyHeartbeat()
        {
            if (joinedLobby == null) return;
            
            if (IsLobbyHost()) {
                _heartbeatTimer -= Time.deltaTime;
                if (_heartbeatTimer < 0f) {
                    float heartbeatTimerMax = 15f;
                    _heartbeatTimer = heartbeatTimerMax;

                    // Debug.Log("Heartbeat");
                    await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                }
            }
        }

        private async void HandleLobbyPolling()
        {
            if (joinedLobby == null) return;
            
            _lobbyPollTimer -= Time.deltaTime;
            if (_lobbyPollTimer < 0f) {
                float lobbyPollTimerMax = 1.1f;
                _lobbyPollTimer = lobbyPollTimerMax;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                if (!IsPlayerInLobby()) {
                    // Player was kicked out of this lobby
                    // Debug.Log("Kicked from Lobby!");

                    OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                    joinedLobby = null;
                }
            }
        }

        public Lobby GetJoinedLobby() {
            return joinedLobby;
        }

        public bool IsLobbyHost() {
            return joinedLobby?.HostId == AuthenticationService.Instance.PlayerId;
        }

        private bool IsPlayerInLobby() {
            if (joinedLobby != null && joinedLobby.Players != null) {
                foreach (Player player in joinedLobby.Players) {
                    if (player.Id == AuthenticationService.Instance.PlayerId) {
                        // This player is in this lobby
                        return true;
                    }
                }
            }
            return false;
        }
        
        public bool ArePlayersReady()
        {
            if (joinedLobby == null) return false;
            return !joinedLobby.Players.Exists(player => player.Data[PLAYER_KEYS.KEY_READY].Value != "Ready");
        }

        public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate, GameMode gameMode) {
            PlayerManager.Instance.OnPlayerLobbyCreate();
            Player player = PlayerManager.Instance.GetPlayer();

            CreateLobbyOptions options = new CreateLobbyOptions {
                Player = player,
                IsPrivate = isPrivate,
                Data = new Dictionary<string, DataObject> {
                    { PLAYER_KEYS.KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });

            Debug.Log("Created Lobby " + lobby.Name);
        }

        public async void RefreshLobbyList() {
            try {
                QueryLobbiesOptions options = new QueryLobbiesOptions
                {
                    Count = 25,
                    // Filter for open lobbies only
                    Filters = new List<QueryFilter> {
                        new QueryFilter(
                            field: QueryFilter.FieldOptions.AvailableSlots,
                            op: QueryFilter.OpOptions.GT,
                            value: "0")
                    },
                    // Order by newest lobbies first
                    Order = new List<QueryOrder> {
                        new QueryOrder(
                            asc: false,
                            field: QueryOrder.FieldOptions.Created)
                    }
                };

                QueryResponse lobbyListQueryResponse = await LobbyService.Instance.QueryLobbiesAsync();

                OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
        
        public void ChangeGameMode()
        {
            if (joinedLobby == null) return;
            
            if (IsLobbyHost()) {
                GameMode gameMode =
                    Enum.Parse<GameMode>(joinedLobby.Data[PLAYER_KEYS.KEY_GAME_MODE].Value);

                switch (gameMode) {
                    default:
                    case GameMode.PvP:
                        gameMode = GameMode.PvP;
                        break;
                }

                UpdateLobbyGameMode(gameMode);
            }
        }
        
        private async void UpdateLobbyGameMode(GameMode gameMode) {
            if (joinedLobby == null) return;
            
            try {
                Debug.Log("UpdateLobbyGameMode " + gameMode);
            
                joinedLobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                    Data = new Dictionary<string, DataObject> {
                        { PLAYER_KEYS.KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) }
                    }
                });

                OnLobbyGameModeChanged?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }

        #region Join Lobby
        
        public async void JoinLobbyByCode(string lobbyCode) {
            Player player = PlayerManager.Instance.GetPlayer();

            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions {
                Player = player
            });

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        }

        public async void JoinLobby(Lobby lobby) {
            Player player = PlayerManager.Instance.GetPlayer();

            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions {
                Player = player
            });

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        
        public async void QuickJoinLobby() {
            try {
                QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

                joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);

                OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
        
        #endregion

        #region Leave Lobby
        
        public async void LeaveLobby()
        {
            if (joinedLobby == null) return;
            
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                PlayerManager.Instance.OnPlayerLobbyLeave();

                joinedLobby = null;

                OnLeftLobby?.Invoke(this, EventArgs.Empty);
                PlayerManager.Instance.OnPlayerLobbyLeave();
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }

        public async void KickPlayer(string playerId) {
            if (joinedLobby == null) return;
            
            if (IsLobbyHost()) {
                try {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
                    
                    OnLeftLobby?.Invoke(this, EventArgs.Empty);
                    PlayerManager.Instance.OnPlayerLobbyLeave();
                } catch (LobbyServiceException e) {
                    Debug.Log(e);
                }
            }
        }
        
        #endregion
    }
}