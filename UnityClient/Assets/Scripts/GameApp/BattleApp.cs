/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
using System.Threading;

public sealed class BattleApp : AppBase
{

    private BattleKeyboardInputHandler inputHandler = null;

    BattleSyncHandler syncHandler = null;


    public override string GetAppName()
    {
        return "BattleApp";
    }
    public override bool Init()
    {
        ViewUI.Create<UIPublicRoot>();
        EventDispatcher.ins.AddEventListener(this, Events.ID_EXIT);
        EventDispatcher.ins.AddEventListener(this, Events.ID_BATTLE_PVP_RETULT);//接受战斗结束消息 来自RPC派发
        isOver = false;
        if (PublicData.ins.is_client_server == false)
        {
            Utils.SetTargetFPS(0xffffff);
        }
        syncHandler = new BattleSyncHandler();
        syncHandler.app = this;

        inputHandler = new BattleKeyboardInputHandler();


        this.worldMap = ModelMgr.Create<BattleWorldMap>() as BattleWorldMap;


        ViewUI.Create<UIBattleRoot>();

        EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_LOADING_SHOW);
            return "本地资源加载完成，初始化网络资源";
        }));

        EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
        {
            //   Thread.Sleep(1000);
            AppMgr.GetCurrentApp<BattleApp>().InitNet(false);
            return "初始化网络资源，等待服务器响应";
        }));

        //   BattleApp.ins.InitNet(false);


        ///   EventDispatcher.ins.AddEventListener(this, Events.ID_EXIT);
        return true;

    }
    public bool isStart = false;

    public void Startup()
    {

        ///  this.InitSocket();
        ///  

    }
    public override void OnEvent(string type, object userData)
    {

        /*  if (type == "DisConnect")
          {
              Debug.LogError("DisConnect");
              this.socket.Terminal();
              EventDispatcher.ins.RemoveEventListener(this, "DisConnect");

              //  this.SetGameOver();
              this.ReConnect();
          }
          else if (type == "Exit")
          {
              this.Dispose();

          }*/
    }
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_EXIT)
        {
            this.Dispose();

        }
        else if (type == Events.ID_BATTLE_PVP_RETULT)
        {
            /*  Debug.Log("BattleApp Result " + userData as string);

              this.Dispose();


              PublicData.ins.ResetPVP();
              SceneMgr.Load("TownScene");*/
        }
        else if (type == Events.ID_BATTLE_EXIT)
        {
            this.Dispose();
        }

        /*   if (type == Events.ID_NET_DISCONNECT)
           {
               Debug.LogError("DisConnect");
               this.socket.Terminal();
               EventDispatcher.ins.RemoveEventListener(this, Events.ID_NET_DISCONNECT);

               //  this.SetGameOver();
               this.ReConnect();
           }
           else if (type == Events.ID_EXIT)
           {
               this.Dispose();

           }*/
    }
    private void ReConnect()
    {
        /* this.InitNet(true);
         EventDispatcher.ins.PostEvent(Events.ID_UI_WAIT, DATA.UI_RECONNECTING);
         string info = "cmd:reconnect:" + this.current_fps + ":" + HeroMgr.ins.GetSelfHero().no;
         Debug.Log(info);
         this.socket.AddSendMsg(info);*/

    }

    private void InitWithClientServer()
    {
        HashTable kv = Json.Decode(PublicData.ins.client_server_room_info);
        string id = kv["pvproom_id"];
        PublicData.ins.pvp_room_no = id;
        //   id = "67";
        if (kv["mode"] == "pvp")
        {
            PublicData.ins.is_pve = false;
            PublicData.ins.battle_mode = "pvp";
        }
        else
        {
            PublicData.ins.is_pve = true;
            PublicData.ins.battle_mode = "pve";
        }

        System.IO.StreamReader sr = new System.IO.StreamReader(Application.dataPath + "../../../Room/room-" +
            id + ".log", System.Text.Encoding.Default);
        String line;
        while ((line = sr.ReadLine()) != null)
        {
            string str = line.ToString();
            //   Debug.Log("Read From File:      " + str);
            this.AddRecvMsg(str.Substring(0, str.Length - 1));
        }
    }
    public void InitNet(bool isReConnect = false)
    {
        if (PublicData.ins.is_client_server)
        {
            this.InitWithClientServer();
            socket = new SockClientEmpty();
            var controller = ClientServerApp.ins;
            return;
        }
        else if (PublicData.ins.is_pvp_friend_ai)
        {
            socket = new SockClientWithPVPAIMode();
        }

        else if (PublicData.GetInstance().isVideoMode)
        {
            socket = new SockClientWithVideoMode();
        }
        else
        {
            socket = new SocketClient();
        }

        if (this.socket.Startup())
        {
            //  EventDispatcher.ins.PostEvent(Events.ID_UI_WAIT, DATA.UI_WAIT_INFO_OTHERS);
            //  EventDispatcher.ins.AddEventListener(this, Events.ID_NET_DISCONNECT);

        }
        else
        {// connect server error
            // this.Dispose();

            //  SceneMgr.Load(DATA.RES_SCENE_MAIN);

        }

        if (!isReConnect)
        {
            if (PublicData.ins.is_pve)
            {
                ////  this.AddSendMsg("cmd:pve");
            }
            if (PublicData.ins.is_pvp_friend)
            {
                /*  PublicData.ins.self_user.no = 2;

                  PublicData.ins.user_pvp_other = DAO.User.Create();
                  PublicData.ins.user_pvp_other.no = 1;

                  this.socket.AddSendMsg("cmd:new_pvp:" + PublicData.ins.self_user.no.ToString() + ":" + PublicData.ins.pvp_room_no + ":" + PublicData.ins.pvp_room_max);

            */

                this.socket.AddSendMsg("cmd:new_pvp_friend:" + PublicData.ins.self_user.no.ToString() + ":" + PublicData.ins.pvp_room_no + ":" + PublicData.ins.pvp_room_max);
            }
            else if (PublicData.ins.is_pvp_friend_ai)
            {
                this.socket.AddSendMsg("cmd:new:" + PublicData.GetInstance().player_name);
            }
            else
            {
                this.socket.AddSendMsg("cmd:new_pvp_friend:1" + ":1" + ":1");
            }
        }
    }

    public override void Update()
    {
        if (PublicData.ins.is_client_server)
        {
            ClientServerApp.ins.UpdateMS();
        }

        this.Process();
        AutoReleasePool.ins.Clear(); // 没帧数结束 清理一次， 逻辑帧 Process内部已处理
    }

    public void CalculateBattleResult()
    {//计算战斗结果
        EnemyBoss boss = EnemyMgr.ins.GetEnemy<EnemyBoss>();
        if (boss != null && boss.current_hp <= 0)
        {//boss 死亡
            PublicData.ins.battle_result = BattleResult.Win;
        }
        else
        {
            PublicData.ins.battle_result = BattleResult.Lose;
        }
    }
    private void ProcessWithGameOver()
    {
        if (this.socket != null)
        {
            //先关闭战斗服的连接
            this.socket.Terminal();
            this.socket = null;
        }
        if (this.isVideoMode())
        {
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_PVP_WAITFOR_RESULT_SHOW);

            string str = "ret:ok,msg:播放战斗录像结束!,";
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_PVP_RETULT, str);
            return;
        }

        if (PublicData.ins.is_client_server)
        {
            //写入结果

            HashTable kv = Json.Decode(PublicData.ins.client_server_room_info);

            Hero p1 = HeroMgr.ins.GetHero(int.Parse(kv["p1"]));
            Hero p2 = HeroMgr.ins.GetHero(int.Parse(kv["p2"]));

            PublicData.ins.client_server_room_info += "h1:" + p1.current_hp + ",h2:" + p2.current_hp + ",";
            PublicData.ins.client_server_result = PublicData.ins.client_server_room_info;
            Debug.Log("Over:" + PublicData.ins.client_server_result);
            PublicData.ins.ResetPVP();//服务器模式直接设置，其他要在跳转场景设置
            this.Dispose();//服务器模式直接跳转

            Debug.Log(p1.team);
            Debug.Log(p2.team);


        }
        if (PublicData.ins.is_pvp_friend)
        {
            string upload = "pvproom_id:" + PublicData.ins.pvp_room_no + ",p1:";

            if (PublicData.ins.is_pvp_friend_owner)
            {
                //我是发起者
                upload += HeroMgr.ins.self.no.ToString();
                upload += ",p2:" + PublicData.ins.user_pvp_other.no + ",";
                upload += "mode:" + PublicData.ins.battle_mode + ",";
                upload += "h1:" + HeroMgr.ins.self.current_hp.ToString();
                upload += ",h2:" + HeroMgr.ins.GetHero(PublicData.ins.user_pvp_other.no).current_hp + ",";
                Thread.Sleep(100);

            }
            else
            {
                upload += PublicData.ins.user_pvp_other.no.ToString(); ;
                upload += ",p2:" + HeroMgr.ins.self.no.ToString() + ",";
                upload += "mode:" + PublicData.ins.battle_mode + ",";
                upload += "h1:" + HeroMgr.ins.GetHero(PublicData.ins.user_pvp_other.no).current_hp;
                upload += ",h2:" + HeroMgr.ins.self.current_hp.ToString() + ",";

            }

            ///发起验证
            RpcClient.ins.SendRequest("services.battle", "request_verify", upload, (string msg) =>
                {

                    Debug.LogError("Verify: " + msg);
                });

            //显示等待结果界面
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_PVP_WAITFOR_RESULT_SHOW);

        }
        else
        {
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_PVP_WAITFOR_RESULT_SHOW);
        }

    }

    public void SetGameOver()
    {
        Debug.LogError("Over");
        this.ProcessWithGameOver();
        /*  if (this.socket != null)
          {
              EventDispatcher.ins.RemoveEventListener(this, "DisConnect");

              this.socket.Terminal();
              this.socket = null;
          }*/
        isOver = true;
    }

    public void SetPlayerModeSpeed(int sp)
    {
        if (this.isVideoMode() == true)
        {
            (this.socket as SockClientWithVideoMode).SetPlayeSpeed(sp);
        }
    }
    public void Process()
    {
        inputHandler.Update();
        ModelMgr.ins.Update();
        ViewMgr.ins.Update();

        do
        { // do while 流程
            if (isOver)
            {
                ///  this.ProcessWithGameOver();
                break;
            }

            // process to the lastest frame (if current frame less than max frame(from server )  this will block until current frame
            syncHandler.UpdateMS();

        } while (false);

    }

    public void LockAdRecvMsg()
    {
        syncHandler._recvQueue.Lock();
    }
    public void UnLockAdRecvMsg()
    {
        syncHandler._recvQueue.UnLock();
    }

    public void AddRecvMsg(string msg)
    {
        syncHandler.AddRecvMsg(msg);
    }

    public void AddRecvMsgUnSafe(string msg)
    {
        syncHandler.AddRecvMsgUnSafe(msg);
    }

    public void AddSendMsg(string msg)
    {
        if (this.socket != null)
        {
            this.socket.AddSendMsg(msg);
        }
    }
    public void Send(string msg)
    {
        if (this.isVideoMode() == false)
        {
            socket.AddSendMsg(msg);
        }
    }

    /*
     * mode =0 is normal;
     * mode =1 is video mode
     */
    public int mode = 0;

    public void SetVideoMode()
    {
        this.mode = 1;

    }
    public bool isVideoMode() { return PublicData.GetInstance().isVideoMode; }


    public int GetCurrentFrame() { return syncHandler.GetCurrentFrame(); }


    public bool isOver = false;

    private SocketClient socket;

    public System.Random randObj;

    public override void OnDispose()
    {
        if (PublicData.ins.is_client_server == false)
        {
            Utils.SetTargetFPS(0xffffff);
        }
        PublicData.ins.is_pvp_friend_ai = false;
        PublicData.ins.is_pvp_friend = false;
        ///   PublicData.ins.isVideoMode = false;

        ViewMgr.DestroyInstance();
        ModelMgr.DestroyInstance();
        EventDispatcher.DestroyInstance();
        AutoReleasePool.DestroyInstance();
        AudioMgr.DestroyInstance();
        if (this.socket != null)
        {
            //   EventDispatcher.ins.RemoveEventListener(this, Event.ID_NET_DISCONNECT);

            this.socket.Terminal();
            this.socket = null;
        }

        if (EventSystem.ins != null)
        {
            //  EventSystem.ins.RemoveEvent_Update(this);
        }

        base.OnExit();

        if (PublicData.ins.is_client_server)
        {
            SceneMgr.Load("ClientServerScene");

        }
    }
}