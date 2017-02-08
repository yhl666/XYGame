﻿using UnityEngine;
using System.Collections;
using System;
using System.Threading;

public class BattleApp111 : AppBase
{

    public override bool Init()
    {
        base.Init();

        ViewUI.Create<UIPubblicRoot>();

        Hero h1 = HeroMgr.Create<Hero>();
        h1.team = 1;

        Hero h2 = HeroMgr.Create<Hero>();
        h2.team = 2;



        ViewUI.Create<UIBattleRoot>();

        /*  for (int i = 0; i < 100; i++)
          {
              EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
            {

                return 10;
            }));

          }
          */
        return true;
    }


    public override void UpdateMS()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent("attack");

        }
        if (Input.GetKey(KeyCode.A))
        {


            HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent("run_left");
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent("stand");
        }
        if (Input.GetKey(KeyCode.D))
        {


            HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent("run_right");
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent("stand");
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {

            HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent("jump");

        }

        //  EventDispatcher.ins.PostEvent("attack");


        ModelMgr.ins.UpdateMS();
        ViewMgr.ins.UpdateMS();



        AutoReleasePool.ins.Clear();
    }



}










public sealed class BattleApp : AppBase
{

    public BattleWorldMap GetCurrentWorldMap()
    {
        return this.worldMap;
    }
    public override bool Init()
    {
        EventDispatcher.ins.AddEventListener(this, Events.ID_EXIT);
        isOver = false;
        _ins = this;



        for (int i = 0; i < Config.MAX_FRAME_COUNT; i++)
        {
            this.framedatas.Add(null);
        }

        this.worldMap = ModelMgr.Create<BattleWorldMap>();

        ViewUI.Create<UIPubblicRoot>();
        ViewUI.Create<UIBattleRoot>();

      /*  for (int i = 0; i < 100; i++)
        {
            EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
            {
                return DATA.EMPTY_STRING;
            }));
        }

        */
        EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_LOADING_SHOW);
            return "本地资源加载完成，初始化网络资源";
        }));

        EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
        {
         //   Thread.Sleep(1000);
            BattleApp.ins.InitNet(false);

            return "初始化网络资源，等待服务器响应";
        }));

        //   BattleApp.ins.InitNet(false);


        ///   EventDispatcher.ins.AddEventListener(this, Events.ID_EXIT);
        return true;

    }
    bool isStart = false;
    int on_enter_max_fps = 0;//加入游戏时最大帧数
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
        this.InitNet(true);
        EventDispatcher.ins.PostEvent(Events.ID_UI_WAIT, DATA.UI_RECONNECTING);
        string info = "cmd:reconnect:" + this.current_fps + ":" + HeroMgr.ins.GetSelfHero().no;
        Debug.Log(info);
        this.socket.AddSendMsg(info);


    }
    public void InitNet(bool isReConnect = false)
    {

        if (PublicData.GetInstance().isVideoMode)
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
            this.socket.AddSendMsg("cmd:new:" + PublicData.GetInstance().player_name);

        }


    }


    public override void Update()
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















        this.Process();
        AutoReleasePool.ins.Clear(); // 没帧数结束 清理一次， 逻辑帧 Process内部已处理
    }



    private void ProcessWithGameOver()
    {
        if (this.isVideoMode() == false)
        {

        }

        this.Dispose();

    }

    public void SetGameOver()
    {
        Debug.LogError("Over");
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
        ModelMgr.ins.Update();
        ViewMgr.ins.Update();

        do
        { // do while 流程
            if (isOver)
            {
                this.ProcessWithGameOver();

                break;

            }



            if (on_enter_max_fps > current_fps && isStart == true)
            {
                EventDispatcher.ins.PostEvent(Events.ID_UI_WAIT, DATA.UI_WAIT_ENTER + "   " + current_fps * 100 / on_enter_max_fps + " %");
                isStart = true;
            }
            else if (isStart == true && on_enter_max_fps < current_fps)
            {
                isStart = false;
                EventDispatcher.ins.PostEvent(Events.ID_UI_NOWAIT);
            }
            else if (on_enter_max_fps < current_fps && isStart == false)
            {
                isStart = false;
            }

            // process to the lastest frame (if current frame less than max frame(from server )  this will block until current frame


            bool _need_send = false;
            //  _recvQueue.Lock();
            //处理帧
            int i = 0;
            while (_recvQueue.Empty() == false)
            {
                i++;

                if (i > 80) break;//连续计算80帧强制刷新UI
                _need_send = true;
                //     if (_recvQueue.Count() % 50 == 0) return;
                string xx = _recvQueue.Dequeue() as string;

                TranslateDataPack decode = TranslateDataPack.Decode(xx);
                //    Debug.Log(xx);
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

            if (isOver) break;
            if (_need_send == false) break;//如果没有收到帧信息 那么跳过下面步骤

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

            this.Send(dd.toUploadJson());
            //   Debug.Log("upload " + dd.toUploadJson());
        } while (false);



    }

    public void ProcessWithCustomData(TranslateDataPack decode)
    {
        string cmd = decode.customs[0] as string;
        if (cmd == "Start")
        {//开始游戏
            int seed = int.Parse(decode.customs[1] as string);
            isStart = true;
            // FoodsMgr.ins.SetRandomSeed(seed);

            this.randObj = new System.Random(seed);
            //    FoodsMgr.ins.Init();
            HeroMgr.ins.me_no = int.Parse(decode.customs[2] as string);

            on_enter_max_fps = int.Parse(decode.customs[3] as string);
            Debug.Log("Current max fps=" + on_enter_max_fps);


            Hero h2 = HeroMgr.Create<Hero>();
            h2.team = 2;
            h2.no = HeroMgr.ins.me_no;
            HeroMgr.ins.self = h2;

            PublicData.GetInstance()._on_enter_max_fps = on_enter_max_fps;

            EventDispatcher.ins.PostEvent(Events.ID_LOADING_HIDE);
        }

        else if (cmd == "Over")
        {//游戏结束
            this.SetGameOver();
        }
        else if (cmd == "ReConnect")
        {//重新连接
            EventDispatcher.ins.PostEvent(Events.ID_UI_NOWAIT);
        }
        else if (cmd == "new")
        {//新玩家

            string name = decode.customs[1] as string;
            int no = int.Parse(decode.customs[2] as string);

        }
        else if (cmd == "chat")
        {//聊天

        }
        else
        {
            Debug.LogWarning("ProcessWithCustomData:UnKnow cmd=" + cmd);
        }

    }
    private int _FRAMEDATA_EMPTY_ERROR_COUNTER = 0;
    public void ProcessWithFrameData()
    {
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
        if (current_fps < current_max_fps)
        {
            this.ProcessWithFrameData();
        }

    }

    public void AddRecvMsg(string msg)
    {
        _recvQueue.Enqueue(msg);
        //     Debug.Log("[LOG]:Recv:  " + msg);
    }

    public void AddRecvMsgUnSafe(string msg)
    {
        _recvQueue.UnSafeEnqueue(msg);
        //   Debug.Log("[LOG]:Recv:  " + msg);
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

    private string PickOneRecvMsg()
    {
        return (string)_recvQueue.Dequeue();
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


    public int GetCurrentFrame() { return this.current_fps; }


    private int current_fps = 0;

    bool isOver = false;
    ArrayList framedatas = new ArrayList();


    public ThreadSafeQueue _recvQueue = new ThreadSafeQueue();

    private SocketClient socket;


    System.Random randObj;

    private int current_max_fps = 0;
    private BattleWorldMap worldMap = null;

    public static BattleApp ins
    {
        get
        {
            return BattleApp.GetInstance();
        }
    }

    public static BattleApp _ins = null;

    public static BattleApp GetInstance()
    {
        return _ins;
    }


    public override void OnDispose()
    {
        ViewMgr.ins.Dispose();
        //   BallsMgr.ins.Dispose();
        //   FoodsMgr.ins.Dispose();
        ModelMgr.ins.Dispose();
        EventDispatcher.DestroyInstance();
        AutoReleasePool.DestroyInstance();
        _ins = null;
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

    }
}

