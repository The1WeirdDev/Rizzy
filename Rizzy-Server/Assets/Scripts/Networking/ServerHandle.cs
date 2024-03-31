using System;
using System.Collections.Generic;
using System.Linq;
using Riptide;
using Riptide.Utils;

using UnityEngine;

using Vector3 = UnityEngine.Vector3;
public class ServerHandle
{
    public static void OnClientConnnected(object sender, ServerConnectedEventArgs e)
    {
        Debug.Log($"Client connected with the id of {e.Client.Id}");

        Player p = new Player(e.Client.Id);
        Player.players.Add(e.Client.Id, p);

        //Send Client Data
        ServerSend.SendServerData(e);

        if (Game.host == null)
        {
            Game.host = p;
            ServerSend.AlertHost();
        }

    }
    public static void OnClientDisconnected(object sender, ServerDisconnectedEventArgs e)
    {
        Debug.Log($"Client disconnected with the id of {e.Client.Id}");
        
        //Alert Client left
        Message left_message = Message.Create(MessageSendMode.Reliable, (ushort)ServerMessages.PlayerLeft);
        left_message.AddUShort(e.Client.Id);
        GameServer.server.SendToAll(left_message, e.Client.Id);

        ushort id = e.Client.Id;
        Player p = Player.players.GetValueOrDefault(id);
        if (p.is_alive) Game.alive_players--;

        //Remove from list and check for new host
        if (p == Game.monster_plr)
            Game.monster_plr = null;
        if (p == Game.host)
            Game.host = null;

        Player.players.Remove(id);

        if (Player.players.Count < 1)
            Game.End();
        else
        {
            Game.host = Player.players.ElementAt(UnityEngine.Random.Range(0, Player.players.Count)).Value;
            ServerSend.AlertHost();
        }
    }

    public static void OnConnectionFailed(object sender, ServerConnectionFailedEventArgs e)
    {

    }
    [MessageHandler((ushort)ClientMessages.Start)]
    public static void HandleStart(ushort from_client_id, Message message)
    {
        if (Game.host == null || Game.is_game_started) return;

        if(Game.host.id == from_client_id)
        {
            Game.StartGame();
        }
    }

    [MessageHandler((ushort)ClientMessages.SetPositionAndRot)]
    public static void HandleSetPosition(ushort from_client_id, Message message)
    {
        Vector3 pos = new Vector3(message.GetFloat(), message.GetFloat(), message.GetFloat());
        Vector3 rotaton = new Vector3(message.GetFloat(), message.GetFloat(), message.GetFloat());

        Player.players[from_client_id].SetPosition(pos);
        Player.players[from_client_id].SetRotation(rotaton);
    }

    [MessageHandler((ushort)ClientMessages.SendRequestKill)]
    public static void HandleRequestKill(ushort from_client_id, Message message)
    {
        if (Time.time < Game.time_started + Game.monster_wait_time) return;

        Player p = Player.players[from_client_id];

        if(p != Game.monster_plr)
        {
            Debug.Log($"PLR {from_client_id} tried to kill something but doesn't know they are not the monster.");
            return;
        }

        float closest_distance = Game.monster_kill_range;
        Player closest_player = null;

        //Get closest player
        foreach(var player in  Player.players)
        {
            if(player.Value != p && player.Value.is_alive)
            {
                float distance = (player.Value.position -  p.position).magnitude;
                if(distance < closest_distance)
                {
                    closest_distance = distance;
                    closest_player = player.Value;
                }
            }
        }
        //Check kill cooldown and stuff;
        if (Time.time < Game.last_kill_time + Game.monster_wait_time) return;

        if (closest_player != null)
        {
            Game.last_kill_time = Time.time;
            Debug.Log($"Piggy attempting to kill plr {closest_player.id}");
            closest_player.OnDeath();
            ServerSend.AlertDeath(closest_player.id);
        }
    }

    [MessageHandler((ushort)ClientMessages.SetCrouchingMode)]
    public static void OnSetCrouchingMode(ushort from_client_id, Message m)
    {
        bool value = m.GetBool();


        Player p = Player.players[from_client_id];

        if(p != null)
        {
            p.SetCrouchingMode(value);
        }
    }
}  