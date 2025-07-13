using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Menu
{
    public class PauseMenuScript : NetworkBehaviour
    {
        private readonly NetworkVariable<float> _syncedTimeScale = new NetworkVariable<float>(
            1f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        public PlayerInput playerInputs;

        [Range(0.01f, 3f)] public float neutralTimeFlow = 1f;
        [Range(0.001f, 3f)] public float pauseTimeFlow = 0.005f;

        public List<GameObject> objectsToDisable = new List<GameObject>();

        [SerializeField] private GameObject canvas;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button leavePartyButton;
        [SerializeField] private Button leaveGameButton;
        private bool _isPauseActive;

        private InputAction _pauseTrigger;
        private bool _isMultiplayer;

        [ServerRpc(RequireOwnership = false)]
        private void RequestPauseServerRpc(bool shouldPause)
        {
            _syncedTimeScale.Value = shouldPause ? pauseTimeFlow : neutralTimeFlow;
        }
        
        private void OnTimeScaleChanged(float oldFlow, float newFlow)
        {
            ApplyPause(newFlow, Math.Abs(newFlow - neutralTimeFlow) > 0.0012f);
        }
        
        private void ChangePause(bool shouldPause)
        {
            float newFlow = shouldPause ? pauseTimeFlow : neutralTimeFlow;

            ApplyPause(newFlow, shouldPause);
        }
        
        private void ApplyPause(float newFlow, bool shouldPause)
        {
            Time.timeScale = newFlow;
            _isPauseActive = shouldPause;
            canvas.SetActive(shouldPause);
            objectsToDisable.ForEach(x => x.SetActive(!shouldPause));
            
            Cursor.lockState = shouldPause && (!_isMultiplayer || (_isMultiplayer && IsServer)) ? CursorLockMode.None : CursorLockMode.Locked;
        }
        
        public void Start()
        {
            _isMultiplayer = NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient;
            Time.timeScale = neutralTimeFlow;
            
            resumeButton.onClick.AddListener(() =>
            {
                if (_isMultiplayer) RequestPauseServerRpc(false);
                else ChangePause(false);
            });

            leavePartyButton.onClick.AddListener(() =>
            {
                Time.timeScale = neutralTimeFlow;
                SceneManagerScript.ChangeToScene(SceneName.Lobby);
            });

            leaveGameButton.onClick.AddListener(Application.Quit);

            _pauseTrigger = playerInputs.actions["Escape"];
            _pauseTrigger.started += _ =>
            {
                if (_isMultiplayer) RequestPauseServerRpc(!_isPauseActive);
                else ChangePause(!_isPauseActive);
            };
        }

        private void OnEnable()
        {
            _syncedTimeScale.OnValueChanged += OnTimeScaleChanged;
        }

        private void OnDisable()
        {
            _syncedTimeScale.OnValueChanged -= OnTimeScaleChanged;
        }
    }
}
