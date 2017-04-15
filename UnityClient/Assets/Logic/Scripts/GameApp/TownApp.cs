/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
public class TownApp : AppBase
{
    public override string GetAppName()
    {
        return "TownApp";
    }

    private bool init = false;
    // Use this for initialization

    private void InitCustomAsync()
    {
        EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
        {
            this.AddCellApp<WorldChatApp>();
            this.AddCellApp<TownMenuApp>();
            this.AddCellApp<FriendsApp>();
            this.AddCellApp<TownPVPApp>();
            this.AddCellApp<TrumpApp>();
            this.AddCellApp<TrumpLevelApp>();
            this.AddCellApp<BackpackApp>();
            this.AddCellApp<SettingsApp>();
            this.AddCellApp<CharacterInfoApp>();
            this.AddCellApp<AfficheApp>();
            this.AddCellApp<EquipSystemApp>();
            EventDispatcher.ins.PostEvent(Events.ID_LOADING_SHOW);
            return "";
        }));


        EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
        {

            Utils.SetTargetFPS(40);


            this.worldMap = ModelMgr.Create<LogicWorldMap>();

            string seed = (new System.Random(Convert.ToInt32((DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds))).Next(500).ToString();


            if (PublicData.GetInstance().self_user == null)
            {
                //test case   --------------------------------------
                string str = "account:1,pwd:1,";


                RpcClient.ins.SendRequest("services.login", "login", str, (string ree) =>
                {
                    HashTable kv = Json.Decode(ree);
                    Debug.Log("login " + ree);

                    if (kv["ret"] == "ok")
                    {
                        DAO.User user = DAO.User.Create(kv);
                        PublicData.GetInstance().self_user = user;


                        RpcClient.ins.SendRequest("services.room", "enter_room", user.ToJson(), (string msg) =>
                        {
                            if (msg == "")
                            {
                                Debug.Log("enter error");
                            }
                            else
                            {
                                EventDispatcher.ins.PostEvent(Events.ID_LOADING_HIDE);

                            }
                        });

                    }

                });
            }

            else
            {
                //test case   -------------end-------------------------

                var user = PublicData.GetInstance().self_user;


                RpcClient.ins.SendRequest("services.room", "enter_room", user.ToJson(), (string msg) =>
                {
                    if (msg == "")
                    {
                        Debug.Log("enter error");
                    }
                    else
                    {
                        EventDispatcher.ins.PostEvent(Events.ID_LOADING_HIDE);

                    }
                });

            }
            return "本地资源加载完成，连接服务器...";
        }));

    }
    public override bool Init()
    {
        if (init) return true;
        init = true;
        base.Init();

        Utils.SetTargetFPS(0xffffff);
        ViewUI.Create<UIPublicRoot>();


        for (int i = 0; i < 10; i++)
        {
            EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
            {
                return DATA.ADD_STRING + "加载数据";
            }));
        }

        EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
        {
            ViewUI.Create<UITownRoot>();

            this.InitCustomAsync();
            return DATA.ADD_STRING + "加载主场景资源";
        }));
        //init all robot
        int robot_count = UnityEngine.Random.Range(5, 10);
        for (int i = 1; i <= robot_count; i++)
        {
            EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
            {
                BaseHero.CreateRobot();
                return DATA.ADD_STRING + "加载主场景Object";
            }));
        }
        /* for (int i = 0; i < 10; i++)
         {
             EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
             {
                 return "";
             }));
         }*/

        return true;
    }
    public override void OnEnter()
    {
        base.OnEnter();

    }


    public override void OnDispose()
    {

        AppMgr.ins.Dispose();
        ModelMgr.ins.Dispose();
        ViewMgr.ins.Dispose();

        //  PublicData.GetInstance().game.Terminate();
        //   GameObject.DestroyImmediate(GameObject.Find("_ServiceCenterObject"));
        //  PublicData.GetInstance().game.GotoScene();

        EventDispatcher.DestroyInstance();
        base.OnDispose();
    }

    /// <summary>
    /// 
    /// </summary>
    bool is_pvp_ok = false;
    bool is_pvp_wait = false;


    public override void UpdateMS()
    {
        AutoReleasePool.ins.Clear();

        if (this.IsInValid()) return; ;

        base.UpdateMS();

        ModelMgr.ins.UpdateMS();
        ViewMgr.ins.Update();
        ViewMgr.ins.UpdateMS();

        if (HeroMgr.ins.self == null) return;

        //    foreach (BaseHero hero in robots)
        {

        }
        if (Input.GetMouseButtonUp(0) && enable_newposition)
        {
            if ((Input.mousePosition.x > unclicked_xy.x && Input.mousePosition.y > unclicked_xy.y) && ((Input.mousePosition.x < unclicked_xy.x + unclicked_wh.x && Input.mousePosition.y < unclicked_xy.y + unclicked_wh.y)))
            {//限制范围内

            }
            else
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (pos.y <= 2.4f)
                {
                    this.PostSelfNewPosition(pos);

                    ///本地直接生效 不需要服务器验证
                    HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent(Events.ID_LOGIC_NEW_POSITION, new Vector2(pos.x, pos.y));
                }
            }
        }
        if (tick.Tick() == false)
        {
            tick.Reset();
            this.PostAlive();
        }
    }

    private void PostSelfNewPosition(Vector3 pos_world)
    {

        RpcClient.ins.SendRequest("services.room", "new_position", "no:" + HeroMgr.ins.self.no + ",x:" + pos_world.x.ToString() +
             ",y:" + pos_world.y.ToString() + ",name:" + HeroMgr.ins.self.name + ",", (string msg) =>
             {
                 if (msg == "timeout")
                 {
                     // Debug.Log("  time out ");
                 }
                 else
                 {
                     //Debug.Log(" new postion ok ");
                 }
             });
    }

    private void PostAlive()
    {
        RpcClient.ins.SendRequest("services.room", "ckeck_alive", "no:" + HeroMgr.ins.self.no + ",");
    }
    public void ResetTick()
    {
        this.tick.Reset();
    }

    public void SetUnClickAbleRange(Vector2 xy, Vector2 wh)
    {
        this.unclicked_xy = xy;
        this.unclicked_wh = wh;
    }

    Vector2 unclicked_xy = new Vector2(0, 0);
    Vector2 unclicked_wh = new Vector2(0, 0);

    public void SetNewPositionAble(bool enable)
    {
        this.enable_newposition = enable;
    }

    private bool enable_newposition = true;
    Counter tick = Counter.Create(DATA.HERO_ALIVE_TICK);//tick for alive  30s
    public bool isOneCellAppShowLock = false;// 小app 的呼出按钮可用否

}
