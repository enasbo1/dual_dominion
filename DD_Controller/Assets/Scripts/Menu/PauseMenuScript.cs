using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Menu
{
    public class PauseMenuScript : WithEndMonoBehavior
    {
        public PlayerInput playerInputs;

        [Range(0.01f, 3f)] public float neutralTimeFlow = 1f;

        [Range(0.01f, 3f)] public float pauseTimeFlow = 0.1f;

        public List<GameObject> objectsToDisable = new();

        [SerializeField] private GameObject canvas;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button leavePartyButton;
        [SerializeField] private Button leaveGameButton;
        private bool _isPauseActive;

        private InputAction _pauseTrigger;

        private void Awake()
        {
            resumeButton.onClick.AddListener(CloseMenu);

            leavePartyButton.onClick.AddListener(() =>
            {
                Time.timeScale = neutralTimeFlow;
                SceneManagerScript.ChangeToScene(SceneName.Lobby);
            });

            leaveGameButton.onClick.AddListener(Application.Quit);

            _pauseTrigger = playerInputs.actions["Escape"];
            _pauseTrigger.started += ToBeCleanedAction(_ =>
                {
                    if (_isPauseActive) CloseMenu();
                    else OpenMenu();
                },
                a => _pauseTrigger.started -= a
            );
        }

        private void OpenMenu()
        {
            Time.timeScale = pauseTimeFlow;
            canvas.SetActive(true);
            objectsToDisable.ForEach(x => x.SetActive(false));
            Cursor.lockState = CursorLockMode.None;
            _isPauseActive = true;
        }

        private void CloseMenu()
        {
            Time.timeScale = neutralTimeFlow;
            canvas.SetActive(false);
            objectsToDisable.ForEach(x => x.SetActive(true));
            Cursor.lockState = CursorLockMode.Locked;
            _isPauseActive = false;
        }
        
        
        private void OnDestroy()
        {
            Time.timeScale = neutralTimeFlow;
        }
    }
}