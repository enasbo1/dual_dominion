using UnityEngine;
using UnityEngine.InputSystem;

public class show_move : MonoBehaviour
{
    public Transform playerCamera;
    public PlayerInput player;
    public Transform cameraTarget;
    
    private Vector2 look_vector;
    private bool mouse = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.actions["look_GP"].performed += ctx => RotateCamera(ctx.ReadValue<Vector2>());
        player.actions["look_GP"].canceled += ctx => RotateCamera(Vector2.zero);
        player.actions["look_mouse"].performed += ctx => RotateCameraFromMouse(ctx.ReadValue<Vector2>());
        player.actions["look_mouse"].canceled += ctx => RotateCameraFromMouse(Vector2.zero);
        Cursor.lockState = CursorLockMode.Locked;
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
        var rot = playerCamera.rotation.eulerAngles;
        rot.y += look_vector.x * 90 * Time.deltaTime;
        rot.x -= look_vector.y * 90 * Time.deltaTime;
        playerCamera.rotation = Quaternion.Euler(rot);
        playerCamera.position = cameraTarget.position + (playerCamera.rotation * Vector3.back * 10);
    }
}
