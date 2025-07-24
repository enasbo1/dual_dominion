using Globals;
using UnityEngine;

namespace EndGame
{
    public class OutOfBoundLost : MonoBehaviour
    {
        [SerializeField] private GameEnd gameEnd;
        public Transform[] objectLimited;
        public int deathBottom = -100;

        private void Start()
        {
            if (!gameEnd)
                SceneObjectReferencer.WaitingInit += sor =>
                {
                    gameEnd = sor.gameEnd;
                };
        }

        // Update is called once per frame
        private void Update()
        {
            foreach (Transform got in objectLimited)
                if (got.transform.position.y < deathBottom)
                    gameEnd.EndGame(false);
        }
    }


}