using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using Unity.VisualScripting;

public class MainMenuUIHandler : MonoBehaviour
{
    public static MainMenuUIHandler instance;
    public static bool loaded_network_manager = false;
    public GameObject character_prefab;
    public TMP_InputField username;
    public TMP_InputField server_location;
    public Button join_button;

    public GameObject main_gui;
    public GameObject join_gui;
    public GameObject lobby_gui;

    public TMP_Text server_name_label;
    public TMP_Text my_id_label;
    public Button start_button;

    public AudioClip microphone_clip = null;
    public AudioClip test = null;
    public AudioSource source;

    public float last_audio_data_timestamp = 0.0f;
    public int frequency = 44000;

    public static void OnConnectedToServer(string server_name)
    {
        instance.SetToLobbyScreen();
        instance.server_name_label.transform.gameObject.SetActive(true);
        instance.server_name_label.text = $"Server Name : <color=black>{server_name}</color>";
        instance.my_id_label.text = $"My Id: <color=black>{GameClient.client.Id}</color>";
    }

    private void Awake()
    {
        instance = this;
        SetToMainGUI();
        Cursor.lockState = CursorLockMode.None;

        MainMenuUIHandler.instance.start_button.gameObject.SetActive(Game.is_host);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!loaded_network_manager)
        {
            SceneManager.LoadScene("NetworkHandler", LoadSceneMode.Additive);
            loaded_network_manager = true;
        }
        microphone_clip = Microphone.Start(Microphone.devices[0], true, 1, frequency);
        source.clip = microphone_clip;

        start_button.gameObject.SetActive(false);
        start_button.onClick.AddListener(() =>
        {
            if(Game.is_host)
                ClientSend.SendStart();
        });

        Screen.fullScreen = false;
        Screen.SetResolution(1280, 720, false);
    }

    private void Update()
    {

    }
    public void EnableConnectButton()
    {
        join_button.enabled = true;
    }
    public void DisableConnectButton()
    {
        join_button.enabled = false;
    }

    public void SetToMainGUI()
    {
        main_gui.SetActive(true);
        join_gui.SetActive(false);
        lobby_gui.SetActive(false);
    }

    public void SetToJoinGUI()
    {
        main_gui.SetActive(false);
        join_gui.SetActive(true);
        lobby_gui.SetActive(false);
    }

    public void SetToLobbyScreen()
    {
        main_gui.SetActive(false);
        join_gui.SetActive(false);
        lobby_gui.SetActive(true);

        server_name_label.transform.gameObject.SetActive(false);
    }

    public void ShowStartButton()
    {

    }


    public void ConnectToServer()
    {
        if (username.text.Length < 3)
        {
            Debug.Log("Name must be atleast 3 characters long.");
            return;
        }
        DisableConnectButton();

        string ip = "127.0.0.1";
        if(server_location.text.Length > 0)
        {
            ip = server_location.text;
        }

        Debug.Log("IP " + ip);
        GameClient.Connect(ip, 8888);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
