using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class UIHandler : MonoBehaviour
{
    public Button button;
    private TMP_Text text = null;
    public bool start = true;

    public void Awake()
    {
        text = button.transform.GetChild(0).GetComponent<TMP_Text>();
        text.text = "Start Server";
    }
    public void Start()
    {
#if !UNITY_EDITOR
        GameServer.Start(20, 8888);
#endif
        Screen.fullScreen = false;
        Screen.SetResolution(1280, 720, false);
    }

    public void FixedUpdate()
    {
        GameServer.Tick();
    }
    public void OnButtonPressed()
    {
        if (start)
        {
            GameServer.Start(20, 8888);
            text.text = "Stop Server";
        }
        else
        {
            GameServer.Stop();
            text.text = "Start Server";
        }
        start = !start;
    }
}