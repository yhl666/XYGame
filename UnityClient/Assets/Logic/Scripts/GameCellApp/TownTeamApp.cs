/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

namespace Services
{

    public class TownTeam : RpcService
    {
        /// <summary>
        /// 进入房间 消息
        /// 目前 只存在队员进入房间
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cb"></param>
        public void EnterTeam(string msg, VoidFuncString cb)
        {
            cb("ret:ok,");
            HashTable kv = Json.Decode(msg);
            //  string name = kv["name"];
            //    string no = kv["no"];
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_RPC_ENTERTEAM, kv);

        }

        /// <summary>
        /// 退出房间消息
        /// 队长的话收到的是对园内退出
        /// 队员的话收到的是队长退出
        /// 队长退出后 队伍会被解散 客户端处理即可 服务器数据会被立刻抹掉
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cb"></param>
        public void LeaveTeam(string msg, VoidFuncString cb)
        {
            HashTable kv = Json.Decode(msg);
            //  string name = kv["name"];
            //    string no = kv["no"];
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_RPC_LEAVETEAM, kv);
            cb("ret:ok,");
        }
        public void ChangeSkill(string msg, VoidFuncString cb)
        {
            HashTable kv = Json.Decode(msg);
            //no 是切换技能的玩家no 而不是房间no
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_RPC_CHANGESKILL, kv);
            cb("ret:ok,");
        }
        public void ChangeState(string msg, VoidFuncString cb)
        {
            HashTable kv = Json.Decode(msg);
            //no 是改变状态的玩家no 而不是房间no
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_RPC_OK, kv);
            cb("ret:ok,");
        }
        /// <summary>
        /// 玩家点击 开始 or 准备按钮
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cb"></param>
        public void Start(string msg, VoidFuncString cb)
        {
            HashTable kv = Json.Decode(msg);
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_RPC_OK, kv);
            cb("ret:ok,");
        }
        /// <summary>
        /// 开始游戏，
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cb"></param>
        public void StartGame(string msg, VoidFuncString cb)
        {
            cb("ret:ok,");
            HashTable kv = Json.Decode(msg);
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_RPC_STARTGAME, msg);
        }
    }

}

/// <summary>
/// 组队玩家信息
/// </summary>
public sealed class TownTeamPlayer
{
    private TownTeamPlayer() { }
    public int no = 0;//玩家id
    public string name;
    public State state = State.Wait;
    public int SkillGroup = 1;// 技能组 1 2 
    public bool IsCaptain = false;

    public string state_string
    {
        get
        {
            return consts_type[(int)state];
        }
    }
    string[] consts_type = { "准备完毕", "等待中" };
    public void ChangeSkillGroup()
    {
        if (SkillGroup == 1)
        {
            SkillGroup = 2;
        }
        else if (SkillGroup == 2)
        {
            SkillGroup = 1;
        }
        else
        {

        }
    }
    public void Reset()
    {
        state = State.Wait;
        SkillGroup = 1;// 技能组 1 2 
    }
    public void ChangeState()
    {
        if (state == State.Wait)
        {
            state = State.Ready;
        }
        else if (state == State.Ready)
        {
            state = State.Wait;
        }
    }

    public enum State
    {
        Ready = 0,//已准备
        Wait,//等待
    }
    public static TownTeamPlayer Create(int no, string name = "")
    {
        TownTeamPlayer ret = new TownTeamPlayer();
        ret.no = no;
        ret.name = name;
        return ret;
    }
}

/// <summary>
/// 组队队伍信息
/// </summary>
public sealed class TownTeaminfo
{
    public static TownTeaminfo Create(int no)
    {
        TownTeaminfo ret = new TownTeaminfo();
        ret.no = no;
        return ret;
    }
    private TownTeaminfo() { }
    public int no;//房间id
    public int Captain = 0; // 队长no

    public ArrayList list = new ArrayList();
    public bool AddPlayer(int no, string name)
    {
        if (GetPlayer(no) != null)
        {
            //  return false;
        }

        var p = TownTeamPlayer.Create(no, name);
        list.Add(p);
        if (no == Captain)
        {
            p.IsCaptain = true;
        }
        return true;
    }
    public bool RemovePlayer(int no)
    {
        var p = GetPlayer(no);
        if (p == null) return false;
        list.Remove(p);
        return true;
    }
    public int GetCurrentPlayerCount()
    {
        return list.Count;
    }
    public TownTeamPlayer GetPlayer(int no)
    {
        foreach (TownTeamPlayer p in list)
        {
            if (no == p.no) return p;
        }
        return null;
    }
    public bool IsSelfCaptain()
    {
        if (Captain == PublicData.ins.self_user.no) return true;
        return false;
    }
    public bool CanStartGame()
    {
        if (IsFull() == false) return false;
        foreach (TownTeamPlayer p in list)
        {
            if (p.IsCaptain == false && p.state == TownTeamPlayer.State.Wait)
            {
                return false;
            }
        }
        return true;
    }
    public bool IsFull()
    {
        if (list.Count >= 2) return true;
        return false;
    }
    /// <summary>
    /// 获得队长
    /// </summary>
    /// <returns></returns>
    public TownTeamPlayer GetCaptain()
    {
        return GetPlayer(Captain);
    }
    /// <summary>
    /// 获得出 队长的其他玩家
    /// </summary>
    /// <returns></returns>
    public ArrayList GetPlayerExceptCaptain()
    {
        ArrayList ret = new ArrayList();
        foreach (TownTeamPlayer p in list)
        {
            if (p.IsCaptain == false)
            {
                ret.Add(p);
            }
        }
        return ret;
    }

    //------------------------玩家操作接口
    /// <summary>
    /// 进入房间
    /// </summary>
    /// <param name="no"></param>
    /// <returns></returns>
    public bool EnterTeam(int no, string name)
    {
        if (GetPlayer(no) != null)
        {
            return false;
        }
        this.AddPlayer(no, name);
        return true;
    }
    /// <summary>
    /// 离开房间
    /// </summary>
    /// <param name="no"></param>
    /// <returns></returns>
    public bool RemoveTeam(int no)
    {
        if (GetPlayer(no) == null)
        {
            //  return false;
        }
        this.RemovePlayer(no);
        return true;
    }
    /// <summary>
    /// 改变技能组
    /// </summary>
    /// <returns></returns>
    public bool ChangeSkillGroup(int no)
    {
        var p = GetPlayer(no);
        if (p == null)
        {
            return false;
        }
        p.ChangeSkillGroup();
        return true;
    }
    /// <summary>
    /// 改变状态
    /// </summary>
    /// <returns></returns>
    public bool ChangeState(int no)
    {
        var p = GetPlayer(no);
        if (p == null)
        {
            return false;
        }
        p.ChangeState();
        return true;
    }
    public void SetJsonn()
    {

    }
    public string ToJson()
    {
        return "";
    }
}


/// <summary>
/// 组队系统
/// </summary>
public class TownTeamApp : CellApp
{
    TownTeaminfo info = null;
    UI_townteamapp ui = null;
    public TownTeamApp() { }
    public override bool Init()
    {
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_BTN_SKILL);//队伍信息 切换技能组
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_BTN_CLOSE); // 队伍信息 退出队伍
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_BTN_OK); // 队伍信息 开始游戏or 准备
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_BTN_MAIN_CREATE); //  主界面 创建房间
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_BTN_MAIN_JOIN); //  主界面 加入
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_BTN_MAIN_RANDOM); //  主界面 随机
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_BTN_MAIN_SEARCH); //  主界面 搜索


        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_RPC_ENTERTEAM); //  rpc 进入房间
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_RPC_LEAVETEAM); //  rpc 进入房间
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_RPC_CHANGESKILL); //  rpc 进入房间
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_RPC_OK); //  rpc 进入房间
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_RPC_STARTGAME); //  rpc 进入房间

        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_BTN_ALL_CLOSE); //   
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_BTN_SHOW); //   


        return true;
    }

    private void JoinRoom(string no)
    {
        string json = "no:" + no + ",other:" + PublicData.ins.self_user.no.ToString() + ",name:" + PublicData.ins.self_user.name + ",";

        RpcClient.ins.SendRequest("services.battle_team", "join", json, (string msg) =>
        {
            HashTable kv = Json.Decode(msg);
            if ("ok" == kv["ret"])
            {
                ui.ShowInfoPanel();
                info = TownTeaminfo.Create(kv.GetInt("no"));
                info.Captain = kv.GetInt("captain");
                info.AddPlayer(info.Captain, kv.Get("name"));
                info.AddPlayer(PublicData.ins.self_user.no, PublicData.ins.self_user.name);
                ui.Sync(info);
            }
            else
            {
                EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "加入房间失败!" + kv["msg"]);
            }
        });

    }
    public override void OnEvent(int type, object userData)
    {
        TownApp p = (this.parent as TownApp);

        if (type == Events.ID_TOWN_TEAM_BTN_ALL_CLOSE)
        {
            UI_townteamapp ui = userData as UI_townteamapp;
            p.isOneCellAppShowLock = false;
            this.ui = ui;
        }
        if (type == Events.ID_TOWN_TEAM_BTN_SHOW)
        {
            UI_townteamapp ui = userData as UI_townteamapp;
            p.isOneCellAppShowLock = true;
            this.ui = ui;
        }
        else if (type == Events.ID_TOWN_TEAM_BTN_MAIN_CREATE)
        {//创建房间
            string json = "no:" + PublicData.ins.self_user.no.ToString() + ",name:" + PublicData.ins.self_user.name + ",";
            RpcClient.ins.SendRequest("services.battle_team", "create", json, (string msg) =>
            {
                HashTable kv = Json.Decode(msg);
                if (kv["ret"] != "ok")
                {
                    EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "房间房间失败" + kv["msg"]);
                }
                UI_townteamapp ui = userData as UI_townteamapp;
                info = TownTeaminfo.Create(kv.GetInt("no"));
                info.Captain = kv.GetInt("captain");
                info.AddPlayer(info.Captain, PublicData.ins.self_user.name);

                ui.Sync(info);
                this.ui = ui;
            });
        }
        else if (type == Events.ID_TOWN_TEAM_BTN_MAIN_JOIN)
        {//加入房间
            UI_townteamapp ui = userData as UI_townteamapp;
            this.ui = ui;
            this.JoinRoom(ui.join_team_no);
        }
        else if (type == Events.ID_TOWN_TEAM_BTN_MAIN_RANDOM)
        {//随机房间
            UI_townteamapp ui = userData as UI_townteamapp;
            this.ui = ui;
            RpcClient.ins.SendRequest("services.battle_team", "random", "no:123,", (string msg1) =>
            {
                HashTable kv1 = Json.Decode(msg1);
                if ("ok" == kv1["ret"])
                {
                    this.JoinRoom(kv1["no"]);
                }
                else
                {
                    EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, kv1["msg"]);
                }
            });
        }
        else if (type == Events.ID_TOWN_TEAM_BTN_MAIN_SEARCH)
        {//搜索房间
            RpcClient.ins.SendRequest("services.battle_team", "search", "no:123,", (string msg) =>
            {
                if (msg != "")
                {
                    ui.ShowSearchList(Json.MultiDecode(msg));
                }
                else
                {
                    EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "当前没有任何房间!");
                }
            });
        }
        else if (type == Events.ID_TOWN_TEAM_BTN_SKILL)
        {//切换技能 通过服务器来通知其他客户端 服务器不做状态存储
            if (info.IsFull() == false)
            {
                EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "请等待其他玩家加入后再操作!");
                return;
            }
            string json = "no:" + info.no.ToString() + ",player_no:" + PublicData.ins.self_user.no.ToString() + ",";
            RpcClient.ins.SendRequest("services.battle_team", "change_skill", json, (string msg) =>
            {
                HashTable kv = Json.Decode(msg);
                if ("ok" == kv["ret"])
                {
                    info.GetPlayer(PublicData.ins.self_user.no).ChangeSkillGroup();
                    ui.Sync(info);
                    EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "切换技能组OK!");
                }
                else
                {
                    EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "切换技能组失败!" + kv["msg"]);
                }
            });
        }
        else if (type == Events.ID_TOWN_TEAM_BTN_OK)
        {
            if (info.IsSelfCaptain())
            {//我是队长  队员未满
                if (info.IsFull() == false)
                {//我是队长  队员未满
                    EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "等待队员进入!");
                }
                else if (info.CanStartGame())
                {//我是队长  所有队员准备完毕

                    string json = "no:" + info.no.ToString() + ",";
                    RpcClient.ins.SendRequest("services.battle_team", "start_game", json, (string msg) =>
                    {
                        HashTable kv = Json.Decode(msg);
                        if ("ok" == kv["ret"])
                        {
                            EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "发起开始游戏成功!");
                        }
                        else
                        {
                            EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "失败!" + kv["msg"]);
                        }
                    });
                }
                else
                {
                    EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "队员未准备!");
                }
            }
            else
            {
                string json = "no:" + info.no.ToString() + ",player_no:" + PublicData.ins.self_user.no.ToString() + ",";
                RpcClient.ins.SendRequest("services.battle_team", "change_state", json, (string msg) =>
                {
                    HashTable kv = Json.Decode(msg);
                    if ("ok" == kv["ret"])
                    {
                        info.GetPlayer(PublicData.ins.self_user.no).ChangeState();
                        ui.Sync(info);
                    }
                    else
                    {
                        EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "准备失败!" + kv["msg"]);
                    }
                });
            }

        }
        else if (type == Events.ID_TOWN_TEAM_BTN_CLOSE)
        {//离开队伍
            RpcClient.ins.SendRequest("services.battle_team", "leave", "no:" + info.no.ToString() +
              ",player_no:" + PublicData.ins.self_user.no.ToString() + ","
                , (string msg) =>
            {
                HashTable kv = Json.Decode(msg);
                if ("ok" == kv["ret"])
                {
                    ui.Hide();
                    EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "退出队伍成功" + kv["msg"]);
                }
            });
        }
        else if (type == Events.ID_TOWN_TEAM_BTN_OK)
        {

        }
        //---------------------------RPC 
        else if (type == Events.ID_TOWN_TEAM_RPC_ENTERTEAM)
        {// rpc 进入房间
            HashTable kv = userData as HashTable;
            string name = kv["name"];
            int no = kv.GetInt("no");
            info.AddPlayer(no, name);
            ui.Sync(info);
        }
        else if (type == Events.ID_TOWN_TEAM_RPC_LEAVETEAM)
        {// rpc 房间
            HashTable kv = userData as HashTable;
            if (info.IsSelfCaptain())
            {//队长

                EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "队员离开了队伍");
                info.RemoveTeam(kv.GetInt("no"));
                info.GetCaptain().Reset();
                ui.Sync(info);
            }
            else
            {//队员
                EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "队长离开队伍，队伍已解散");
                ui.ShowPVEPanel();
            }
        }
        else if (type == Events.ID_TOWN_TEAM_RPC_CHANGESKILL)
        {// rpc  
            HashTable kv = userData as HashTable;
            info.GetPlayer(kv.GetInt("no")).ChangeSkillGroup();
            ui.Sync(info);
        }
        else if (type == Events.ID_TOWN_TEAM_RPC_OK)
        {// rpc  

            HashTable kv = userData as HashTable;
            info.GetPlayer(kv.GetInt("no")).ChangeState();
            ui.Sync(info);
        }
        else if (type == Events.ID_TOWN_TEAM_RPC_STARTGAME)
        {// rpc  

            //code copy from TownPVPApp

            PublicData.ins.battle_mode = "pve";
            PublicData.ins.is_pve = true;

            string str = userData as string;
            HashTable kv = Json.Decode(str);


            if (kv["p2"] == HeroMgr.ins.self.no.ToString())
            {//第二个发起匹配的为 
                //暂时用 好友对战一套
                PublicData.ins.is_pvp_friend_owner = false;
            }
            //接受成功 自动连接战斗服
            string room_id = kv["pvproom_id"];
            string max = kv["max_no"];
            string mode = kv["mode"];

            PublicData.ins.pvp_room_max = max;
            PublicData.ins.pvp_room_no = room_id;
            PublicData.ins.user_pvp_other = DAO.User.Create(kv);

            PublicData.ins.is_pvp_friend = true;
            AppMgr.GetCurrentApp<TownApp>().Dispose();


            if (mode == "pve")
            {
                PublicData.ins.is_pve = true;

                SceneMgr.Load("BattlePVE25D");//BattlePVE
            }
            else
            {
                PublicData.ins.is_pve = false;

                SceneMgr.Load("BattlePVP");
            }
        }
        return;

        if (type == Events.ID_TOWN_BTN_BATTLE_ENTER_QUEUE_CLICKED)
        {// open
            //允许移动 但不允许点击其他APP
            if (p.isOneCellAppShowLock) return;
            p.isOneCellAppShowLock = true;


            RpcClient.ins.SendRequest("services.battle", "request_pvp_ramdon_enter_queue_v2",
                "no:" + PublicData.ins.self_user.no.ToString() + "," + "mode:" + PublicData.ins.battle_mode + ",", (string msg) =>
                {
                    UI_townpvpapp view = userData as UI_townpvpapp;
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
        else if (type == Events.ID_TOWN_BTN_BATTLE_LEAVE_QUEUE_CLICKED)
        { // close 
            UI_townpvpapp view = userData as UI_townpvpapp;
            RpcClient.ins.SendRequest("services.battle", "request_pvp_ramdon_leave_queue_v2",
           "no:" + PublicData.ins.self_user.no.ToString() + "," + "mode:" + PublicData.ins.battle_mode + ",", (string msg) =>
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
            string mode = kv["mode"];

            PublicData.ins.pvp_room_max = max;
            PublicData.ins.pvp_room_no = room_id;
            PublicData.ins.user_pvp_other = DAO.User.Create(kv);

            PublicData.ins.is_pvp_friend = true;
            AppMgr.GetCurrentApp<TownApp>().Dispose();


            if (mode == "pve")
            {
                PublicData.ins.is_pve = true;

                SceneMgr.Load("BattlePVE25D");//BattlePVE
            }
            else
            {
                PublicData.ins.is_pve = false;

                SceneMgr.Load("BattlePVP");

            }
        }
        else if (type == Events.ID_TOWN_BTN_PVE_RAMDON_QUEUE_CLICKED)
        {
            //pve 组队
            Debug.Log("pve");
            PublicData.ins.battle_mode = "pve";
            PublicData.ins.is_pve = true;
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_BTN_BATTLE_ENTER_QUEUE_CLICKED, userData);//显示PVP流程

        }
        else if (type == Events.ID_TOWN_BTN_PVP_RAMDON_QUEUE_CLICKED)
        {
            //pve 组队
            Debug.Log("pvp");
            PublicData.ins.battle_mode = "pvp";
            PublicData.ins.is_pve = false;
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_BTN_BATTLE_ENTER_QUEUE_CLICKED, userData);//显示PVP流程
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
