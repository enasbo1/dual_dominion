using UnityEngine;
using UnityEngine.InputSystem;

public class ShowMove : MonoBehaviour
{
    public Transform playerCamera;
    public PlayerInput player;
    public Transform cameraTarget;
    
    private Vector2 _lookVector;
    private bool _mouse;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        _lookVector = direction;
    }    
    
    private void RotateCameraFromMouse(Vector2 direction)
    {
        if (_mouse)
        {
            _lookVector = direction/4;
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

        Quaternion cameraRotation = playerCamera.rotation;
        Vector3 cameraEulerRotation = cameraRotation.eulerAngles;
        
        cameraEulerRotation.y += _lookVector.x * 90 * Time.deltaTime;
        cameraEulerRotation.x -= _lookVector.y * 90 * Time.deltaTime;
        cameraRotation = Quaternion.Euler(cameraEulerRotation);
        playerCamera.rotation = cameraRotation;
        playerCamera.position = cameraTarget.position + (cameraRotation * Vector3.back * 10);
    }
}