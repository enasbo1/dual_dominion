using GoD.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoD
{
    public class GodMovementScript : MonoBehaviour
    {
        public GodPointerManagerScript godPointerManager;
        public PlayerInput godInputs;
        public Transform playerTransform;
        public float globalSpeed = 40f;
        public float masterVerticalSensibility = 1f;
        public float masterHorizontalSensibility = 2f;
        private bool _mouseMove;

        private InputAction _movementGamepadTrigger;
        private Vector3 _movementMouseVector;

        private Vector3 _movementVector;
        private float _positionX;

        private float _positionZ;
        private InputAction _rotationTrigger;

        private void Awake()
        {
            _movementGamepadTrigger = godInputs.actions["MoveGamepad"];
            _movementGamepadTrigger.performed += ctx => SetCameraDirection(ctx.ReadValue<Vector2>());
            _movementGamepadTrigger.canceled += _ => SetCameraDirection(Vector3.zero);

            // Mouse position is relative to app screen size
            Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
        }

        private void Update()
        {
            MouseControl();
        }

        private void FixedUpdate()
        {
            playerTransform.position += _movementVector * (globalSpeed * Time.deltaTime);

            if (_positionZ != 0f || _positionX != 0f)
            {
                _movementMouseVector = new Vector3(
                    -_positionX,
                    0f,
                    _positionZ
                );

                playerTransform.position += _movementMouseVector * (globalSpeed * Time.deltaTime);

                _positionZ = 0;
                _positionX = 0;
            }
        }

        private void SetCameraDirection(Vector2 directions)
        {
            _movementVector = new Vector3(
                directions.x,
                0f,
                directions.y
            ).normalized;
        }

        private void MouseControl()
        {
            if (Input.GetMouseButton(0) && !godPointerManager.isPointerOverScrollRect)
            {
                _positionZ += -Input.GetAxis("Mouse Y") * masterVerticalSensibility;
                _positionX += Input.GetAxis("Mouse X") * masterHorizontalSensibility;
            }
        }
    }
}