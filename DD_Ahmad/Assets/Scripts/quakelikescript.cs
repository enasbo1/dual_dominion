using UnityEngine;
using UnityEngine.InputSystem;

public class Quakelikescript : MonoBehaviour
{
    public Transform playerTransform;
    public Transform headTransform;
    public float speed;
    public float sensibilityH;
    public float sensibilityV;
    public Rigidbody playerRigibody;
    public PlayerInput player;
    
    private Vector2 look_vector;
    private bool mouse = false;
    private bool wantToJump = false;
    private Vector3 directionIntent;

    // Start is called before the first frame update
    void Start()
    {
        player.actions["move"].performed += ctx => addMove(ctx.ReadValue<Vector2>());
        player.actions["move"].canceled += ctx => addMove(ctx.ReadValue<Vector2>());
        player.actions["look_GP"].performed += ctx => RotateCamera(ctx.ReadValue<Vector2>());
        player.actions["look_GP"].canceled += ctx => RotateCamera(Vector2.zero);
        player.actions["look_mouse"].performed += ctx => RotateCameraFromMouse(ctx.ReadValue<Vector2>());
        player.actions["look_mouse"].canceled += ctx => RotateCameraFromMouse(Vector2.zero);
        player.actions["jump"].performed += ctx => JumpIntent();
    }

    private void addMove(Vector2 movement)
    {
        directionIntent = (Vector3.forward * movement.y) + (Vector3.right * movement.x);
    }
    private void RotateCamera(Vector2 direction)
    {
        look_vector = direction;
    }    
    
    private void RotateCameraFromMouse(Vector2 direction)
    {
        if (mouse)
        {
            look_vector = direction/4;
        }
    }

    private void JumpIntent()
    {
        if ((Physics.SphereCast(playerTransform.position + (Vector3.up * 0.55f),
                0.45f, 
                Vector3.down, 
                out var _hitInfo, 
                1.0f)))
        {
            wantToJump = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouse = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RotateCamera(Vector2.zero);
            mouse = false;
        }

        var rot = playerTransform.rotation.eulerAngles;
        rot.y += look_vector.x * 90 * Time.deltaTime*sensibilityH;
                
        playerTransform.rotation = Quaternion.Euler(rot);

        var roth = headTransform.localRotation.eulerAngles;
        roth.x -= look_vector.y * 90 * Time.deltaTime*sensibilityV;
        headTransform.localRotation = Quaternion.Euler(roth);

    }

    private void FixedUpdate()
    {
        playerRigibody.linearVelocity = (playerTransform.rotation * directionIntent.normalized) * (speed) + Vector3.up * playerRigibody.linearVelocity.y;
        if (wantToJump)
        {
            playerRigibody.AddForce(Vector3.up*40f, ForceMode.Impulse);
            wantToJump = false;
        }
    }
}
