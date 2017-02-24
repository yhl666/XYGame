using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class CharacterInfoApp : CellApp
{
    public override bool Init()
    {
        EventDispatcher.ins.AddEventListener(this, Events.ID_CHARACTER_INFO_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_CHARACTER_INFO_CLOSE_CLICKED);
        return true;
    }

    public override void OnEvent(int type, object userData)
    {
        TownApp p = (this.parent as TownApp);
        UI_characterinfoapp view = userData as UI_characterinfoapp;

        if (type == Events.ID_CHARACTER_INFO_CLOSE_CLICKED)
        {
            p.SetNewPositionAble(true);
            view.Hide();

            p.isOneCellAppShowLock = false;

        }
        if (type == Events.ID_CHARACTER_INFO_CLICKED)
        {
            if (p.isOneCellAppShowLock) return;
            p.isOneCellAppShowLock = true;

            p.SetNewPositionAble(false);
            view.Show();
        }

    }

}

