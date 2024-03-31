using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LocalMonster : LocalPlayer
{
    public bool is_countdown_finished = false;
    private void Awake()
    {
        GetLocalComponents();
    }
    // Start is called before the first frame update
    void Start()
    {
        LockMouse();
        StartCoroutine(WaitForCountdown());
    }

    // Update is called once per frame
    void Update()
    {
        BaseUpdate();
        CalculateCameraBobbing();

        if (is_countdown_finished == false) return;

        CalculateCameraAndBodyMovement();

        if (Input.GetMouseButtonDown(0))
        {
            ClientSend.SendRequestKill();
        }
    }
    private void FixedUpdate()
    {
        BaseFixedUpdate();
    }
    IEnumerator WaitForCountdown()
    {
        InGameGUIHandler.instance.EnableCountdownGUI();
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(Game.monster_wait_time);
        is_countdown_finished = true;
        //After we have waited 5 seconds print the time again.
        InGameGUIHandler.instance.DisableCountdownGUI();
    }
}
