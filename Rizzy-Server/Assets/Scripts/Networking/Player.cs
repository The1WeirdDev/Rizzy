using System;
using System.Numerics;
using System.Collections.Generic;

using UnityEngine;

using Riptide;
using Riptide.Utils;

using Vector3 = UnityEngine.Vector3;

public class Player
{
    public static Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();
    public ushort id = 0;
    public bool is_monster = false;
    public bool is_alive = true;
    public bool is_crouching = false;

    //Host item scripts
    public GameObject game_object;
    public Vector3 position;
    public Vector3 rotation;

    public Component item = null;

    public Player(ushort id)
    {
        this.id = id;
        this.is_alive = true;
    }

    public void DestroyGameObject()
    {
        UnityEngine.Object.Destroy(game_object);
    }

    public void SetPosition(Vector3 position)
    {
        this.position = position;
    }
    public void SetRotation(Vector3 rotation)
    {
        this.rotation = rotation;
    }

    public void OnDeath()
    {
        is_alive = false;
        Game.alive_players--;
    }

    public void SetCrouchingMode(bool value)
    {
        is_crouching = value;
    }
}