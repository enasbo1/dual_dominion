using UnityEngine;

namespace Master
{
    public class MasterPlacementScript : MonoBehaviour
    {
        public Camera satelliteCamera;
        public Transform playerTransformA;
        public Transform playerTransformB;
        public Transform targetTransform;
        
        private float _positionZ;
        private float _positionX;

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKey(KeyCode.W)) {
                _positionZ += 1f;
            } else if (Input.GetKey(KeyCode.S)) {
                _positionZ -= 1f;
            } else {
                if (_positionZ > 0f) {
                    _positionZ -= 1f;
                }
                if (_positionZ < 0f) {
                    _positionZ += 1f;
                }
            }
            
            if (Input.GetKey(KeyCode.A)) {
                _positionX -= 1f;
            } else if (Input.GetKey(KeyCode.D)) {
                _positionX += 1f;
            } else {
                if (_positionX > 0f) {
                    _positionX -= 1f;
                }
                if (_positionX < 0f) {
                    _positionX += 1f;
                }
            }
        }

        private void FixedUpdate()
        {
            _positionZ = Mathf.Clamp(_positionZ, -10f, 10f);
            _positionX = Mathf.Clamp(_positionX, -10, 10f);
            
            Vector3 satellitePosition = satelliteCamera.transform.position;
            
            playerTransformA.rotation = Quaternion.Euler(_positionZ + 90f, 0f, 0f);
            playerTransformB.rotation = Quaternion.Euler(0f, 0f, -_positionX);
            playerTransformA.position = satellitePosition + new Vector3(_positionX, 0f, _positionZ);   
            
            Vector3 cameraPos = Input.mousePosition;
            Vector3 targetPosition = targetTransform.position;
            
            cameraPos.z = satellitePosition.y - targetPosition.y;
            Vector3 pos = satelliteCamera.ScreenToWorldPoint(cameraPos);
            pos.y = targetPosition.y;
            targetPosition = pos;
            targetTransform.position = targetPosition;
        }
    }
}
