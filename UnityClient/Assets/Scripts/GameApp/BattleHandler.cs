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
        if (PublicData.ins.inputAble == false) return;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PublicData.ins.IS_jump = true;
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PublicData.ins.IS_s1 = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PublicData.ins.IS_s1 = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PublicData.ins.IS_s1 = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PublicData.ins.IS_s1 = 4;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PublicData.ins.IS_atk = true;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            PublicData.ins.IS_s1 = 5;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PublicData.ins.IS_atk = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PublicData.ins.IS_s1 = 5;
        }

    }
}

class BattleHandlerBase : GAObject
{

    public override void UpdateMS()
    {

    }
    public virtual void UpdateMSBefore()
    {

    }
    public virtual void UpdateMSAfter()
    {
        /*  if (HeroMgr.ins.self.current_hp <= 0)
          {
              app.AddSendMsg("cmd:Over");
          }
          if (PublicData.ins.is_pve && EnemyMgr.ins.GetEnemyCount() <= 0)
          {
              app.AddSendMsg("cmd:Over");
          }*/
    }
    public override bool Init()
    {
        return base.Init();
    }

    public BattleApp app = null;
}

sealed class BattlePVEHandler : BattleHandlerBase
{
    public override void UpdateMS()
    {

    }
    public override bool Init()
    {
        base.Init();
        PublicData.ins.battle_random_seed = int.Parse(PublicData.ins.pvp_room_no);
        Tower tower = BuildingMgr.Create<Tower>();
        tower.x = 5f;
        tower.z = 3f;
        tower.current_hp = 15;
        tower.hp = 0xffff;
        var randObj = new System.Random(PublicData.ins.battle_random_seed);
        Utils.random_frameMS = randObj;
        {
            for (int i = 0; i <0; i++)
            {
                Enemy e1 = EnemyMgr.Create<EnemyBoss>();
                e1.x = randObj.Next(0, 340000) / 10000f;
                e1.z = randObj.Next(1000, 4000) / 1000.0f;

                //   e1.x = 5+i*0.1f;   
                ///   e1.x = 55555;
                e1.y = 5;
                e1.team = 333;
            }
        }
        HeroMgr.ins.self.z = 3f;
        HeroMgr.ins.self.x = 8f;

        return true;
    }
    public static BattleHandlerBase Create(BattleApp app)
    {
        BattleHandlerBase ret = new BattlePVEHandler();
        ret.app = app;
        return ret;
    }
    private BattlePVEHandler() { }
}

sealed class BattlePVPHandler : BattleHandlerBase
{
    public override void UpdateMS()
    {

    }
    public override bool Init()
    {
        return base.Init();
    }
    public static BattleHandlerBase Create(BattleApp app)
    {
        BattleHandlerBase ret = new BattlePVPHandler();
        ret.app = app;
        return ret;
    }
    private BattlePVPHandler() { }
}

/// <summary>
/// 帧同步相关处理
/// </summary>
public sealed class BattleSyncHandler
{
    BattleHandlerBase battleHandler = null;
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
        ///    Debug.Log("[LOG]:Recv:  " + msg);
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
        if (PublicData.ins.IS_s1 != 0)
        {
            dd.s1 = PublicData.ins.IS_s1;
            PublicData.ins.IS_s1 = 0;
        }
        if (PublicData.ins.IS_revive_point != 0)
        {
            dd.revive = PublicData.ins.IS_revive_point;
            PublicData.ins.IS_revive_point = 0;
        }
        if (PublicData.ins.IS_dir > 0) // default -1
        {
            dd.dir = PublicData.ins.IS_dir;
            PublicData.ins.IS_dir = -1;
        }
        if ((int)PublicData.ins.IS_opt > 0)
        {
            dd.opt = (int)PublicData.ins.IS_opt;
            PublicData.ins.IS_opt = FrameCustomsOpt.UnKnown;
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
        else if (cmd == "revive")
        {

        }
        else
        {
            Debug.LogError("ProcessWithCustomData:UnKnow cmd=" + cmd);
        }

    }

    private void InitWithFirstFrame()
    {
        if (PublicData.ins.is_client_server == false)
        {
            Utils.SetTargetFPS(80);
        }

        if (PublicData.ins.is_pve)
        {

            this.battleHandler = BattlePVEHandler.Create(this.app);
            this.battleHandler.Init();
        }
        else
        {
            this.battleHandler = BattlePVPHandler.Create(this.app);
            this.battleHandler.Init();
        }
    }
    public void ProcessWithFrameData()
    {
        if (current_fps == 0)
        {
            this.InitWithFirstFrame();
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

        this.battleHandler.UpdateMSBefore();

        foreach (FrameData f in data)
        {
            if (f.no == 0) continue;
            Hero hero = HeroMgr.ins.GetHero(f.no);
            if (hero == null)
            {
                Debug.LogError("Can not find Entity no=" + f.no);
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
            if (f.s1 != 0)
            {
                hero.s1 = f.s1;
            }
            if (f.revive != 0)
            {
                BufferRevive b = new BufferRevive();
                b.point_index = f.revive;
                hero.AddBuffer(b);
            }
            if (f.opt > 0)
            {//收到了自定义2数据 
                hero.opt = (FrameCustomsOpt)f.opt;
            }
            hero.dir = f.dir; // 收到了数据 直接 处理
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
        this.battleHandler.UpdateMSAfter();

        ViewMgr.ins.UpdateMS();
        TimerQueue.ins.TickMS();

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
