using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class UI_townlogoutapp : ViewUI
{
    public override bool Init()
    {
        logoutButton = GameObject.Find("btn_bluetooth").GetComponent<Button>();
        logoutButton.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_BTN_LOGOUT);
            Debug.Log("logout");
        });
        return true;
    }

    private Button logoutButton = null;
}

