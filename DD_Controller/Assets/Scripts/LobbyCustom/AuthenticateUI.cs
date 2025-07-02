using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace LobbyCustom
{
    public class AuthenticateUI : MonoBehaviour {
        
        [SerializeField] private Button authenticateButton;
        [SerializeField] private LobbyListUI lobbyListUI;

        private void Start()
        {
            bool isSignedIn = false;
            try
            {
                isSignedIn = AuthenticationService.Instance.IsSignedIn;
            }
            catch (ServicesInitializationException)
            {
            }

            if (isSignedIn)
            {
                lobbyListUI.Show();
                Hide();
            }
            else
            {
                authenticateButton.onClick.AddListener(() => {
                    PlayerManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
                    lobbyListUI.Show();
                    LobbyManager.Instance.RefreshLobbyList();
                    Hide();
                });
            }
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

    }
}