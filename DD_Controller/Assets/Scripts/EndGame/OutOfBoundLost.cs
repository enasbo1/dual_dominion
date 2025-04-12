using JetBrains.Annotations;
using UnityEngine;

namespace end_game
{
    public class OutOfBoundLost : MonoBehaviour
    {
        [CanBeNull] public GameObject gameOverCanvas;
        public GameObject[] objectToDelete;
        public Transform[] objectLimited;
        public int deathBottom = -100;

        private void Start()
        {
            gameOverCanvas?.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
            foreach (Transform got in objectLimited)
                if (got.transform.position.y < deathBottom)
                    GameOver();
        }

        private void GameOver()
        {
            gameOverCanvas?.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            foreach (GameObject go in objectToDelete) Destroy(go);
            Cursor.visible = true;
        }
    }
}