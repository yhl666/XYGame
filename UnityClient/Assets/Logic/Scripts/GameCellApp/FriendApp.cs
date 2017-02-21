using UnityEngine;
using System.Collections;

namespace Services
{
    public class Friends : RpcService
    {
        public void Add(string msg, VoidFuncString cb)
        {
            cb("ret:ok,");
            DAO.User who = DAO.User.Create(msg);

            EventDispatcher.ins.PostEvent(Events.ID_RPC_NEW_FRIEND, who);
        }

        public void RecvPVP(string msg, VoidFuncString cb)
        {
            DAO.User who = DAO.User.Create(msg);
            HashTable kv = Json.Decode(msg);

            GlobalDialogInfo info = new GlobalDialogInfo();
            info.txt_info = "玩家:" + who.name + " 邀请你PK,接受吗?";
            info.txt_title = "好友邀请";
            info._OnYes = () =>
            {
                cb("ret:ok,");
                PublicData.ins.is_pvp_friend_owner = false;

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


            };

            info._OnNo = () =>
            {
                cb("ret:error,");

            };




            EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_GLOBALDIALOG_SHOW, info);
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

        EventDispatcher.ins.AddEventListener(this, Events.ID_RPC_NEW_FRIEND);
        EventDispatcher.ins.AddEventListener(this, Events.ID_ADD_FRIEND_SUCCESS);
        EventDispatcher.ins.AddEventListener(this, Events.ID_FRIENDS_DELETE_CLICKED);

        EventDispatcher.ins.AddEventListener(this, Events.ID_FRIENDS_PVP_CLICKED);
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

            RpcClient.ins.SendRequest("services.friends", "query_all", "no:" + PublicData.ins.self_user.no + ",", (string msg) =>
                {
                    Debug.Log("friend list " + msg);
                    if (msg == "") return;

                    ArrayList list = Json.MultiDecode(msg);
                    friends.Clear();
                    foreach (HashTable kv in list)
                    {

                        friends.Add(DAO.User.Create(kv));


                    }
                    EventDispatcher.ins.PostEvent(Events.ID_FRIEND_SYNC_VIEW, friends);

                });







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


        else if (type == Events.ID_RPC_NEW_FRIEND)
        { // 有人添加你为好友

            DAO.User who = userData as DAO.User;

            EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "玩家:" + who.name + " 已添加你为好友!");

            friends.Add(who);
            EventDispatcher.ins.PostEvent(Events.ID_FRIEND_SYNC_VIEW, friends);

        }
        else if (type == Events.ID_ADD_FRIEND_SUCCESS)
        {

            DAO.User who = userData as DAO.User;


            friends.Add(who);
            EventDispatcher.ins.PostEvent(Events.ID_FRIEND_SYNC_VIEW, friends);

            EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "添加:" + who.name + "为好友成功");




        }
        else if (Events.ID_FRIENDS_DELETE_CLICKED == type)
        {

            RpcClient.ins.SendRequest("services.friends", "delete_by_no", "no:" + PublicData.ins.self_user.no + ",who:" + view.current_detail_user.no + ",", (string msg) =>
            {
                HashTable kv = Json.Decode(msg);
                if (kv["ret"] == "ok")
                {

                    friends.Remove(view.current_detail_user);

                    EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "删除好友:" + view.current_detail_user.name + "成功");
                    EventDispatcher.ins.PostEvent(Events.ID_FRIEND_SYNC_VIEW, friends);
                }
                else
                {

                    EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "删除好友:" + view.current_detail_user.name + "失败 " + kv["msg"]);
                }
            });
        }
        else if (type == Events.ID_FRIENDS_PVP_CLICKED)
        {
            //切磋
            DAO.User other = userData as DAO.User;

            RpcClient.ins.SendRequest("services.battle_pvp", "request_pvp_v2", "no:1,no_target:" + other.no.ToString() + ",", (string msg) =>
             {
                 /*if (msg == "")
                 {

                     GlobalDialogInfo info = new GlobalDialogInfo();
                     info.txt_title = "好友PVP";
                     info.txt_info = "玩家拒绝了你的邀请,是否进入离线模式PVP";

                     info._OnYes = () =>
                     {
                         PublicData.ins.is_pvp_friend_ai = true;
                         PublicData.ins.user_pvp_other = other;

                         this.parent.Dispose();

                         SceneMgr.Load("BattlePVP");

                         Debug.Log("拒绝你的邀请");
                     };


                     EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_GLOBALDIALOG_SHOW, info);
                     return;
                 }*/

                 HashTable kv = Json.Decode(msg);
                 if (kv["ret"] == "ok")
                 {
                     string room_id = kv["pvproom_id"];
                     string max = kv["max_no"];

                     PublicData.ins.pvp_room_max = max;
                     PublicData.ins.pvp_room_no = room_id;
                     PublicData.ins.user_pvp_other = DAO.User.Create(kv);
                   

                     PublicData.ins.is_pvp_friend = true;
                     this.parent.Dispose();
                     //    AutoReleasePool.ins.Clear();
                     SceneMgr.Load("BattlePVP");
                 }
                 else if (kv["ret"] == "error")
                 { //拒绝  启动离线模式


                     GlobalDialogInfo info = new GlobalDialogInfo();
                     info.txt_title = "好友PVP";
                     info.txt_info = "玩家拒绝了你的邀请("  + kv["msg"] +"),是否进入离线模式PVP";

                     info._OnYes = () =>
                     {
                         PublicData.ins.is_pvp_friend_ai = true;
                         PublicData.ins.user_pvp_other = other;

                         this.parent.Dispose();

                         SceneMgr.Load("BattlePVP");

                         Debug.Log("拒绝你的邀请");
                     };


                     EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_GLOBALDIALOG_SHOW, info);

                 }

                 /*   PublicData.ins.is_pvp_friend_ai = true;

                    var other1 = DAO.User.Create(); 
                    var self = DAO.User.Create();
                    self.no = 1;
                    other.no = 2;
                    PublicData.ins.user_pvp_other = other1;
                    PublicData.ins.self_user = self;
                    this.parent.Dispose();

                    SceneMgr.Load("BattlePVP");

                    Debug.Log("拒绝你的邀请");*/



             });
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
