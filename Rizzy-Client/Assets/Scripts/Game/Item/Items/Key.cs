using System;

using UnityEngine;

public class Key : MonoBehaviour, WorldItem
{
    public string name = "key";
    public Color color = Color.white;
    public void OnInteract()
    {
        Debug.Log("Interacted with key");
    }
}