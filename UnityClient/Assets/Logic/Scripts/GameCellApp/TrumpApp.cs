using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using UnityEngine;


class TrumpApp:CellApp
    {
    public TrumpApp() { }
    public override bool Init()
    {


        EventDispatcher.ins.AddEventListener(this, Events.ID_TRUMP_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_TRUMP_CLOSE_CLICKED);

        return true;
    }
    public override void OnEvent(int type, object userData)
    {

        UI_trumpapp view = userData as UI_trumpapp;
        TownApp p = (this.parent as TownApp);
        if (type == Events.ID_TRUMP_CLICKED)
        {// open

            if (p.isOneCellAppShowLock) return;
            p.isOneCellAppShowLock = true;

            p.SetNewPositionAble(false);
            view.Show();
        }
        else if (type == Events.ID_TRUMP_CLOSE_CLICKED)
        { // close 


            Debug.Log("hide trump");
            p.SetNewPositionAble(true);
            view.Hide();

            p.isOneCellAppShowLock = false;

        }
    }

}

