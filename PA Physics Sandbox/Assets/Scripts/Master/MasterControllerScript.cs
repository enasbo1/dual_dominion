using UnityEngine;
using UnityEngine.InputSystem;

namespace Master
{
    public enum MovementType
    {
        Mouse,
        KeyBoard,
        Both,
    }
    
    public class MasterControllerScript : MonoBehaviour
    {
        public MovementType movementType;
        public Transform playerTransform;
        public float speed = 5f;
        public float masterVerticalSensibility = 3f;
        public float masterHorizontalSensibility = 5f;

        private float _positionZ;
        private float _positionX;
    
        private Vector3 _movementVector;
        private Vector3 _movementMouseVector;
        private float _movementZ;
        private float _movementX;
        private bool _mouseMove;

    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            // Mouse position is relative to app screen size
            Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
        }

        private void Update()
        {
            switch (movementType)
            {
                case MovementType.Mouse :
                    MouseControl();
                    break;
                case MovementType.KeyBoard :
                    KeyControl();
                    break;
                case MovementType.Both :
                default:
                    MouseControl();
                    KeyControl();
                    break;
            }
        }
    
        private void FixedUpdate()
        {
            _movementVector = new Vector3(
                _movementX,
                0f,
                _movementZ
            ).normalized;
        
            playerTransform.position += _movementVector * (speed * Time.deltaTime);

            if (_mouseMove) {
                _movementMouseVector = new Vector3(
                    -_positionX,
                    0f,
                    _positionZ
                );
            
                playerTransform.position += _movementMouseVector * (speed * Time.deltaTime);
                
                _positionZ = 0;
                _positionX = 0;
            }
        
            _movementZ = 0.0f;
            _movementX = 0.0f;
        }
    
        private void MouseControl()
        {
            if (Input.GetMouseButton(0))
            {
                _positionZ += -Input.GetAxis("Mouse Y") * masterVerticalSensibility;
                _positionX += Input.GetAxis("Mouse X") * masterHorizontalSensibility;
            
                _mouseMove = true;
            } else {
                _mouseMove = false;
            }

            // if (Input.GetKey(KeyCode.Space)) {
            //     Physics.SphereCast(playerTransform.position + Vector3.up * 0.5f, 1f, Vector3.down, out _, 0.6f);
            //     // Debug.Log(hitInfo);
            // }
        }
        
        private void KeyControl()
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
        }
    }
}
