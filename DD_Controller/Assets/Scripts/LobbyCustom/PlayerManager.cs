using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace LobbyCustom
{
    public static class PlayerKey
    {
        public const string PLAYER_NAME = "PlayerName";
        public const string PLAYER_CHARACTER = "Character";
        public const string READY = "Ready";
    }

    public static class LobbyKey
    {
        public const string GAME_MODE = "GameMode";
        public const string RELAY_CODE = "RelayCode";
    }
        
    
    public enum PlayerCharacter {
        Random,
        Survivor,
        God
    }
    
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }
        
        private static string _playerName = "PlayerName";
        private static string _playerStatus = "Not Ready";
        
        private void Awake() {
            Instance = this;
        }
        
        public async void Authenticate(string playerName) {
            _playerName = playerName;
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(playerName);

            await UnityServices.InitializeAsync(initializationOptions);

            AuthenticationService.Instance.SignedIn += () => {
                // do nothing
                Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        
        public Player GetPlayer() {
            return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
                { PlayerKey.PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName) },
                { PlayerKey.PLAYER_CHARACTER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerCharacter.Random.ToString()) },
                { PlayerKey.READY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerStatus) }
            });
        }
        
        public bool IsPlayerReady() {
            return _playerStatus == "Ready";
        }
        
        public string GetPlayerStatus() {
            return _playerStatus;
        }

        public void OnPlayerLobbyCreate()
        {
            _playerStatus = "Ready";
        }
        
        public void OnPlayerLobbyLeave()
        {
            _playerStatus = "Not Ready";
        }
        
        public async void UpdatePlayerName(string playerName)
        {
            if (LobbyManager.Instance.joinedLobby == null) return;
            
            _playerName = playerName;
            try {
                UpdatePlayerOptions options = new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>() {
                        {
                            PlayerKey.PLAYER_NAME, new PlayerDataObject(
                                visibility: PlayerDataObject.VisibilityOptions.Public,
                                value: playerName)
                        }
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                LobbyManager.Instance.joinedLobby = await LobbyService.Instance.UpdatePlayerAsync(LobbyManager.Instance.joinedLobby.Id, playerId, options);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
        
        public async void UpdateReadyStatus(bool status)
        {
            if (LobbyManager.Instance.joinedLobby == null) return;
            
            _playerStatus = status ? "Not Ready" : "Ready";
            try {
                UpdatePlayerOptions options = new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>() {
                        {
                            PlayerKey.READY, new PlayerDataObject(
                                visibility: PlayerDataObject.VisibilityOptions.Public,
                                value: _playerStatus)
                        }
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                LobbyManager.Instance.joinedLobby = await LobbyService.Instance.UpdatePlayerAsync(LobbyManager.Instance.joinedLobby.Id, playerId, options);
                
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }

        public async void UpdatePlayerCharacter(PlayerCharacter playerCharacter)
        {
            if (LobbyManager.Instance.joinedLobby == null) return;
            
            try {
                UpdatePlayerOptions options = new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>() {
                        {
                            PlayerKey.PLAYER_CHARACTER, new PlayerDataObject(
                                visibility: PlayerDataObject.VisibilityOptions.Public,
                                value: playerCharacter.ToString())
                        }
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                LobbyManager.Instance.joinedLobby = await LobbyService.Instance.UpdatePlayerAsync(LobbyManager.Instance.joinedLobby.Id, playerId, options);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }
}