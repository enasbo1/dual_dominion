using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class QuakeLikeScript : MonoBehaviour
{
    public Transform playerTransform;
    public Transform headTransform;
    public float speed;
    public float sensibilityH;
    public float sensibilityV;
    [FormerlySerializedAs("playerRigibody")] public Rigidbody playerRigidBody;
    public PlayerInput player;
    
    private Vector2 _lookVector;
    private bool _mouse;
    private bool _wantToJump;
    private Vector3 _directionIntent;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player.actions["move"].performed += ctx => AddMove(ctx.ReadValue<Vector2>());
        player.actions["move"].canceled += ctx => AddMove(ctx.ReadValue<Vector2>());
        player.actions["look_GP"].performed += ctx => RotateCamera(ctx.ReadValue<Vector2>());
        player.actions["look_GP"].canceled += _ => RotateCamera(Vector2.zero);
        player.actions["look_mouse"].performed += ctx => RotateCameraFromMouse(ctx.ReadValue<Vector2>());
        player.actions["look_mouse"].canceled += _ => RotateCameraFromMouse(Vector2.zero);
        player.actions["jump"].performed += _ => JumpIntent();
    }

    private void AddMove(Vector2 movement)
    {
        _directionIntent = (Vector3.forward * movement.y) + (Vector3.right * movement.x);
    }
    private void RotateCamera(Vector2 direction)
    {
        _lookVector = direction;
    }    
    
    private void RotateCameraFromMouse(Vector2 direction)
    {
        if (_mouse)
        {
            _lookVector = direction/4;
        }
    }

    private void JumpIntent()
    {
        if ((Physics.SphereCast(playerTransform.position + (Vector3.up * 0.55f),
                0.45f, 
                Vector3.down, 
                out var _, 
                1.0f)))
        {
            _wantToJump = true;
        }
    }
    // Update is called once per frame
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

        var rot = playerTransform.rotation.eulerAngles;
        rot.y += _lookVector.x * 90 * Time.deltaTime*sensibilityH;
                
        playerTransform.rotation = Quaternion.Euler(rot);

        var roth = headTransform.localRotation.eulerAngles;
        roth.x -= _lookVector.y * 90 * Time.deltaTime*sensibilityV;
        headTransform.localRotation = Quaternion.Euler(roth);

    }

    private void FixedUpdate()
    {
        playerRigidBody.linearVelocity = (playerTransform.rotation * _directionIntent.normalized) * (speed) + Vector3.up * playerRigidBody.linearVelocity.y;
        if (_wantToJump)
        {
            playerRigidBody.AddForce(Vector3.up*40f, ForceMode.Impulse);
            _wantToJump = false;
        }
    }
}
