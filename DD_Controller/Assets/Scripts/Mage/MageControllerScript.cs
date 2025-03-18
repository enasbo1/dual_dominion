using UnityEngine;

namespace Mage
{
    public class MageControllerScript : MonoBehaviour
    {
        public Rigidbody playerRigidBody;
        public Transform headTransform;
        public float speed = 10f;
        public float jumpForce = 325f;
        public float mageVerticalSensibility = 2f;
        public float mageHorizontalSensibility = 3f;

        private float _rotationX;
        private float _rotationY;
    
        private Vector3 _movementVector;
        private float _movementZ;
        private float _movementX;
        private bool _jump;

        // Update is called once per frame
        private void Update()
        {
            ControlTransform();
        
            _rotationX += -Input.GetAxis("Mouse Y") * mageVerticalSensibility;
            _rotationY += Input.GetAxis("Mouse X") * mageHorizontalSensibility;
            _rotationX = Mathf.Clamp(_rotationX, -45f, 45f);
            headTransform.rotation = Quaternion.Euler(_rotationX, _rotationY, 0f);
        }
    
        private void FixedUpdate()
        {
            _movementVector = new Vector3(
                _movementX,
                0f,
                _movementZ
            ).normalized;
        
            if (_jump) {
                playerRigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                _jump = false;
            }
        
            playerRigidBody.transform.position += Quaternion.Euler(0f, _rotationY, 0f) * _movementVector * (speed * Time.deltaTime);
        
        
            _movementZ = 0.0f;
            _movementX = 0.0f;
        }
    
        private void ControlTransform()
        {
            if (Input.GetKey(KeyCode.W)) {
                _movementZ = 1f;
            }
            if (Input.GetKey(KeyCode.S)) {
                _movementZ = -1f;
            }
            if (Input.GetKey(KeyCode.A)) {
                _movementX = -1f;
            }
            if (Input.GetKey(KeyCode.D)) {
                _movementX = 1f;
            }
        
            if (Input.GetKeyDown(KeyCode.Space) && Physics.SphereCast(playerRigidBody.transform.position + Vector3.up * 0.5f, 1f, Vector3.down,out _, 0.6f))
            {
                _jump = true;
            }
        }
    }
}
