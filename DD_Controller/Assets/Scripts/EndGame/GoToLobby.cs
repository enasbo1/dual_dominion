using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace end_game
{
    public class GoToLobby : MonoBehaviour
    {
        private static readonly string LOBBY_SCENE = "LobbyTutorial_Done";

        public Button backToLobbyButton;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            backToLobbyButton.onClick.AddListener(_GoToLobby);
        }

        private static void _GoToLobby()
        {
            Debug.Log(LOBBY_SCENE);
            SceneManager.LoadScene(LOBBY_SCENE);
        }
    }
}