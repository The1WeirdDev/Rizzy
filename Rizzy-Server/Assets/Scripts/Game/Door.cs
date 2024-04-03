using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Door
{
    public GameObject door_object;
    public string id = "door_id";

    //If it is originally set to locked
    //Then when unlocked we set to false
    public bool is_locked = false;
    [Tooltip("This field is the required item id if locked. If it isn't locked then leave it null")]
    public string required_item = null;

    [HideInInspector]
    public bool is_open = false; // Is it closed or open. Nothing to do wether its unlocked or not

    [HideInInspector] public float last_toggled_time = 0;
    public static float debounce = 2f;

    public void AttemptToggle()
    {
        if (Time.time < last_toggled_time + debounce)
            return;
        last_toggled_time = Time.time;
        is_open = !is_open;

        Message m = Message.Create(MessageSendMode.Reliable, (ushort)ServerMessages.SetDoorStatus);
        m.AddString(id);
        m.AddBool(is_open);
        ServerSend.SendMessageToAll(m);
    }
}
