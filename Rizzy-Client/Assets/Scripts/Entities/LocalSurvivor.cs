using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LocalSurvivor : LocalPlayer
{
    bool is_crouching = false;

    public Transform view_object;

    public Inventory inventory;

    private void Awake()
    {
        GetLocalComponents();
        view_object = transform.GetChild(0);
        inventory = gameObject.AddComponent<Inventory>();
        inventory.SetLocalPlayer(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        LockMouse();
        InGameGUIHandler.instance.EnableTimeRemainingGUI();
    }
     
    // Update is called once per frame
    void Update()
    {
        BaseUpdate();

        if (is_alive == false) return;

        CalculateCameraBobbing();
        CalculateCameraAndBodyMovement();
        if (Input.GetKeyDown(KeyCode.C))
        {
            is_crouching = !is_crouching;
            speed_multiplier = is_crouching ? 0.725f : 1.0f;
            view_object.localPosition = new Vector3(0, is_crouching ? 0.225f : 0.5f, 0);
            ClientSend.SendCrouching(is_crouching);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            float yaw_rad = yaw * Mathf.Deg2Rad;
            float pitch_rad = pitch * Mathf.Deg2Rad;
            Vector3 ray_start = view_object.position;
            Vector3 ray_dir = Vector3.Normalize(new Vector3(Mathf.Sin(yaw_rad), -Mathf.Sin(pitch_rad), Mathf.Cos(yaw_rad)));
            float length = 5.0f;

            Ray r = new Ray(ray_start, ray_dir);
            Debug.DrawRay(ray_start, ray_dir * length, Color.red, 15.0f, false);
            if(Physics.Raycast(r, out RaycastHit hit_info, length))
            {
                if(hit_info.collider.gameObject.TryGetComponent(out WorldItem world_item))
                {
                    world_item.OnInteract();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        BaseFixedUpdate();
    }
}
