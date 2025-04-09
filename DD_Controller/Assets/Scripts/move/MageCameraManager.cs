using UnityEngine;
using UnityEngine.InputSystem;

namespace Move
{
    public class MageCameraManager : MonoBehaviour
    {
        public Transform directionMain;
        public Transform playerCamera;
        public PlayerInput playerInputs;
        public Transform cameraTarget;
        
        public bool isRotable = true;
        
        private Vector2 _lookVector;
        private bool _mouse;

        
        public void SetRotable(bool newState)
        {
            isRotable = newState;
        }
        
        public void SetRotationY(float newRot, Transform objectTransform)
        {
            Quaternion cameraRotation = objectTransform.rotation;
            Vector3 cameraEulerRotation = cameraRotation.eulerAngles;
            objectTransform.rotation = Quaternion.Euler(cameraEulerRotation.x, newRot, cameraEulerRotation.z);
        }
        
        public void SetRotationX(float newRot, Transform objectTransform)
        {
            Quaternion cameraRotation = objectTransform.rotation;
            Vector3 cameraEulerRotation = cameraRotation.eulerAngles;
            objectTransform.rotation = Quaternion.Euler(newRot, cameraEulerRotation.y, cameraEulerRotation.z);
        }
        
        void Start()
        {
            playerInputs.actions["look_GP"].performed += ctx => RotateCamera(ctx.ReadValue<Vector2>());
            playerInputs.actions["look_GP"].canceled += _ => RotateCamera(Vector2.zero);
            playerInputs.actions["look_mouse"].performed += ctx => RotateCameraFromMouse(ctx.ReadValue<Vector2>());
            playerInputs.actions["look_mouse"].canceled += _ => RotateCameraFromMouse(Vector2.zero);
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void RotateCamera(Vector2 direction)
        {
            if (isRotable)
            {
                _lookVector = direction;
            }
        }    
        
        private void RotateCameraFromMouse(Vector2 direction)
        {
            if (_mouse && isRotable)
            {
                _lookVector = direction/4;
            }
        }
        
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouse = true;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                RotateCamera(Vector2.zero);
                _mouse = false;
            }

            Quaternion cameraRotation = directionMain.rotation;
            Vector3 cameraEulerRotation = cameraRotation.eulerAngles;

            if (isRotable)
            {
                cameraEulerRotation.y += _lookVector.x * 90 * Time.deltaTime;
                cameraEulerRotation.x -= _lookVector.y * 90 * Time.deltaTime;
                cameraRotation = Quaternion.Euler(cameraEulerRotation);
            
                directionMain.rotation = cameraRotation;
            }
            directionMain.position = cameraTarget.position;
        }
    }
}