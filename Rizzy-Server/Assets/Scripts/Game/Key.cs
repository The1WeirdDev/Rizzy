using System;
using System.Linq;
using UnityEngine;

public class Key : Item
{
    public override void OnUse()
    {
        Door closest_door = null;
        float closest_distance = 5f;
        Debug.Log("On Use");
        for (int i = 0; i < Game.instance.current_map.doors.Length; i++)
        {
            Door door = Game.instance.current_map.doors[i];

            if (door.is_locked == false) continue;

            if (door.required_item != this.id)continue;

            float distance = (door.door_object.transform.localPosition - player.position).magnitude;
            if (distance < closest_distance)
            {
                Debug.Log("TEST4");
                closest_door = door;
                closest_distance = distance;
            }
        }

        if (closest_door != null)
        {
            Debug.Log("Unlocked door");
            closest_door.is_locked = false;
        }
    }
}