using UnityEngine;

namespace Move
{
    public class FollowPositionManager : MonoBehaviour
    {
        public Transform[] leaders;
        public Transform[] followers;

        private void Start()
        {
            if (leaders.Length != followers.Length) Debug.LogError("leaders and followers must have the same length");
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < leaders.Length; i++) followers[i].position = leaders[i].position;
        }
    }
}