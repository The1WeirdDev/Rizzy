using System;

using UnityEngine;
public class TransformUpdate
{
    public uint tick { get; private set; }
    public bool is_teleport { get; private set; }
    public Vector3 position { get; private set; }

    public TransformUpdate(uint tick, bool is_teleport, Vector3 position)
    {
        this.tick = tick;
        this.is_teleport = is_teleport;
        this.position = position;
    }
}

public class NonLocalPlayerTransformUpdate
{
    public uint tick { get; private set; }
    public Vector3 position { get; private set; }
    public Vector3 rotation { get; private set; }

    public NonLocalPlayerTransformUpdate(uint tick, Vector3 position, Vector3 rotation)
    {
        this.tick = tick;
        this.position = position;
        this.rotation = rotation;
    }
}