using Globals;
using Unity.Netcode;
using UnityEngine;

namespace EndGame
{
    public class GameEnd : NetworkBehaviour
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
            if (SceneObjectReferencer.MainInstance.isNetworkScene)
            {
                EndGameRpc(mageWin);
            }
            
            if (mageWin)
            {
                // god screen
                GameWon();
                return;
            }

            GameLost();
        }

        [Rpc(SendTo.Everyone)]
        private void EndGameRpc(bool mageWin)
        {
            if (IsServer)
            {
                if (mageWin)
                {
                    // god screen
                    GameWon();
                    return;
                }

                GameLost();
                return;
            }
            
            if (mageWin)
            {
                // god screen
                GameLost();
                return;
            }

            GameWon();
        }
    }
}