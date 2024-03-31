using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalSurvivor : LocalPlayer
{
    bool is_crouching = false;


    private void Awake()
    {
        GetLocalComponents();
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
        CalculateCameraBobbing();
        CalculateCameraAndBodyMovement();

        if (is_alive == false) return;
        if (Input.GetKeyDown(KeyCode.C))
        {
            is_crouching = !is_crouching;
            speed_multiplier = is_crouching ? 0.725f : 1.0f;
            ClientSend.SendCrouching(is_crouching);
        }
    }

    private void FixedUpdate()
    {
        BaseFixedUpdate();
    }
}
