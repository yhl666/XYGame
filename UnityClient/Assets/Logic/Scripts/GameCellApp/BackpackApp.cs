using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public sealed class BackpackApp:CellApp
{
    public override bool Init()
    {
        EventDispatcher.ins.AddEventListener(this, Events.ID_BACKPACK_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_BACKPACK_CLOSE_CLICKED);
        EventDispatcher.ins.AddEventListener(this,Events.ID_INFO_BACKPACK_SHOW);
        EventDispatcher.ins.AddEventListener(this,Events.ID_INFO_BACKPACK_UNSHOW);

        return true;
    }

    public override void OnEvent(int type, object userData)
    {
        if ((type == Events.ID_BACKPACK_CLOSE_CLICKED) || (type == Events.ID_BACKPACK_CLICKED) )
        {
            TownApp p = (this.parent as TownApp);
            UI_backpackapp view = userData as UI_backpackapp;
            
            Debug.Log("show or hide");
            if (type == Events.ID_BACKPACK_CLOSE_CLICKED)
            {
                p.SetNewPositionAble(true);
                view.Hide();

                p.isOneCellAppShowLock = false;
            }
            if (type == Events.ID_BACKPACK_CLICKED)
            {
                if (p.isOneCellAppShowLock) return;
                p.isOneCellAppShowLock = true;

                p.SetNewPositionAble(false);
                view.Show();
            }
        }
        if (type == Events.ID_INFO_BACKPACK_SHOW || type == Events.ID_INFO_BACKPACK_UNSHOW)
        {
            GameObject showdetail= GameObject.Find("backpack_panel_detail");
            if (type == Events.ID_INFO_BACKPACK_SHOW)
            {
                showdetail.SetActive(true);
            }
            if (type == Events.ID_INFO_BACKPACK_UNSHOW)
            {
                showdetail.SetActive(false);
            }
        }
    }
}

