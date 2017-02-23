/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


namespace Services
{
    public class TownBattlePVP : RpcService
    {

        public void PushResult(string msg, VoidFuncString cb)
        {
            cb("ret:ok,");

            EventDispatcher.ins.PostEvent(Events.ID_TOWN_PVP_QUEUE_RESULT, msg);
        }

    }



}




public class TownPVPApp : CellApp
{
    public TownPVPApp() { }
    public override bool Init()
    {

        //1v1 随机匹配
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_BTN_PVP_ENTER_QUEUE_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_BTN_PVP_LEAVE_QUEUE_CLICKED);

        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_PVP_QUEUE_RESULT);

        return true;
    }
    public override void OnEvent(int type, object userData)
    {

        UI_townpvpapp view = userData as UI_townpvpapp;
        TownApp p = (this.parent as TownApp);
        if (type == Events.ID_TOWN_BTN_PVP_ENTER_QUEUE_CLICKED)
        {// open
            //允许移动 但不允许点击其他APP
            if (p.isOneCellAppShowLock) return;
            p.isOneCellAppShowLock = true;


            RpcClient.ins.SendRequest("services.battle_pvp", "request_pvp_ramdon_enter_queue_v2",
                "no:" + PublicData.ins.self_user.no.ToString() + ",", (string msg) =>
                {
                    if (msg == "")
                    {
                        p.isOneCellAppShowLock = false;//防止重复点击

                        EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "进入队列服务器响应失败,请重试");
                        return;
                    }
                    HashTable kv = Json.Decode(msg);

                    if (kv["ret"] == "ok")
                    {

                        //  p.SetNewPositionAble(false);
                        view.Show();//进入成功后 才显示UI
                        EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "进入队列成功!匹配到对手后会自动进入战斗");
                    }
                    else
                    {
                        p.isOneCellAppShowLock = false;

                        EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "进入队列失败!" + kv["msg"]);

                    }

                });



        }
        else if (type == Events.ID_TOWN_BTN_PVP_LEAVE_QUEUE_CLICKED)
        { // close 

            RpcClient.ins.SendRequest("services.battle_pvp", "request_pvp_ramdon_leave_queue_v2",
           "no:" + PublicData.ins.self_user.no.ToString() + ",", (string msg) =>
           {

               if (msg == "")
               {
                   EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "退出队列服务器响应失败,请重试");
                   return;
               }
               HashTable kv = Json.Decode(msg);

               if (kv["ret"] == "ok")
               {
                   p.isOneCellAppShowLock = false;

                   p.SetNewPositionAble(true);
                   view.Hide();//进入成功后 才显示UI
                   EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "退出队列成功!");
               }
               else
               {

                   EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "退出队列失败!" + kv["msg"]);
               }

           });





        }
        else if (type == Events.ID_TOWN_PVP_QUEUE_RESULT)
        {
            //匹配结果
            string str = userData as string;
            Debug.Log("PVP QUEUE " + str);
            HashTable kv = Json.Decode(str);


            if (kv["p2"] == HeroMgr.ins.self.no.ToString())
            {//第二个发起匹配的为 
                //暂时用 好友对战一套
                PublicData.ins.is_pvp_friend_owner = false;
            }
            //接受成功 自动连接战斗服
            string room_id = kv["pvproom_id"];
            string max = kv["max_no"];

            PublicData.ins.pvp_room_max = max;
            PublicData.ins.pvp_room_no = room_id;
            PublicData.ins.user_pvp_other = DAO.User.Create(kv);

            PublicData.ins.is_pvp_friend = true;
            AppMgr.GetCurrentApp<TownApp>().Dispose();

            //    AutoReleasePool.ins.Clear();
            SceneMgr.Load("BattlePVP");


















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
