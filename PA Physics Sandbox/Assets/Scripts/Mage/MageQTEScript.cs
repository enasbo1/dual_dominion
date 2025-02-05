using UnityEngine;

namespace Mage
{
    public class MageQTEScript : MonoBehaviour
    {
        // public Transform playerTransform;
        public float timeLimit = 10f;

        private float _inputTime;
        private int _inputStep;
        private const int LastStep = 4;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Debug.Log("S Z Z Z"); // Adapt to keyboard (querty/azerty) or controller
            }
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                return;
            }
            
            if (_inputStep is 0 && Input.GetKeyDown(KeyCode.S))
            {
                _inputStep += 1;
                _inputTime -= 1f;
            }
            if (_inputStep is 1 or 2 or 3 && Input.GetKeyDown(KeyCode.W))
            {
                _inputStep += 1;
                _inputTime -= 1f;
            }
            
            if (_inputTime >= timeLimit)
            {
                _inputStep = 0;
                _inputTime = 0f;
                Debug.Log("failure");
            }
            if (_inputStep >= LastStep) {
                _inputStep = 0;
                _inputTime = 0f;
                Debug.Log("success");
            }
        }

        private void FixedUpdate()
        {
            if (_inputStep > 0) {
                _inputTime += Time.deltaTime;
            }
        }
    }
}
