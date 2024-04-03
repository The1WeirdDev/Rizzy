using System;

using UnityEngine;

using Riptide;
using Riptide.Utils;


class ServerSend{
    //Doesnt contain every message sent

    public static void SendMessage(ushort id, Message message)
    {
        GameServer.server.Send(message, id);
    }
    public static void SendMessageToAll(Message message)
    {
        GameServer.server.SendToAll(message);
    }
    public static void SendMessageToAll(Message message, ushort id)
    {
        GameServer.server.SendToAll(message, id);
    }
    public static void SendTickSync()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)ServerMessages.SyncTick);
        message.Add(Game.tick);
        GameServer.server.SendToAll(message);
    }
    public static void SendServerData(ServerConnectedEventArgs e)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerMessages.SetLobbyData);
        message.AddString(GameServer.server_name);
        e.Client.Send(message);
    }

    public static void SendPositionAndRotationDatas()
    {
        Message m = Message.Create(MessageSendMode.Unreliable, (ushort)ServerMessages.SetPlayersPositionAndRotationData);
        m.AddUInt(Game.tick);
        m.AddInt(Game.alive_players);
        foreach (var plr in Player.players)
        {
            if (!plr.Value.is_alive) continue;
            m.AddUShort(plr.Key);
            m.AddBool(plr.Value.is_crouching);
            m.AddFloat(plr.Value.position.x);
            m.AddFloat(plr.Value.position.y);
            m.AddFloat(plr.Value.position.z);
            m.AddFloat(plr.Value.rotation.x);
            m.AddFloat(plr.Value.rotation.y);
            m.AddFloat(plr.Value.rotation.z);

        }
        GameServer.server.SendToAll(m);
    }
    public static void AlertHost()
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerMessages.SetHost);
        message.AddUShort(Game.host.id);
        GameServer.server.SendToAll(message);
    }

    public static void OnGameStarted()
    {
        Message m = Message.Create(MessageSendMode.Reliable, (ushort)ServerMessages.StartGame);
#if UNITY_EDITOR
        if(Game.allow_monster)
            m.AddUShort(Game.monster_plr.id);
        else
            m.AddUShort(0);
#else
        m.AddUShort(Game.monster_plr.id);
#endif
        m.AddByte(Game.monster_wait_time);
        m.AddInt(Player.players.Count);
        foreach(var p in Player.players){
            m.AddUShort(p.Key);
        }
        GameServer.server.SendToAll(m);
    }

    public static void AlertDeath(ushort id)
    {
        Message m = Message.Create(MessageSendMode.Reliable, (ushort)ServerMessages.PlayerDied);
        m.AddUShort(id);
        GameServer.server.SendToAll(m);

    }
}