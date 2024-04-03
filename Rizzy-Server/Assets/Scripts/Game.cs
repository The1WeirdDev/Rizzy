using Riptide;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public enum ClientMessages : ushort
{
    Start,
    SetPositionAndRot,
    SendRequestKill,
    RequestItemPickup,
    UseItem,
    SetCrouchingMode,
    SetDoorStatus
}
public enum ServerMessages : ushort
{
    SyncTick,
    SetLobbyData,
    SetHost,
    StartGame,
    PlayerLeft,
    PlayerDied,
    SetPlayersPositionAndRotationData,
    SetItem,
    SetDoorStatus
}
public class Game : MonoBehaviour
{
    public static Game instance;
    public static uint tick;
    public static bool is_game_started = false;

    public static Player host;
    public static Player monster_plr;

    public static byte monster_wait_time = 3;
    public static float monster_kill_range = 0.5f;
    public static float monster_kill_cooldown = 1.0f;

    public static float time_started = 0;
    public static float last_kill_time = 0;

    public static int alive_players = 0;

    //This is just for local player testing in unity
    public static bool allow_monster = false;

    [HideInInspector]
    public Map current_map;
    public Map map1;

    public void Awake()
    {
        instance = this;
        current_map = map1;
    }

    public static void Init()
    {
        host = null;
        monster_plr = null;
        is_game_started = false;
        Player.players.Clear();

    }
    public static void StartGame()
    {
        if (Player.players.Count < 1) return;

        //Reset Characters
        foreach(var p in Player.players)
        {
            Player player = p.Value;
            player.is_monster = false;
            player.is_alive = true;
            player.SetPosition(Vector3.zero);
            player.item = null;
        }

        for(var i = 0; i < instance.current_map.items.Length; i++)
        {
            instance.current_map.items[i].is_picked_up = false;
        }
        alive_players = Player.players.Count;
        is_game_started = true;
        time_started = Time.time;
        last_kill_time = Time.time;

#if UNITY_EDITOR
        if (allow_monster)
            monster_plr = Player.players.ElementAt(Random.Range(1, Player.players.Count) - 1).Value;
        else
            monster_plr = null;
#else
        monster_plr = Player.players.ElementAt(Random.Range(1, Player.players.Count) - 1).Value;
#endif
        ServerSend.OnGameStarted();
        Debug.Log("Game Started");
    }

    public static void Tick()
    {
        if (tick % 200 == 0) ServerSend.SendTickSync();
        tick++;

        if(tick % 2 == 0)
        {
            ServerSend.SendPositionAndRotationDatas();
        }
    }

    public static void End()
    {
        is_game_started = false;
        host = null;
        monster_plr = null;
        Debug.Log("Game Ended");
    }
}