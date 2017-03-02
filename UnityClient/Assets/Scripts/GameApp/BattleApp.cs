/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
using System.Threading;

namespace Services
{
    public class BattlePVP : RpcService
    {

        public void PushResult(string msg, VoidFuncString cb)
        {
            cb("ret:ok,");

            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_PVP_RETULT, msg);
        }

    }



}


/// <summary>
/// 处理玩家键盘 输入信息
/// </summary>
sealed class BattleKeyboardInputHandler
{

    public void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            PublicData.ins.IS_left = true;

        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            PublicData.ins.IS_stand = true;
            PublicData.ins.IS_left = false; //fix
        }
        if (Input.GetKey(KeyCode.D))
        {
            PublicData.ins.IS_right = true;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            PublicData.ins.IS_stand = true;
            PublicData.ins.IS_right = false;// fix 
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

            PublicData.ins.IS_atk = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            PublicData.ins.IS_jump = true;

        }
    }
}



/// <summary>
/// 帧同步相关处理
/// </summary>
sealed class BattleSyncHandler
{
    public BattleSyncHandler()
    {
        for (int i = 0; i < Config.MAX_FRAME_COUNT; i++)
        {
            this.framedatas.Add(null);
        }
    }

    public void AddRecvMsg(string msg)
    {
        _recvQueue.Enqueue(msg);
        ///  Debug.Log("[LOG]:Recv:  " + msg);
    }

    public void AddRecvMsgUnSafe(string msg)
    {
        _recvQueue.UnSafeEnqueue(msg);
        //  Debug.Log("[LOG]:Recv:  " + msg);
    }



    private int current_max_fps = 0;
    public int GetCurrentFrame() { return this.current_fps; }
    public ThreadSafeQueue _recvQueue = new ThreadSafeQueue();

    ArrayList framedatas = new ArrayList();

    private int current_fps = 0;

    int on_enter_max_fps = 0;//加入游戏时最大帧数
    public BattleApp app = null;
    private int _FRAMEDATA_EMPTY_ERROR_COUNTER = 0;

    public void UpdateMS()
    {


        if (on_enter_max_fps > current_fps && app.isStart == true)
        {
            EventDispatcher.ins.PostEvent(Events.ID_UI_WAIT, DATA.UI_WAIT_ENTER + "   " + current_fps * 100 / on_enter_max_fps + " %");
            app.isStart = true;
        }
        else if (app.isStart == true && on_enter_max_fps < current_fps)
        {
            app.isStart = false;
            EventDispatcher.ins.PostEvent(Events.ID_UI_NOWAIT);
        }
        else if (on_enter_max_fps < current_fps && app.isStart == false)
        {
            app.isStart = false;
        }




        bool _need_send = false;
        //  _recvQueue.Lock();
        //处理帧
        int i = 0;
        while (_recvQueue.Empty() == false)
        {
            i++;
            ///  Thread.Sleep(40);

            if (i > 2) break;//连续计算80帧强制刷新UI
            _need_send = true;
            //     if (_recvQueue.Count() % 50 == 0) return;
            string xx = _recvQueue.Dequeue() as string;

            TranslateDataPack decode = TranslateDataPack.Decode(xx);
            //    Debug.Log("Recv " + xx);
            if (decode == null) { continue; }

            if (decode.isCustomData)
            {
                _need_send = false;
                ///   
                this.ProcessWithCustomData(decode);
            }
            else
            {

                ArrayList data = FrameData.CreateWithMultiJson(decode.data);

                int fps = (data[0] as FrameData).fps;
                if (fps > current_max_fps)
                {
                    current_max_fps = fps;
                }

                if (framedatas[fps - 1] != null)
                {
                    Debug.LogError("has exist  " + fps);
                    break;
                }
                framedatas[fps - 1] = data;
                this.ProcessWithFrameData();
            }

        }

        if (app.isOver) return;
        if (_need_send == false) return;//如果没有收到帧信息 那么跳过下面步骤

        // 处理玩家输入信息，（当前在帧里）
        //   _recvQueue.UnLock();

        FrameData dd = FrameData.Create();
        dd.no = HeroMgr.ins.me_no;
        //分析玩家操作

        if (PublicData.ins.IS_left)
        {
            dd.left = 1;
            PublicData.ins.IS_left = false;
        }

        if (PublicData.ins.IS_right)
        {
            dd.right = 1;
            PublicData.ins.IS_right = false;
        }

        if (PublicData.ins.IS_atk)
        {
            dd.atk = 1;
            PublicData.ins.IS_atk = false;
        }
        if (PublicData.ins.IS_jump)
        {
            dd.jump = 1;
            PublicData.ins.IS_jump = false;
        }

        if (PublicData.ins.IS_stand)
        {
            dd.stand = 1;
            PublicData.ins.IS_stand = false;
        }
        if (PublicData.ins.IS_s1)
        {
            dd.s1 = 1;
            PublicData.ins.IS_s1 = false;
        }

        app.Send(dd.toUploadJson());
        //   Debug.Log("upload " + dd.toUploadJson());
    }
    public void ProcessWithCustomData(TranslateDataPack decode)
    {
        string cmd = decode.customs[0] as string;
        if (cmd == "Start")
        {//开始游戏
            int seed = int.Parse(decode.customs[1] as string);
            app.isStart = true;
            // FoodsMgr.ins.SetRandomSeed(seed);

            app.randObj = new System.Random(seed);
            //    FoodsMgr.ins.Init();
            HeroMgr.ins.me_no = int.Parse(decode.customs[2] as string);

            on_enter_max_fps = int.Parse(decode.customs[3] as string);
            Debug.Log("Current max fps=" + on_enter_max_fps);


            /*
            
            Hero h2 = HeroMgr.Create<BattleHero>();
             h2.team = 0xfff;
             h2.no = HeroMgr.ins.me_no;
             HeroMgr.ins.self = h2;
             */
            PublicData.GetInstance()._on_enter_max_fps = on_enter_max_fps;

        }

        else if (cmd == "Over")
        {//游戏结束
            app.SetGameOver();
        }
        else if (cmd == "ReConnect")
        {//重新连接
            EventDispatcher.ins.PostEvent(Events.ID_UI_NOWAIT);
        }
        else if (cmd == "new")
        {//新玩家
            string name = decode.customs[1] as string;
            int no = int.Parse(decode.customs[2] as string);
            if (no == HeroMgr.ins.me_no) return;

            Hero h2 = HeroMgr.Create<BattleHero>();
            h2.team = 0xfff;
            h2.no = no;
            h2.name = name;

        }
        else if (cmd == "new_pvp_friend_ai")
        {//pvp 好友 离线模式 模式 新玩家

            EventDispatcher.ins.PostEvent(Events.ID_LOADING_HIDE);
            int no = int.Parse(decode.customs[1] as string);
            ///  if (no == HeroMgr.ins.me_no)
            //  if (HeroMgr.ins.GetHero(no) != null) return;
            {
                Debug.Log("NEW PVP player no= " + no);
                BattleHero h2 = HeroMgr.Create<BattleHero>();
                h2.team = PublicData.ins.user_pvp_other.no;
                h2.no = PublicData.ins.user_pvp_other.no; ;
                h2.name = PublicData.ins.user_pvp_other.name;

                h2.x = 10;//目标玩家初始在右边
                h2.flipX = 1f;
                HeroMgr.ins.self.x = 3.6f;
                h2.SetPVPAIEnable(true);

            }

        }
        else if (cmd == "new_pvp_friend")
        {//pvp 好友  模式 新玩家

            EventDispatcher.ins.PostEvent(Events.ID_LOADING_HIDE);
            int no = int.Parse(decode.customs[1] as string);
            bool is_pve = PublicData.ins.is_pve;

            if (PublicData.ins.is_client_server)
            { //客户服务器

                if (HeroMgr.ins.GetHero(no) != null) return;


                Debug.Log("NEW PVP player no= " + no);
                BattleHero h2 = HeroMgr.Create<BattleHero>();

                if (is_pve)
                {
                    h2.team = 0xfff;
                    h2.no = no; ;
                    h2.name = no.ToString();

                    if (Json.Decode(PublicData.ins.client_server_room_info)["p1"] == no.ToString())
                    {//默认self是发起方

                        PublicData.ins.is_pvp_friend_owner = true;
                        HeroMgr.ins.self = h2;
                        HeroMgr.ins.me_no = h2.no;
                        h2.x = 0.5f;//目标玩家初始在右边

                    }
                    else
                    {
                        PublicData.ins.is_pvp_friend_owner = false;
                        h2.x = 0.7f;//目标玩家初始在右边

                    }
                }
                else
                {
                    //pvp
                    h2.team = no;
                    h2.no = no; ;
                    h2.name = no.ToString();

                    if (Json.Decode(PublicData.ins.client_server_room_info)["p1"] == no.ToString())
                    {//默认self是发起方

                        PublicData.ins.is_pvp_friend_owner = true;
                        HeroMgr.ins.self = h2;
                        HeroMgr.ins.me_no = h2.no;
                        h2.x = 3.6f;//目标玩家初始在右边

                    }
                    else
                    {
                        PublicData.ins.is_pvp_friend_owner = false;
                        h2.x = 10;//目标玩家初始在右边
                        h2.flipX = 1f;
                    }
                }
            }
            else
            {///UnityClient
                ///    if (no == HeroMgr.ins.me_no) return;
                if (HeroMgr.ins.GetHero(no) != null) return;

                Debug.Log("   NEW player no= " + no);
                BattleHero h2 = HeroMgr.Create<BattleHero>();

                if (is_pve)
                {
                    h2.team = 0xfff;
                    h2.no = PublicData.ins.user_pvp_other.no; ;
                    h2.name = PublicData.ins.user_pvp_other.name;

                    h2.x = 0.7f;//目标玩家初始在右边
                    if (HeroMgr.ins.me_no == no)
                    {
                        HeroMgr.ins.self = h2;
                        h2.no = PublicData.ins.self_user.no;
                        h2.name = PublicData.ins.self_user.name;
                    }

                    if (PublicData.ins.is_pvp_friend_owner)
                    {
                        //我是发起方
                        h2.x = 0.7f;
                        if (HeroMgr.ins.self != null)
                        {
                            HeroMgr.ins.self.x = 0.5f;
                        }
                    }
                    else
                    {//我不是发起方

                        h2.x = 0.5f;
                        if (HeroMgr.ins.self != null)
                        {
                            HeroMgr.ins.self.x = 0.7f;
                        }
                    }
                }
                else
                {
                    //pvp

                    h2.team = PublicData.ins.user_pvp_other.no;
                    h2.no = PublicData.ins.user_pvp_other.no; ;
                    h2.name = PublicData.ins.user_pvp_other.name;

                    h2.x = 10;//目标玩家初始在右边
                    if (HeroMgr.ins.me_no == no)
                    {
                        HeroMgr.ins.self = h2;
                        h2.no = PublicData.ins.self_user.no;
                        h2.name = PublicData.ins.self_user.name;
                        h2.team = h2.no;
                    }
                    if (PublicData.ins.is_pvp_friend_owner)
                    {
                        //我是发起方
                        h2.x = 10;
                        h2.flipX = 1f;
                        if (HeroMgr.ins.self != null)
                        {
                            var xxx = HeroMgr.ins.self;
                            HeroMgr.ins.self.x = 3.6f;
                            HeroMgr.ins.self.flipX = -1.0f;
                        }
                    }
                    else
                    {//我不是发起方

                        h2.x = 3.6f;
                        h2.flipX = -1f;
                        if (HeroMgr.ins.self != null)
                        {
                            HeroMgr.ins.self.x = 10;
                            HeroMgr.ins.self.flipX = 1f;

                        }

                    }
                }
            }
        }

        else
        {
            Debug.LogError("ProcessWithCustomData:UnKnow cmd=" + cmd);
        }

    }
    private void InitPVE()
    {
        PublicData.ins.battle_random_seed = int.Parse(PublicData.ins.pvp_room_no);
        var randObj = new System.Random(PublicData.ins.battle_random_seed);
        {
            for (int i = 0; i < 100; i++)
            {
                Enemy e1 = EnemyMgr.Create<Enemy221>();
                e1.x = randObj.Next(5, 80);
                ;
               //    e1.x = 5;
                e1.y = 5;
                e1.team = 333;
            }
            /*  for (int i = 0; i < 10; i++)
              {
                  Enemy e1 = EnemyMgr.Create<Enemy>();
                  e1.x = Random.Range(5, 80);
                  ///   e1.x =5;
                  e1.y = 5;
                  e1.team = 333;
              }*/


         /*   for (int i = 0; i < 25; i++)
            {
                Enemy e1 = EnemyMgr.Create<Enemy>();
                e1.x = randObj.Next(5, 80);
                ;
                //    e1.x = 5;
                e1.y = 5;
                e1.team = 333;
            }*/
        }
    }
    public void ProcessWithFrameData()
    {
        if (current_fps == 0 && PublicData.ins.is_pve)
        {
            this.InitPVE();
        }
        if(current_fps==0)
        {
            if (PublicData.ins.is_client_server == false)
            {
                Utils.SetTargetFPS(80);
            }
        }
        AutoReleasePool.ins.Clear();//更新前 先清理一次 使每帧 都有效清理

        ArrayList data = (ArrayList)framedatas[current_fps];
        {
            if (data == null)
            {
                Debug.LogError("FrameData:" + (current_fps + 1) + " Not Find");
                _FRAMEDATA_EMPTY_ERROR_COUNTER++;
                if (_FRAMEDATA_EMPTY_ERROR_COUNTER > 100)
                {
                    // Application.Quit();//数据丢失超过限制 强制回到主界面
                    //TODO  FIND BUG OF THIS(FRAME DATA EMPTY)
                    //    SceneMgr.Load(DATA.RES_SCENE_MAIN);

                }
                return;
            }
        }

        this.current_fps++;
        foreach (FrameData f in data)
        {
            if (f.no == 0) continue;
            Hero hero = HeroMgr.ins.GetHero(f.no);
            if (hero == null)
            {
                Debug.LogError("Can not fild ball no=" + f.no);
                continue;
            }

            if (f.left == 1)
            {
                hero.left = true;
            }
            if (f.right == 1)
            {
                hero.right = true;
            }
            if (f.jump == 1)
            {
                hero.jump = true;
            }
            if (f.stand == 1)
            {
                hero.stand = true;
            }
            if (f.atk == 1)
            {
                hero.atk = true;
            }
            if (f.s1 == 1)
            {
                hero.s1 = true;
            }


            /*  if (f.no == 0) continue;
           
              if (f.op == -1)
              {//普通帧信息
                  if (f.dir != -1)
                  {
                      ArrayList list = BallsMgr.ins.GetBallsByNo(f.no);
                      foreach (Ball b in list)
                      {
                          b.dir = f.dir;
                      }
                  }
              }
              else if (f.op == 0)
              {//放孢子

                  FoodsMgr.ins.CreateChild(BallsMgr.ins.GetTheMaxOne(obj.no));
                  //  Debug.Log("孢子");
              }
              else if (f.op == 1)
              {//分裂
                  BallsMgr.ins.CreateSplit(f.no);

              }
              else
              {
                  Debug.LogWarning("ProcessWithFrameData:UnKnow op=" + f.op);
              }*/
        }
        //process update
        ModelMgr.ins.UpdateMS();
        ViewMgr.ins.UpdateMS();

        if (HeroMgr.ins.self.current_hp <= 0)
        {
            this.AddSendMsg("cmd:over");
        }
        if (PublicData.ins.is_pve && EnemyMgr.ins.GetEnemyCount() <= 0)
        {
            this.AddSendMsg("cmd:over");
        }
        if (current_fps < current_max_fps)
        {
            this.ProcessWithFrameData();
        }

    }

    public void AddSendMsg(string msg)
    {
        app.AddSendMsg(msg);
    }

}



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
            ///    Debug.Log("Read From File:      " + str);
            this.AddRecvMsg(str.Substring(0, str.Length - 1));
        }



    }
    public void InitNet(bool isReConnect = false)
    {
        if (PublicData.ins.is_client_server)
        {
            this.InitWithClientServer();
            socket = new SockClientEmpty();
            EventDispatcher.ins.PostEvent(Events.ID_LOADING_HIDE);
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
            ///  EventDispatcher.ins.PostEvent(Events.ID_BATTLE_PVP_WAITFOR_RESULT_SHOW);

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




        ViewMgr.ins.Dispose();
        //   BallsMgr.ins.Dispose();
        //   FoodsMgr.ins.Dispose();
        ModelMgr.ins.Dispose();
        EventDispatcher.DestroyInstance();
        AutoReleasePool.DestroyInstance();
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

