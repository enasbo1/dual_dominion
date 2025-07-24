using EndGame;
using UnityEngine;

namespace end_game
{
    public class OutOfBoundLost : MonoBehaviour
    {
        [SerializeField] private GameEnd gameEnd;
        public Transform[] objectLimited;
        public int deathBottom = -100;



        // Update is called once per frame
        private void Update()
        {
            foreach (Transform got in objectLimited)
                if (got.transform.position.y < deathBottom)
                    gameEnd.EndGame(false);
        }
    }


}