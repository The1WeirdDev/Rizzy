using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class InGameGUIHandler : MonoBehaviour {
    public static InGameGUIHandler instance;
    public GameObject canvas_prefab;
    [HideInInspector]public GameObject canvas = null;

    private GameObject countdown_gui;
    private GameObject death_gui;
    private GameObject inventory_gui;
    private TMP_Text countdown_gui_text;
    private TMP_Text time_remaining_text;
    private TMP_Text current_countdown_text = null;
    private Image item_image = null;
    private float time_remaining = 0;
    private float last_time_check = 0;

    public void Awake()
    {
        instance = this;
        canvas = Instantiate(canvas_prefab, transform);

        countdown_gui = canvas.transform.GetChild(0).gameObject;
        countdown_gui_text = countdown_gui.transform.GetChild(1).GetComponent<TMP_Text>();

        time_remaining_text = canvas.transform.GetChild(1).GetComponent<TMP_Text>();

        death_gui = canvas.transform.GetChild(2).gameObject;
        inventory_gui = canvas.transform.GetChild(4).gameObject;
        item_image = inventory_gui.transform.GetChild(1).GetComponent<Image>();

        DisableCountdownGUI();
        DisableTimeRemainingGUI();
        DisableDeathGUI();
        EnableInventoryGUI();
    }

    public void SetImage(Sprite sprite)
    {
        item_image.sprite = sprite;
    }
    public void EnableInventoryGUI()
    {
        inventory_gui.SetActive(true);
    }
    public void HideInventoryGUI()
    {
        inventory_gui.SetActive(false);
    }
    public void EnableCountdownGUI()
    {
        countdown_gui.SetActive(true);
        time_remaining = Game.monster_wait_time;
        countdown_gui_text.text = "Time Remaining : " + time_remaining;
        current_countdown_text = countdown_gui_text;
    }
    public void DisableCountdownGUI()
    {
        countdown_gui.SetActive(false);
    }
    public void EnableTimeRemainingGUI()
    {
        time_remaining_text.gameObject.SetActive(true);
        time_remaining = Game.monster_wait_time;
        time_remaining_text.text = $"<color=red>Time Remaining : {time_remaining}</color>";
        current_countdown_text = time_remaining_text;
    }
    public void DisableTimeRemainingGUI()
    {
        time_remaining_text.gameObject.SetActive(false);
    }

    public void ShowDeathGUI()
    {
        HideInventoryGUI();
        DisableCountdownGUI();
        DisableTimeRemainingGUI();
        death_gui.SetActive(true);
    }
    public void DisableDeathGUI()
    {
        death_gui.SetActive(false);
    }

    public void Update()
    {
        if (time_remaining < 0) return;
        if(Time.time - last_time_check > 1.0f)
        {
            last_time_check = Time.time;
            current_countdown_text.text = "Time Remaining : " + time_remaining;
            time_remaining--;
        }
    }
}