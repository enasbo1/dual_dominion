using JetBrains.Annotations;
using UnityEngine;

namespace end_game
{
    class GameEnd : MonoBehaviour
    {
        [SerializeField][CanBeNull] private GameObject gameOverCanvas;
        [SerializeField][CanBeNull] private GameObject gameWonCanvas;
        public GameObject[] objectToDelete;
        private void Start()
        {
            gameOverCanvas?.SetActive(false);
        }
        public void GameOver()
        {
            gameOverCanvas?.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            foreach (GameObject go in objectToDelete) Destroy(go);
            Cursor.visible = true;
        }
    }
}