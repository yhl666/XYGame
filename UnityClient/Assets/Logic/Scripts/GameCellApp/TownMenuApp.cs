using UnityEngine;
using System.Collections;
using UnityEngine;
using System.Collections;
namespace Services
{

}



public class TownMenutApp : CellApp
{
    public TownMenutApp() { }
    public override bool Init()
    {


        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_MENU_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_MENU_CLOSE_CLICKED);


        return true;
    }
    public override void OnEvent(int type, object userData)
    {

        UI_townmenuapp view = userData as UI_townmenuapp;
        TownApp p = (this.parent as TownApp);
        if (type == Events.ID_TOWN_MENU_CLICKED)
        {// open

            if (p.isOneCellAppShowLock) return;
            p.isOneCellAppShowLock = true;

            p.SetNewPositionAble(false);
            view.__app__Show();
        }
        else if (type == Events.ID_TOWN_MENU_CLOSE_CLICKED)
        { // close 

            p.SetNewPositionAble(true);
            view.__app__Hide();

            p.isOneCellAppShowLock = false;

        }
    }

    public override void UpdateMS()
    {
        base.UpdateMS();
    }


    public override void OnEnter()
    {
        base.OnEnter();
    }


    public override void OnExit()
    {
        base.OnExit();
    }


    public override void OnDispose()
    {
        base.OnDispose();
    }

}
