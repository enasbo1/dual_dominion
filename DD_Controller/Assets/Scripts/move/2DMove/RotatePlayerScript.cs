using UnityEngine;
using UnityEngine.InputSystem;

namespace Move._2DMove
{
    public class RotatePlayerScripts : MonoBehaviour
    {
        public PlayerInput playerInputs;
        public Transform playerCharacter;
        
        private InputAction _playerRotationTrigger;
        
        private Vector2 _lookVector;
    
        private void Start()
        {
            _playerRotationTrigger = playerInputs.actions["Look_GP"];
            _playerRotationTrigger.performed += ctx => Rotate(ctx.ReadValue<Vector2>());
        }
        
        private void Rotate(Vector2 direction)
        {
            _lookVector = direction;
        }
        
        private void Update()
        {
            if (_lookVector.sqrMagnitude > 0.01f)
            {
                float targetAngle = Mathf.Atan2(_lookVector.x, _lookVector.y) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
                playerCharacter.rotation = targetRotation;
            }
        }
    }
}
