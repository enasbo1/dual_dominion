using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace script.end_game
{
    public class GoToLobby : MonoBehaviour
    {
    
        public Button backToLobbyButton;
        private static string LOBBY_SCENE =  "LobbyTutorial_Done";
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
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
