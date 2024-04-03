using System;

using UnityEngine;

//These items have no actual "functionality"
//We tell the server we want to pick these up
//If we do then they will have functionality with their inventoryitem
[System.Serializable]
public class WorldItem : MonoBehaviour
{
    public string id = "item_id";
    public string name = "Item";
    public Color color = Color.white;
    public Sprite image = null;

    public void OnInteract()
    {
        Debug.Log($"Requesting to pickup item {id}");
        ClientSend.RequestItemPickup(id);
    }
}