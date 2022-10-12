using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField]
    private float walkSpeed;//! Object Walking Speed

    [SerializeField]
    private float lookSensitivity;//! Camera rotation sensitivity
    [SerializeField]
    private Camera main_camera;


    [SerializeField]
    private float camera_Roatation_Limit;//limit camera rotation



    private float camera_rotation_X = 0f;
    private float camera_rotation_Y = 180f;

    private Rigidbody myRigid;
    void Start()
    {
        myRigid = GetComponent<Rigidbody>();//! Get this Rigid
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        camera_rotation();
        character_rotation();
    }

    /*
    !Input Keyboard direction for horizon, verti   (-1 or 1)
    */
    private void Move()
    {
        float _moveX = Input.GetAxisRaw("Horizontal");
        float _moveZ = Input.GetAxisRaw("Vertical");

        // These vector3 is mapping direction to vector
        Vector3 _moveHorizontal = transform.right * _moveX;
        Vector3 _moveVertical   = transform.forward * _moveZ;

        
        Vector3 _velocity  = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

        //rigidbody interpolating(보간) function
        //this.transform.position updated to position + velocity vector * Time.deltaTime
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);

    }


    //Rotationing camera
    private void camera_rotation()
    {
        float Input_rotationX = Input.GetAxisRaw("Mouse Y");
    
        float Input_rotationY = Input.GetAxisRaw("Mouse X");

        float update_camera_rotationX = Input_rotationX * lookSensitivity;
        camera_rotation_X -= update_camera_rotationX;
        camera_rotation_X = Mathf.Clamp(camera_rotation_X, -camera_Roatation_Limit, camera_Roatation_Limit);
        
        
        float update_camera_rotationY = Input_rotationY * lookSensitivity;
        camera_rotation_Y -= update_camera_rotationY;
        camera_rotation_Y = Mathf.Clamp(camera_rotation_Y, -camera_Roatation_Limit, camera_Roatation_Limit);
        
        // 카메라 각도 제한.
        main_camera.transform.localEulerAngles = new Vector3(camera_rotation_X, camera_rotation_Y, 0);
    }
    private void character_rotation()
    {
      
    }
}
