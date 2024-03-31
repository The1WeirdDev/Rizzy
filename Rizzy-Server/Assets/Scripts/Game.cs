using Riptide;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public enum ClientMessages : ushort
{
    Start = 0,
    SetPositionAndRot = 1,
    SendRequestKill,
    SetCrouchingMode
}
public enum ServerMessages : ushort
{
    SyncTick=0,
    SetLobbyData,
    SetHost,
    StartGame,
    PlayerLeft,
    SetPlayersPositionAndRotationData,
    PlayerDied
}
public class Game
{
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
            p.Value.is_monster = false;
            p.Value.is_alive = true;
            p.Value.SetPosition(Vector3.zero);
        }
        alive_players = Player.players.Count;
        monster_plr = Player.players.ElementAt(Random.Range(1, Player.players.Count) - 1).Value;
        is_game_started = true;
        time_started = Time.time;
        last_kill_time = Time.time;

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