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
                    EventDispatcher.ins.PostEvent(Events.ID_VIEW_SYNC_FRIENDS_LIST, friends);

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
            EventDispatcher.ins.PostEvent(Events.ID_VIEW_SYNC_FRIENDS_LIST, friends);

        }
        else if (type == Events.ID_ADD_FRIEND_SUCCESS)
        {

            DAO.User who = userData as DAO.User;


            friends.Add(who);
            EventDispatcher.ins.PostEvent(Events.ID_VIEW_SYNC_FRIENDS_LIST, friends);

            EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "添加:" + who.name + "为好友成功");




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
