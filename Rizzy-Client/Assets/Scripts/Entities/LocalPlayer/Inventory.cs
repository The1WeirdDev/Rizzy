using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [HideInInspector]
    public LocalSurvivor local_player;
    
    public void SetLocalPlayer(LocalSurvivor local_player) { this.local_player = local_player; }

    private void Awake()
    {
    }
}