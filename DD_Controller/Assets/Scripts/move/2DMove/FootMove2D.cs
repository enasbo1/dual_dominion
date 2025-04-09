using UnityEngine;
using UnityEngine.InputSystem;

namespace Move._2DMove
{
    public class FootMove2D : MonoBehaviour
    {
        public PlayerInput playerInputs;
        public Transform characterTransform;
        public float movementSpeed = 4.0f;
        
        private InputAction _playerMovementDirectionTrigger;
        
        private Vector2 _directionVector;
        
        private void Rotate(Vector2 direction)
        {
            _directionVector = direction;
        }
        
        private void Start()
        {
            _playerMovementDirectionTrigger = playerInputs.actions["Move"];
            _playerMovementDirectionTrigger.performed += ctx => Rotate(ctx.ReadValue<Vector2>());
            _playerMovementDirectionTrigger.canceled += _ => Rotate(Vector2.zero);
        }
        
        private void Move()
        {
            Vector3 move = new Vector3(_directionVector.x, 0, _directionVector.y).normalized;
            characterTransform.position += move * (movementSpeed * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Move();
        }
    }
}
