using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Riptide;
using Riptide.Utils;

using UnityEngine;

public class ClientSend
{
    public static void SendStart()
    {
        GameClient.client.Send(Message.Create(MessageSendMode.Reliable, (ushort)ClientMessages.Start));
    }

    public static void SendPositionAndRot(Vector3 position, Vector3 rotation)
    {
        Message m = Message.Create(MessageSendMode.Unreliable, (ushort)ClientMessages.SetPositionAndRot);
        m.AddFloat(position.x);
        m.AddFloat(position.y);
        m.AddFloat(position.z);
        m.AddFloat(rotation.x);
        m.AddFloat(rotation.y);
        m.AddFloat(rotation.z);
        GameClient.client.Send(m);
    }

    public static void SendCrouching(bool value)
    {
        Message m = Message.Create(MessageSendMode.Reliable, (ushort)ClientMessages.SetCrouchingMode);
        m.Add(value);
        GameClient.client.Send(m);
    }
    public static void SendRequestKill()
    {
        Message m = Message.Create(MessageSendMode.Reliable, (ushort)ClientMessages.SendRequestKill);

        GameClient.client.Send(m);
    }
}