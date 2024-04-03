using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string door_id = "door_id_blank";
    public bool is_open = false;
    [HideInInspector]public Transform door_object;
    [HideInInspector]public Vector3 start_pos = Vector3.zero;

    public Vector3 desired_position;
    public Vector3 desired_rotation;

    public void Start()
    {
        Game.doors.Add(this);
        door_object = transform.GetChild(3);
        start_pos = door_object.localPosition;
        desired_position = start_pos;
        door_object.localEulerAngles = new Vector3(0, 180, 0);
        desired_rotation = door_object.localEulerAngles;
    }

    public void Update()
    {
        door_object.localPosition = Vector3.Lerp(door_object.localPosition, desired_position, Time.deltaTime);
        door_object.localEulerAngles = Vector3.Lerp(door_object.localEulerAngles, desired_rotation, Time.deltaTime);
    }

    public void OnInteract()
    {
        ClientSend.RequestToggleDoor(door_id);
    }
    public void SetOpen(bool is_open)
    {
        this.is_open = is_open;
        Vector3 offset = Vector3.zero;
        Vector3 rot = new Vector3(0, 180, 0);
        if (is_open)
        {
            float width = door_object.localScale.x;
            float yaw = Mathf.Deg2Rad * transform.localEulerAngles.y;
            offset.x = Mathf.Sin(yaw) * width - width * 0.5f;// - width * 0.5f - door_object.localScale.z;
            offset.z = Mathf.Cos(yaw) -width * 0.5f + door_object.localScale.z * 2.0f;// Mathf.Cos(Mathf.Deg2Rad * transform.localEulerAngles.y) - width * 0.25f;
            rot = new Vector3(0, 90, 0);
            Debug.Log($"{Mathf.Sin(yaw)} {Mathf.Cos(yaw)}");
        }
        desired_position =  offset;
        desired_rotation = rot;
    }
}
