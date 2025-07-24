using UnityEngine;

namespace PlayerSpace.GoD
{
    public class GodManagerScript : MonoBehaviour
    {
        public double karmaPoint;

        private double _lastServerTime;
        
        private void FixedUpdate()
        {
            karmaPoint += Time.deltaTime * 3;
        }
    }
}