using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LocalSurvivor : LocalPlayer
{
    public bool is_crouching = false;
    public bool can_interact = true;
    public bool has_item = false;

    public GameObject inventory_object = null;

    public Inventory inventory;

    private void Awake()
    {
        GetLocalComponents();
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
        if (is_alive == false) return;
        BaseUpdate();

        CalculateCameraBobbing();
        CalculateCameraAndBodyMovement();

        //Crouching
        if (Input.GetKeyDown(KeyCode.C))
        {
            is_crouching = !is_crouching;
            speed_multiplier = is_crouching ? 0.725f : 1.0f;
            view_object.localPosition = new Vector3(0, is_crouching ? 0.225f : 0.5f, 0);
            ClientSend.SendCrouching(is_crouching);
        }

        //Picking up items
        if (Input.GetKeyDown(KeyCode.F))
        {
            float yaw_rad = yaw * Mathf.Deg2Rad;
            float pitch_rad = pitch * Mathf.Deg2Rad;
            Vector3 ray_start = view_object.position;
            Vector3 ray_dir = Vector3.Normalize(new Vector3(Mathf.Sin(yaw_rad) * Mathf.Cos(pitch_rad), -Mathf.Sin(pitch_rad), Mathf.Cos(yaw_rad) * Mathf.Cos(pitch_rad)));
            float length = 5.0f;

            Ray r = new Ray(ray_start, ray_dir);
            if(Physics.Raycast(r, out RaycastHit hit_info, length))
            {
                if (hit_info.collider.gameObject.TryGetComponent(out WorldItem world_item))
                {
                    world_item.OnInteract();
                    can_interact = false;
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.E) )
        {
            ClientSend.RequestUseItem();
        }
    }

    private void FixedUpdate()
    {
        BaseFixedUpdate();
    }

    public void OnItemDrop(string item_id)
    {

    }
}
