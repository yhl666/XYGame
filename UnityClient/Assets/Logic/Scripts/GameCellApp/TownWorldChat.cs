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


            EventDispatcher.ins.PostEvent(Events.ID_RPC_WORLD_CHAT_NEW_MSG, msg);

        }




    }


}















public sealed class TownWorldChat : CellApp
{

    public override bool Init()
    {

        EventDispatcher.ins.AddEventListener(this, Events.ID_RPC_WORLD_CHAT_NEW_MSG);

        return true;
    }
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_RPC_WORLD_CHAT_NEW_MSG)
        {//新消息 // process




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
