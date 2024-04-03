using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Riptide;
using Riptide.Utils;

using UnityEngine;

public class ClientSend
{
    public static void SendMessage(Message message)
    {
        GameClient.client.Send(message);
    }
    public static void SendStart()
    {
        SendMessage(Message.Create(MessageSendMode.Reliable, (ushort)ClientMessages.Start));
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
        SendMessage(m);
    }

    public static void SendCrouching(bool value)
    {
        Message m = Message.Create(MessageSendMode.Reliable, (ushort)ClientMessages.SetCrouchingMode);
        m.Add(value);
        SendMessage(m);
    }
    public static void SendRequestKill()
    {
        Message m = Message.Create(MessageSendMode.Reliable, (ushort)ClientMessages.SendRequestKill);

        SendMessage(m);
    }

    public static void RequestItemPickup(string item_id)
    {
        Message m = Message.Create(MessageSendMode.Reliable, (ushort)ClientMessages.RequestItemPickup);
        m.AddString(item_id);
        SendMessage(m);
    }

    public static void RequestUseItem()
    {
        Message m = Message.Create(MessageSendMode.Reliable, (ushort)ClientMessages.UseItem);
        SendMessage(m);
    }

    public static void RequestToggleDoor(string door_id)
    {
        Message m = Message.Create(MessageSendMode.Reliable, (ushort)ClientMessages.SetDoorStatus);
        m.AddString(door_id);
        SendMessage(m);
    }
}