using System;

using UnityEngine;
using Riptide;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ClientHandle
{
    public static List<ushort> other_players_ids;

    [MessageHandler((ushort)ServerMessages.SyncTick)]
    public static void HandleSetTick(Message message)
    {
        Game.SetTick(message.GetUInt());
    }
    [MessageHandler((ushort)ServerMessages.SetLobbyData)]
    public static void HandleLobbyData(Message message)
    {
        MainMenuUIHandler.OnConnectedToServer(message.GetString());
    }

    [MessageHandler((ushort)ServerMessages.SetHost)]
    public static void HandleSetHost(Message message)
    {
        ushort id = message.GetUShort();
        Game.is_host = id == GameClient.client.Id;
        if (MainMenuUIHandler.instance)
            MainMenuUIHandler.instance.start_button.gameObject.SetActive(Game.is_host);

        Debug.Log("We Are " + (Game.is_host ? "Host" : "Not Host"));
    }

    [MessageHandler((ushort)ServerMessages.StartGame)]
    public static void HandleStartGame(Message message)
    {
        Debug.Log("Game starting");
        Game.monster_id = message.GetUShort();
        Game.monster_wait_time = message.GetByte();
        Game.is_monster = Game.monster_id == GameClient.client.Id;
        int amount_of_players = message.GetInt();

        other_players_ids = new List<ushort>();
        for (int i = 0; i < amount_of_players; i++)
        {
            ushort id = message.GetUShort();
            if (id != GameClient.client.Id)
                other_players_ids.Add(id);
        }

        SceneManager.UnloadSceneAsync("MainMenu");
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Riggy-Map-0", LoadSceneMode.Additive);
    }

    [MessageHandler((ushort)ServerMessages.PlayerLeft)]
    public static void OnPlayerLeft(Message message)
    {
        ushort id = message.GetUShort();
        NonLocalPlayerObject p = Game.other_players.GetValueOrDefault(id);

        if (p != null)
        {
            UnityEngine.Object.Destroy(p.game_object);
            Game.other_players.Remove(id);
        }
    }

    public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        for (int i = 0; i < other_players_ids.Count; i++)
        {
            Game.CreatePlayer(other_players_ids[i]);
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    [MessageHandler((ushort)ServerMessages.SetPlayersPositionAndRotationData)]
    public static void HandleSetPlayerPosition(Message message)
    {
        uint tick = message.GetUInt();
        int amount_of_players = message.GetInt();

        for (var i = 0; i < amount_of_players; i++)
        {
            ushort id = message.GetUShort();
            bool is_crouching = message.GetBool();

            Vector3 position = new Vector3(message.GetFloat(), message.GetFloat(), message.GetFloat());
            Vector3 rotation = new Vector3(message.GetFloat(), message.GetFloat(), message.GetFloat());

            NonLocalPlayerObject p = Game.GetNonLocalPlayer(id);

            if (p != null)
            {
                p.script.SetCrouchingMode(is_crouching);
                p.script.AddPositionToUpdate(tick, position, rotation);
            }
        }
    }

    [MessageHandler((ushort)ServerMessages.PlayerDied)]
    public static void HandleDied(Message message)
    {
        ushort id = message.GetUShort();

        if (id == GameClient.client.Id)
        {
            //It is ourself
            Game.instance.lplr_script.OnDeath();
            return;
        }

        NonLocalPlayerObject p = Game.GetNonLocalPlayer(id);

        if (p != null)
        {
            Debug.Log($"PLR {id} Died.");
            UnityEngine.Object.Destroy(p.game_object);
            Game.other_players.Remove(id);
            //p.game_object.transform.SetPositionAndRotation(position, Quaternion.Lerp(p.game_object.transform.rotation, Quaternion.Euler(0.0f, Mathf.Rad2Deg * rotation.y, 0.0f), Time.fixedTime));
            //p.view_object.transform.localRotation = Quaternion.Lerp(p.view_object.transform.localRotation, Quaternion.Euler(Mathf.Rad2Deg * rotation.x, 0, Mathf.Rad2Deg * rotation.z), Time.fixedTime);
        }
    }


    [MessageHandler((ushort)ServerMessages.SetItem)]
    public static void OnSetItem(Message message)
    {
        /*
         * Server Packet Idea
         * ushort - player id
         * bool - has item
         * string item-name
         */
        ushort plr_id = message.GetUShort();
        bool has_item = message.GetBool();
        string item_id = message.GetString();

        //Shows or hides the item in the world
        for (var i = 0; i < Game.instance.items.Length; i++)
        {
            WorldItem item = Game.instance.items[i];
            if (item.id != item_id) continue;

            item.gameObject.SetActive(!has_item);

            if (plr_id == GameClient.client.Id)
            {
                LocalSurvivor survivor = ((LocalSurvivor)Game.instance.lplr_script);
                survivor.has_item = has_item;
                survivor.can_interact = true;
                InGameGUIHandler.instance.SetImage(item.image);
            }
            else
            {

            }
            return;
        }
    }


    [MessageHandler((ushort)ServerMessages.SetDoorStatus)]
    public static void OnSetDoorStatus(Message message)
    {
        string door_id = message.GetString();
        bool is_open = message.GetBool();

        for(int i = 0; i < Game.doors.Count; i++)
        {
            Door door = Game.doors[i];

            if (door.door_id != door_id) continue;
            door.SetOpen(is_open);
            return;
        }
    }
}