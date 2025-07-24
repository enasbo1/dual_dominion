using Globals;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSpace.GoD
{
    public class GodPlaceMonsterScript : MonoBehaviour
    {
        public PlayerInput godInputs;
        public Transform cameraHolder;
        public Transform targetTransform;

        private Vector2 _directions;
        private bool _isRotating;

        private InputAction _rotationTrigger;
        private float _warnTimer = 0f;
        private bool _touched = true;
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

            cameraPos.z = 1f;
            
            if (!SceneObjectReferencer.MainInstance.camera)
            {
                if (Time.time < _warnTimer) return;
                Debug.LogWarning("Camera is null");
                _warnTimer = Time.time + 2f;
                return;
            }

            Vector3 pos = SceneObjectReferencer.MainInstance.camera.ScreenToWorldPoint(cameraPos);
            
            Vector3 satellitePosition = SceneObjectReferencer.MainInstance.camera.transform.position;
            
            Vector3 direction = pos - satellitePosition;

            if (Physics.Raycast(pos, direction, out RaycastHit hitInfo,
                    SceneObjectReferencer.MainInstance.camera.farClipPlane,
                    SceneObjectReferencer.MainInstance.MapLayer))
            {
                if (!_touched)
                {
                    _touched = true;
                    targetTransform.gameObject.SetActive(true);
                }
               
                targetTransform.position = hitInfo.point + Vector3.up;
                targetTransform.rotation *= Quaternion.Euler(0,Time.deltaTime*10f,0);
            }
            else if (_touched)
            {
                _touched = false;
                targetTransform.gameObject.SetActive(false);
            }
        }
    }
}