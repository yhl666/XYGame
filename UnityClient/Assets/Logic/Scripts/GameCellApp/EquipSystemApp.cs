/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

namespace Services
{
    public class EquipSystemApp : RpcService
    {
        public void Add(string msg, VoidFuncString cb)
        {
            cb("ret:ok,");
            DAO.User who = DAO.User.Create(msg);

            EventDispatcher.ins.PostEvent(Events.ID_RPC_NEW_FRIEND, who);
        }

     
    }

}



public class EquipSystemApp : CellApp
{
    public EquipSystemApp() { }
    public override bool Init()
    {

        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_EQUIPSYSTEM_BTN_CLOSE_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_EQUIPSYSTEM_BTN_SHOW_CLICKED);
 
        return true;
    }
    public override void OnEvent(int type, object userData)
    {

        UI_equipsystemapp view = userData as UI_equipsystemapp;
        TownApp p = (this.parent as TownApp);


        if (type == Events.ID_TOWN_EQUIPSYSTEM_BTN_SHOW_CLICKED)
        {
            if (p.isOneCellAppShowLock) return;
            p.isOneCellAppShowLock = true;

            p.SetNewPositionAble(false);
            view.Show();

         /*   RpcClient.ins.SendRequest("services.friends", "query_all", "no:" + PublicData.ins.self_user.no + ",", (string msg) =>
                {
                    Debug.Log(  msg);
                

                });
            */


        }

        else if (type == Events.ID_TOWN_EQUIPSYSTEM_BTN_CLOSE_CLICKED)
        {  // 点击关闭

            p.SetNewPositionAble(true);
            view.Hide();

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





    private ArrayList friends = new ArrayList();

}
