using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Move
{
    public class MageCameraController : MonoBehaviour
    {
        public Transform directionMain;
        public PlayerInput playerInputs;
        public Transform cameraTarget;
        public Vector3 cameraOffset;
        public MoveMode moveMode = MoveMode.ThirdPerson;

        private Vector2 _lookVector;
        private bool _mouse;

        private void Start()
        {
            playerInputs.actions["look_GP"].performed += ctx => RmbRotation(ctx.ReadValue<Vector2>());
            playerInputs.actions["look_GP"].canceled += _ => RmbRotation(Vector2.zero);
            playerInputs.actions["look_mouse"].performed += ctx => RotateCameraFromMouse(ctx.ReadValue<Vector2>());
            playerInputs.actions["look_mouse"].canceled += _ => RotateCameraFromMouse(Vector2.zero);
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) _mouse = true;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                RmbRotation(Vector2.zero);
                _mouse = false;
            }

            switch (moveMode)
            {
                case MoveMode.ThirdPerson:
                    ThirdPerson();
                    break;
                case MoveMode.UpView:
                    UpView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void SetRotationY(float newRot, Transform objectTransform)
        {
            Quaternion cameraRotation = objectTransform.rotation;
            Vector3 cameraEulerRotation = cameraRotation.eulerAngles;
            objectTransform.rotation = Quaternion.Euler(cameraEulerRotation.x, newRot, cameraEulerRotation.z);
        }

        public static void SetRotationX(float newRot, Transform objectTransform)
        {
            Quaternion cameraRotation = objectTransform.rotation;
            Vector3 cameraEulerRotation = cameraRotation.eulerAngles;
            objectTransform.rotation = Quaternion.Euler(newRot, cameraEulerRotation.y, cameraEulerRotation.z);
        }

        private void RmbRotation(Vector2 direction)
        {
            _lookVector = direction;
        }

        private void RotateCameraFromMouse(Vector2 direction)
        {
            if (_mouse && moveMode.Equals(MoveMode.ThirdPerson)) _lookVector = direction / 4;
        }


        private void ThirdPerson()
        {
            Quaternion cameraRotation = directionMain.rotation;
            Vector3 cameraEulerRotation = cameraRotation.eulerAngles;

            cameraEulerRotation.y += _lookVector.x * 90 * Time.deltaTime;
            cameraEulerRotation.x -= _lookVector.y * 90 * Time.deltaTime;
            cameraRotation = Quaternion.Euler(cameraEulerRotation);

            directionMain.rotation = cameraRotation;
            directionMain.position = cameraTarget.position + cameraRotation * cameraOffset;
        }

        private void UpView()
        {
            if (!(_lookVector.sqrMagnitude > 0.01f)) return;
            float targetAngle = Mathf.Atan2(_lookVector.x, _lookVector.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            directionMain.rotation = targetRotation;
        }
    }

    public interface IRotationApply
    {
        public void Apply(Transform mainDirection, Vector2 inputVector);
    }
}