using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using UnityEngine;

public class SettingsApp : CellApp
{
    public override bool Init()
    {
        EventDispatcher.ins.AddEventListener(this,Events.ID_SETTING_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_SETTING_CLOSE_CLICKED);
 
        return true;
    }

    public override void OnEvent(int type, object userData)
    {
        TownApp p = (this.parent as TownApp);
        UI_settingapp view=userData as UI_settingapp;
        
 
        if (type==Events.ID_SETTING_CLOSE_CLICKED)
        {
            p.SetNewPositionAble(true);
            view.Hide();

            p.isOneCellAppShowLock = false;

        }
        if (type==Events.ID_SETTING_CLICKED)
        {
            if (p.isOneCellAppShowLock) return;
            p.isOneCellAppShowLock = true;
            Debug.Log("show or hide setting");
            p.SetNewPositionAble(false);
            view.Show();
        }

    }
}

