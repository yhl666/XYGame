using UnityEngine;
using System.Collections;
 
namespace Services
{
    public class WorldChat : RpcService
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



public class WorldChatApp : CellApp
{
    public WorldChatApp() { }
    public override bool Init()
    {

      ///  EventDispatcher.ins.AddEventListener(this, Events.ID_RPC_WORLD_CHAT_NEW_MSG);
        EventDispatcher.ins.AddEventListener(this, Events.ID_WORLDCHAT_CELL_BTN_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_WORLDCHAT_SEND_BTN_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_WORLDCHAT_CLOSE_BTN_CLICKED);


        return true;
    }
    public override void OnEvent(int type, object userData)
    {

        UI_worldchatapp view = userData as UI_worldchatapp;
        TownApp p = (this.parent as TownApp);

        if (type == Events.ID_RPC_WORLD_CHAT_NEW_MSG)
        {//新消息 // process



        }
        else if (type == Events.ID_WORLDCHAT_CELL_BTN_CLICKED)
        {

            if (p.isOneCellAppShowLock) return;
            p.isOneCellAppShowLock = true;

            p.SetNewPositionAble(false);
            view.Show();
        }
        else if (Events.ID_WORLDCHAT_SEND_BTN_CLICKED == type)
        {
            string what = userData as string;

            string msg = "name:" + PublicData.GetInstance().self_name;
            msg = msg + ",type:[世界],msg:" + what + ",name:" + PublicData.GetInstance().self_name + ",";

            RpcClient.ins.SendRequest("services.worldchat", "push", msg);


        }
        else if (type == Events.ID_WORLDCHAT_CLOSE_BTN_CLICKED)
        {

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

}
