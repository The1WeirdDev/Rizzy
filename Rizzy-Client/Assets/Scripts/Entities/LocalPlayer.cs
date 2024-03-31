using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LocalPlayer : Character
{
    public float gravity_constant = -9.81f;
    public float speed = 2.0f;
    public float speed_multiplier = 1.0f;
    public bool is_grounded = true;

    protected Vector3 movement_direction = Vector3.zero;
    public Vector3 velocity = Vector3.zero;

    [Header("Camera Movement")]
    public float sensitivity = 1.0f;

    //Camera Bobbing
    protected float camera_bob_yaw_amount = 0.45f;
    protected float camera_bob_yaw_speed = 2.0f;
    protected float camera_bob_pitch_amount = 0.35f;
    protected float camera_bob_pitch_speed = 2.0f;

    protected float camera_bob_yaw = 0.0f;
    protected float camera_bob_pitch = 0.0f;

    protected float camera_tilt_x = 0.0f;
    protected float camera_tilt_degrees = 4.5f;

    public bool is_alive = true;

    protected void Awake()
    {
        Screen.fullScreen = false;
    }

    protected void GetLocalComponents()
    {
        controller = GetComponent<CharacterController>();
        camera = this.transform.GetChild(0).GetComponent<Camera>();
        ground_check = this.transform.GetChild(1).GetChild(0);
    }
    protected void BaseUpdate()
    {
        
    }
    protected void BaseFixedUpdate()
    {
        if (Game.server_tick % 2 == 0)
            ClientSend.SendPositionAndRot(controller.transform.position, Quaternion.ToEulerAngles(camera.transform.localRotation * this.transform.localRotation));
    }

    public void LockMouse()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    protected void CalculateCameraBobbing()
    {
        camera_bob_yaw = Mathf.Cos(camera_bob_yaw_speed * Time.time) * camera_bob_yaw_amount;
        camera_bob_pitch = Mathf.Sin(camera_bob_pitch_speed * Time.time) * camera_bob_pitch_amount;
    }

    protected void CalculateCameraAndBodyMovement()
    {
        if (!is_alive) return;

        //Get Axes
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        //Getting Mouse Movement and rotating
        yaw += x;
        pitch -= y * sensitivity;
        pitch = Mathf.Clamp(pitch, -80.0f, 85.0f);

        camera_tilt_x = Mathf.Lerp(camera_tilt_x, -horizontal * camera_tilt_degrees, Mathf.Clamp(Time.deltaTime * 8.0f, 0.0f, 1.0f));
        this.transform.localRotation = Quaternion.Euler(0, yaw + camera_bob_yaw, 0);
        camera.transform.localRotation = Quaternion.Euler(pitch + camera_bob_pitch, 0, camera_tilt_x);

        //Movement
        is_grounded = Physics.Raycast(ground_check.position, Vector3.down, 0.1f);
        velocity.y += gravity_constant * Time.deltaTime;
        if (is_grounded == true) velocity.y = 0;

        Vector3 movement_direction = transform.forward * vertical + transform.right * horizontal;

        controller.Move(velocity * Time.deltaTime);
        controller.Move(movement_direction * speed * speed_multiplier * Time.deltaTime);
    }

    public void OnDeath()
    {
        is_alive = false;
        InGameGUIHandler.instance.ShowDeathGUI();
    }
}
