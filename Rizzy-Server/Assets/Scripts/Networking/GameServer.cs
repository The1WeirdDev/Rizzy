using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using UnityEngine;

using Riptide;
using Riptide.Utils;

using static Riptide.Server;
using Riptide.Transports;

class GameServer {
    public static Server server;
    public static ushort port;
    public static ushort max_players;

    public static string server_name = "Server";

    public static void Start(ushort max_players, ushort port)
    {
        GameServer.port = port;
        GameServer.max_players = max_players;
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        server = new Server();
        server.Start(port, max_players);

        server.ClientConnected += ServerHandle.OnClientConnnected;
        server.ClientDisconnected += ServerHandle.OnClientDisconnected;
        server.ConnectionFailed += ServerHandle.OnConnectionFailed;
        Game.Init();
    }

    public static void Stop()
    {
        if(server != null)
        {
            server.Stop();
        }
    }

    public static void Tick()
    {
        if (server == null) return;

        server.Update();
        Game.Tick();
    }
}