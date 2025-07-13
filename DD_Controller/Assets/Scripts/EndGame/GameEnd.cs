using Unity.Netcode;
using UnityEngine;

namespace end_game
{
    class GameEnd : NetworkBehaviour
    {
        [SerializeField] private GameObject gameOverCanvas;
        [SerializeField] private GameObject gameWonCanvas;
        
        public GameObject[] objectToDelete;
        public MonoBehaviour[] scriptToDisable;
            
        private void Start()
        {
            gameOverCanvas.SetActive(false);
            gameWonCanvas.SetActive(false);
        }
        
        private void GameLost()
        {
            gameOverCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            foreach (GameObject go in objectToDelete) Destroy(go);
            foreach (MonoBehaviour script in scriptToDisable) script.enabled = false;
            Cursor.visible = true;
        }
        
        private void GameWon()
        {
            gameWonCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            foreach (GameObject go in objectToDelete) Destroy(go);
            foreach (MonoBehaviour script in scriptToDisable) script.enabled = false;
            Cursor.visible = true;
        }
        
        public void EndGame(bool mageWin)
        {
            if (mageWin)
            {
                if (IsServer)
                {
                    // god screen
                    GameLost();
                    return;
                }

                GameWon();
                return;
            }
            
            if (IsServer)
            {
                // god screen
                GameWon();
                return;
            }

            GameLost();
        }
    }
}