using System;
using JetBrains.Annotations;
using UnityEngine;

namespace script.end_game
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

        private void GameOver()
        {
            gameOverCanvas?.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            foreach (var go in objectToDelete)
            {
                Destroy(go);
            }
            Cursor.visible = true;
        }
        
        // Update is called once per frame
        private void Update()
        {
            foreach (var got in objectLimited)
            {
                if (got.transform.position.y < deathBottom)
                {
                    GameOver();
                }
            }
        }
    }
}
