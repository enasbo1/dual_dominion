using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LobbyCustom
{
    public class EditPlayerName : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI playerNameText;

        private string _playerName = "Username";
        
        public static EditPlayerName Instance { get; private set; }
        public event EventHandler OnNameChanged;

        private void Awake() {
            Instance = this;

            GetComponent<Button>().onClick.AddListener(() => {
                UI_InputWindow.Show_Static("Player Name", _playerName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-1234567890", 20,
                    () => {
                        // Cancel
                    },
                    (string newName) => {
                        _playerName = newName;

                        playerNameText.text = _playerName;

                        OnNameChanged?.Invoke(this, EventArgs.Empty);
                    });
            });

            playerNameText.text = _playerName;
        }

        private void Start() {
            OnNameChanged += EditPlayerName_OnNameChanged;
        }

        private void EditPlayerName_OnNameChanged(object sender, EventArgs e) {
            PlayerManager.Instance.UpdatePlayerName(GetPlayerName());
        }

        public string GetPlayerName() {
            return _playerName;
        }


    }
}