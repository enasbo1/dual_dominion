using UnityEngine;
using UnityEngine.InputSystem;

namespace GoD
{
    public class GodPlaceMonsterScript : MonoBehaviour
    {
        public PlayerInput godInputs;
        public Camera satelliteCamera;
        public Transform cameraHolder;
        public Transform targetTransform;

        private Vector2 _directions;
        
        private InputAction _rotationTrigger;
        private bool _isRotating;

        private void Start()
        {
            _rotationTrigger = godInputs.actions["Look"];
            _rotationTrigger.performed += ctx => _directions = ctx.ReadValue<Vector2>() * 10;
            _rotationTrigger.canceled += _ => _directions = Vector2.zero;
        }

        private void FixedUpdate()
        {
            Vector3 satellitePosition = satelliteCamera.transform.position;
            
            cameraHolder.rotation = Quaternion.Euler(-_directions.y, 0f, _directions.x);
            
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
