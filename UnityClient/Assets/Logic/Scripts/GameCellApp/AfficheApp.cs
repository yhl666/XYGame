using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class AfficheApp:CellApp
{
    public override bool Init()
    {
        EventDispatcher.ins.AddEventListener(this, Events.ID_AFFICHE_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_AFFICHE_CLOSE_CLICKED);

        return true;
    }

    public override void OnEvent(int type, object userData)
    {
        TownApp p = (this.parent as TownApp);
        UI_afficheapp view = userData as UI_afficheapp;

        if (type == Events.ID_AFFICHE_CLOSE_CLICKED)
        {
            p.SetNewPositionAble(true);
            view.Hide();

            p.isOneCellAppShowLock = false;
        
        }
        if (type == Events.ID_AFFICHE_CLICKED)
        {
            if (p.isOneCellAppShowLock) return;
            p.isOneCellAppShowLock = true;

            p.SetNewPositionAble(false);
            view.Show();
        }

    }
}

