using initScene;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoD
{
    public class GodPlaceMonsterScript : CameraUser
    {
        public PlayerInput godInputs;
        public Transform cameraHolder;
        public Transform targetTransform;

        private Vector2 _directions;
        private bool _isRotating;

        private InputAction _rotationTrigger;

        private void Start()
        {
            _rotationTrigger = godInputs.actions["Look"];
            _rotationTrigger.performed += ctx => _directions = ctx.ReadValue<Vector2>() * 10;
            _rotationTrigger.canceled += _ => _directions = Vector2.zero;
        }

        private void FixedUpdate()
        {
            cameraHolder.rotation = Quaternion.Euler(-_directions.y, 0f, _directions.x);

            Vector3 cameraPos = Input.mousePosition;
            Vector3 targetPosition = targetTransform.position;

            if (!Camera)
            {
                Debug.LogWarning("Camera is null");
                return;
            }

            Vector3 pos = Camera.ScreenToWorldPoint(cameraPos);

            Vector3 satellitePosition = Camera.transform.position;

            cameraPos.z = satellitePosition.y - targetPosition.y;
            pos.y = targetPosition.y;
            targetPosition = pos;
            targetTransform.position = targetPosition;
        }
    }
}