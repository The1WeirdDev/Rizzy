using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string id = "item";
    public int index = -1;
    public Player player;
    public WorldItem world_item;

    public abstract void OnUse();
}
