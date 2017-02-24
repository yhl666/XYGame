using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public sealed class TrumpLevelApp:CellApp
{
    public override bool Init()
    {


        EventDispatcher.ins.AddEventListener(this, Events.ID_TRUMP_LEVELUP_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_TRUMP_LEVELUP_CLOSE_CLICKED);

        return true;
    }
    public override void OnEvent(int type, object userData)
    {

        UI_trump_levelupapp view = userData as UI_trump_levelupapp;

        TownApp p = (this.parent as TownApp);
        if (type == Events.ID_TRUMP_LEVELUP_CLICKED)
        {// open

            if (p.isOneCellAppShowLock) return;
            p.isOneCellAppShowLock = true;

            p.SetNewPositionAble(false);
            view.Show();
        }
        else if (type == Events.ID_TRUMP_LEVELUP_CLOSE_CLICKED)
        { // close 


            Debug.Log("hide trumplevel");
            p.SetNewPositionAble(true);
            view.Hide();

            p.isOneCellAppShowLock = false;

        }
    }

}

