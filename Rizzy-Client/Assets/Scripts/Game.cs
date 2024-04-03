using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
public class NonLocalPlayerObject
{
    public GameObject game_object;
    public GameObject view_object;
    public NonLocalPlayer script;
}
public class Game : MonoBehaviour
{
    public static Game instance;

    #region TICKS

    private static uint _server_tick;
    public static uint server_tick
    {
        get => _server_tick;
        set
        {
            _server_tick = value;
            interpolation_tick = (uint)(value - ticks_between_position_updates);
        }
    }

    public static uint interpolation_tick { get; private set; }
    private static uint _ticks_between_position_updates = 2;
    private static uint tick_divergence_tolerance = 1;

    public static uint ticks_between_position_updates
    {
        get => _ticks_between_position_updates;
        private set
        {
            _ticks_between_position_updates = value;
            interpolation_tick = (ushort)(server_tick - value);
        }
    }
    #endregion

    public static Dictionary<ushort, NonLocalPlayerObject> other_players = new Dictionary<ushort, NonLocalPlayerObject>();
    public static List<Door> doors = new List<Door>();
    public static ushort monster_id = 0;
    public static byte monster_wait_time = 5;
    public static bool is_host = false;
    public static bool is_monster = false;

    [Header("Player Prefabs")]
    public GameObject other_plr_prefab = null;
    public GameObject local_player_prefab = null;
    public GameObject monster_model_prefab = null;
    public GameObject survivor_model_prefab = null;

    [Header("Spawn Prefabs")]
    public Transform monster_spawn = null;
    public Transform survivor_spawn = null;

    [Header("Map Items")]
    public WorldItem[] items;

    [HideInInspector] public GameObject local_player_object;
    [HideInInspector] public LocalPlayer lplr_script;
    //Each instance is inside the map
    public void Awake()
    {
        //This gets called when the map is loaded. aka game started
        instance = this;
        server_tick = 2;
        doors.Clear();

        if (Game.is_monster)
        {
            local_player_object = Instantiate(local_player_prefab, monster_spawn.position, Quaternion.identity);
            lplr_script = local_player_object.AddComponent<LocalMonster>();
        }
        else
        {
            local_player_object = Instantiate(local_player_prefab, survivor_spawn.position, Quaternion.identity);
            lplr_script = local_player_object.AddComponent<LocalSurvivor>();
        }

    }

    public void FixedUpdate()
    {
    }
    public static void OnDisconnected()
    {
        int c = SceneManager.sceneCount;
        for (int i = 0; i < c; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != "NetworkHandler")
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        foreach(var other_player in other_players)
        {
            Destroy(other_player.Value.game_object);
        }
        other_players.Clear();
    }

    public static void SetTick(uint __server_tick)
    {
        if(Mathf.Abs(server_tick - __server_tick) > tick_divergence_tolerance)
        {
            Debug.Log($"Client tick: {server_tick} -> {__server_tick}");
            server_tick = __server_tick;
        }
    }

    public static void CreatePlayer(ushort id)
    {
        NonLocalPlayerObject non_local_plr_object = new NonLocalPlayerObject();
        non_local_plr_object.game_object = Instantiate(instance.other_plr_prefab);
        non_local_plr_object.view_object = non_local_plr_object.game_object.transform.GetChild(0).gameObject;

        if (monster_id == id)
        {
            //non_local_plr_object.game_object.transform.GetChild(1).GetComponent<MeshRenderer>().material = instance.monster_material;
            Instantiate(instance.monster_model_prefab, non_local_plr_object.game_object.transform);
        }
        else
        {
            Instantiate(instance.survivor_model_prefab, non_local_plr_object.game_object.transform);
        }
        non_local_plr_object.script = non_local_plr_object.game_object.AddComponent<NonLocalPlayer>();
        non_local_plr_object.script.view_object = non_local_plr_object.view_object.transform;

        other_players.Add(id, non_local_plr_object);
    }

    public static void OnPickupItem(string item_id)
    {
        Debug.Log($"Picked up item \"{item_id}\"");
    }

    public static NonLocalPlayerObject GetNonLocalPlayer(ushort id)
    {
        return other_players.GetValueOrDefault(id);
    }
}