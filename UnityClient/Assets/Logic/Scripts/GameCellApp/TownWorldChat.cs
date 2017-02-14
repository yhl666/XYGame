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















public   class TownWorldChat : CellApp
{
    public TownWorldChat() { }
    public override bool Init()
    {

        EventDispatcher.ins.AddEventListener(this, Events.ID_RPC_WORLD_CHAT_NEW_MSG);
        EventDispatcher.ins.AddEventListener(this, Events.ID_WORLDCHAT_CELL_BTN_CLICKED);

        return true;
    }
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_RPC_WORLD_CHAT_NEW_MSG)
        {//新消息 // process




        }
        else if (type == Events.ID_WORLDCHAT_CELL_BTN_CLICKED)
        {

            string msg= "name:" + PublicData.GetInstance().self_name ;
            msg = msg + ",type:[世界],msg:求老司机带我刷本，我是一直会喊6666的咸鱼,";
 
            RpcClient.ins.SendRequest("services.worldchat","push",msg);

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
