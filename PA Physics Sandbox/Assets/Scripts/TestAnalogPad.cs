using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class TestAnalogPad : MonoBehaviour
{
    public Transform outTarget;
    public PlayerInput player;
    // public new Transform camera;
    
    private Vector2 _moveVector;
    private Vector2 _lookVector;
    // private bool _mouse = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.actions["move"].performed += ctx => _moveVector = ctx.ReadValue<Vector2>();
        player.actions["move"].canceled += ctx => _moveVector = ctx.ReadValue<Vector2>();
        // player.actions["look_GP"].performed += ctx => RotateCamera(ctx.ReadValue<Vector2>());
        // player.actions["look_GP"].canceled += ctx => RotateCamera(Vector2.zero);
        // player.actions["look_mouse"].performed += ctx => RotateCameraFromMouse(ctx.ReadValue<Vector2>());
        // player.actions["look_mouse"].canceled += ctx => RotateCameraFromMouse(Vector2.zero);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // private void RotateCamera(Vector2 direction)
    // {
    //     _lookVector = direction;
    // }    
    // 
    // private void RotateCameraFromMouse(Vector2 direction)
    // {
    //     if (_mouse)
    //     {
    //         _lookVector = direction/4;
    //     }
    // }
    
    // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         _mouse = true;
    //     }
    // 
    //     if (Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         _mouse = false;
    //     }
    // }
    
    // Update is called once per frame
    void FixedUpdate()
    {


        Vector3 test = new Vector3(
            _moveVector.x,
            _moveVector.y,
            outTarget.position.z
        );
        
        outTarget.position = test;
        
        // if (_mouse == false)
        // {
        //     RotateCamera(Vector2.zero);
        // }
        //     
        // var rot = camera.rotation.eulerAngles;
        // rot.y += _lookVector.x * 90 * Time.deltaTime;
        // rot.x -= _lookVector.y * 90 * Time.deltaTime;
        // camera.rotation = Quaternion.Euler(rot);
        // camera.position = camera.rotation * Vector3.back * 10;
    }
}
