using UnityEngine;
using UnityEngine.InputSystem;

public class AnalogMoves : MonoBehaviour
{
    public Transform target;
    public PlayerInput player;
    
    private Vector3 _targetStart;
    private Vector3 _movementVector;
    private Vector2 _moveVector;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _targetStart = target.position;
        player.actions["move"].performed += ctx => _moveVector = ctx.ReadValue<Vector2>();
        player.actions["move"].canceled += ctx => _moveVector = ctx.ReadValue<Vector2>();
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        _movementVector = new Vector3(
            _moveVector.x,
            _moveVector.y,
            0
        );
        
        target.position = _targetStart + _movementVector;
    }
}
