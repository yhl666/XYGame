using UnityEngine;
using System.Collections;

namespace Services
{
    public class Friends : RpcService
    {
        public void Push(string msg, VoidFuncString cb)
        {
            cb("ret:ok,");
            var hash = Json.Decode(msg);

            string type = hash["type"];

            EventDispatcher.ins.PostEvent(Events.ID_RPC_WORLD_CHAT_NEW_MSG, hash);
        }
    }

}



public class FriendsApp : CellApp
{
    public FriendsApp() { }
    public override bool Init()
    {


        EventDispatcher.ins.AddEventListener(this, Events.ID_FRIENDS_ADD_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_FRIENDS_CLOSE_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_FRIENDS_SHOW_CLICKED);

        return true;
    }
    public override void OnEvent(int type, object userData)
    {

        UI_friendsapp view = userData as UI_friendsapp;
        TownApp p = (this.parent as TownApp);


        if (type == Events.ID_FRIENDS_SHOW_CLICKED)
        { 
            if (p.isOneCellAppShowLock) return;
            p.isOneCellAppShowLock = true;

            p.SetNewPositionAble(false);
            view.Show();
        }

        else if (type == Events.ID_FRIENDS_CLOSE_CLICKED)
        {  // 点击关闭

            p.SetNewPositionAble(true);
            view.Hide();

            p.isOneCellAppShowLock = false;

        }

        else if (type == Events.ID_FRIENDS_ADD_CLICKED)
        {

            // 点击 添加好友

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
