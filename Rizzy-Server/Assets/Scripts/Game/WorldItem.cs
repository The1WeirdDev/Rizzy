using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class WorldItem
{
    public Transform item_transform;
    public string id = "item_id";
    public MonoScript script;
    [HideInInspector]
    public bool is_picked_up = false;
}
