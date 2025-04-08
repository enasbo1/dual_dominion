using UnityEngine;
using UnityEngine.InputSystem;

namespace Move
{
    public class MageCameraManager : MonoBehaviour
    {
        public Transform playerCamera;
        public PlayerInput player;
        public Transform cameraTarget;
        
        public bool isRotable = true;
        
        private Vector2 _lookVector;
        private bool _mouse;
        private Vector3 skyViewHeight = Vector3.zero;

        
        public void SetRotable(bool newState)
        {
            isRotable = newState;
        }
        public void SetNewHeight(float newHeight)
        {
            skyViewHeight.y = newHeight;
        }
        
        public Quaternion GetCameraRotation()
        {
            return this.playerCamera.rotation;
        }
        public void SetRotationY(float newRot)
        {
            Quaternion cameraRotation = playerCamera.rotation;
            Vector3 cameraEulerRotation = cameraRotation.eulerAngles;
            playerCamera.rotation = Quaternion.Euler(cameraEulerRotation.x, newRot, cameraEulerRotation.z);
        }
        
        public void SetRotationX(float newRot)
        {
            Quaternion cameraRotation = playerCamera.rotation;
            Vector3 cameraEulerRotation = cameraRotation.eulerAngles;
            playerCamera.rotation = Quaternion.Euler(newRot, cameraEulerRotation.y, cameraEulerRotation.z);
        }
        
        void Start()
        {
            player.actions["look_GP"].performed += ctx => RotateCamera(ctx.ReadValue<Vector2>());
            player.actions["look_GP"].canceled += _ => RotateCamera(Vector2.zero);
            player.actions["look_mouse"].performed += ctx => RotateCameraFromMouse(ctx.ReadValue<Vector2>());
            player.actions["look_mouse"].canceled += _ => RotateCameraFromMouse(Vector2.zero);
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

            Quaternion cameraRotation = playerCamera.rotation;
            Vector3 cameraEulerRotation = cameraRotation.eulerAngles;

            if (isRotable)
            {
                cameraEulerRotation.y += _lookVector.x * 90 * Time.deltaTime;
                cameraEulerRotation.x -= _lookVector.y * 90 * Time.deltaTime;
                cameraRotation = Quaternion.Euler(cameraEulerRotation);
            
                playerCamera.rotation = cameraRotation;
            }
            playerCamera.position = cameraTarget.position + skyViewHeight;
        }
    }
}