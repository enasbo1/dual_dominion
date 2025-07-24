using System;
using System.Collections.Generic;
using Globals;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Menu
{
    public class PauseMenuScript : NetworkBehaviour
    {
        private const float TOLERANCE = 0.001f;
        
        public static PauseMenuScript Instance { get; private set; }
        
        public InputActionReference pauseTrigger;
        public List<GameObject> objectsToDisable = new List<GameObject>();

        [SerializeField] private GameObject canvas;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button leavePartyButton;
        [SerializeField] private Button leaveGameButton;
        
        [NonSerialized] public bool isPauseActive;
        private TimeScaleController _timeScaleController;
        private bool _isMultiplayer;

        private void ApplyPause(bool shouldPause)
        {
            if (_isMultiplayer)
            {
                ApplyPauseRpc(shouldPause);
            }
            else
            {
                _applyPause(shouldPause);
            }
        }
        [Rpc(SendTo.Everyone)]
        private void ApplyPauseRpc(bool shouldPause)
        {
            _applyPause(shouldPause);
        }

        private void _applyPause(bool shouldPause)
        { 
            isPauseActive = shouldPause;
            canvas.SetActive(shouldPause);
            objectsToDisable.ForEach(x => x.SetActive(!shouldPause));
            
            Cursor.lockState = shouldPause || (_isMultiplayer && !IsServer) ? CursorLockMode.None : CursorLockMode.Locked;
        }
                
        private void ChangePause(bool shouldPause)
        {
            _timeScaleController.SetTimeScale(shouldPause ? _timeScaleController.timeFlowPause : _timeScaleController.previousTimeFlow);

            if (!_isMultiplayer) ApplyPause(shouldPause);
        }

        private void OnSyncedTimeChange(float _, float newFlow)
        {
            if ((!isPauseActive && Math.Abs(newFlow - _timeScaleController.timeFlowPause) < TOLERANCE) || 
                (isPauseActive && Math.Abs(newFlow - _timeScaleController.timeFlowNeutral) < TOLERANCE)
               ) ApplyPause(!isPauseActive);
        }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        
        public void Start()
        {
            _timeScaleController = TimeScaleController.Instance;
            if (_timeScaleController == null)
            {
                Debug.LogError("TimeScaleController instance not found in scene.");
                enabled = false;
                return;
            }

            SceneObjectReferencer.WaitingInit += sor =>
            {
                _isMultiplayer = sor.isNetworkScene;
            };

            Time.timeScale = _timeScaleController.timeFlowNeutral;
            
            resumeButton.onClick.AddListener(() => { ChangePause(false); });

            leavePartyButton.onClick.AddListener(() =>
            {
                Time.timeScale = _timeScaleController.timeFlowNeutral;
                SceneManagerScript.ChangeToScene(SceneName.Lobby);
            });

            leaveGameButton.onClick.AddListener(Application.Quit);
            
            pauseTrigger.action.Enable();
            pauseTrigger.action.started += _ => { ChangePause(!isPauseActive); };

            _timeScaleController.syncedTimeScale.OnValueChanged += OnSyncedTimeChange;
        }
        
        private new void OnDestroy()
        {
            if (_timeScaleController != null)
            {
                _timeScaleController.syncedTimeScale.OnValueChanged -= OnSyncedTimeChange;
            }
        }
    }
}
