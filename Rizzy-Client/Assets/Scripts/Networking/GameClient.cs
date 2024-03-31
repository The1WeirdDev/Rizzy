using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Riptide;
using Riptide.Utils;
using System;

public class GameClient : MonoBehaviour
{
    public static Client client;

    public static void Connect(string ip, ushort port)
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        client = new Client();
        client.Connected += GameClient.OnConnected;
        client.ConnectionFailed += GameClient.OnConnectionFailed;
        client.Disconnected += GameClient.OnDisconnected;
        string joined_ip = $"{ip}:{port}";
        Debug.Log($"JOINED IP IS \"{joined_ip}\"");
        client.Connect(joined_ip, 5);
    }

    public static void Tick()
    {
        if (client == null) return;
        client.Update();
        Game.server_tick++;
    }

    public static void OnConnected(object o, EventArgs e)
    {
        Debug.Log($"Connected to server With id of {client.Id}");
    }
    
    public static void OnConnectionFailed(object o, EventArgs e)
    {
        MainMenuUIHandler.instance.EnableConnectButton();
        Debug.Log("Failed to Connect to server.");
    }
    public static void OnDisconnected(object o, EventArgs e)
    {
        Debug.Log("Disconnected from server.");
        Game.OnDisconnected();
    }
}
