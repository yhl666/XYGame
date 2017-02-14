﻿using UnityEngine;
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

    public override bool Init()
    {
        if (init) return true;
        init = true;
        base.Init();


        ViewUI.Create<UIPublicRoot>();

        for (int i = 0; i < 10; i++)
        {
            EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
            {
                return "";
            }));

        }

        EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
        {
            ViewUI.Create<UITownRoot>();


            this.AddCellApp<TownWorldChat>();

            EventDispatcher.ins.PostEvent(Events.ID_LOADING_SHOW);
            Application.targetFrameRate = 40;



            this.worldMap = ModelMgr.Create<LogicWorldMap>();


            string seed = (new System.Random(Convert.ToInt32((DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds))).Next(500).ToString();

            string p = "name:" + PublicData.GetInstance().self_name + "," + "no:" + PublicData.GetInstance().self_no + ",";
            Debug.Log(p);


            if (PublicData.GetInstance().self_name == "")
            {
                //test case   --------------------------------------
                string str = "account:1,pwd:1,no:1,name:0001,";

                PublicData.GetInstance().self_name = "0001";
                PublicData.GetInstance().self_account = "1";
                PublicData.GetInstance().self_no = "1";


                RpcClient.ins.SendRequest("services.login", "login", str, (string msg11) =>
                {
                    RpcClient.ins.SendRequest("services.room", "enter_room", str, (string msg) =>
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



                });
            }

            else
            {
                //test case   -------------end-------------------------


                RpcClient.ins.SendRequest("services.room", "enter_room", p, (string msg) =>
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








        return true;
    }
    public override void OnEnter()
    {
        base.OnEnter();



    }

    public override void UpdateMS()
    {
        base.UpdateMS();
        AutoReleasePool.ins.Clear();

        ModelMgr.ins.UpdateMS();
        ViewMgr.ins.Update();
        ViewMgr.ins.UpdateMS();


        if (HeroMgr.ins.self == null) return;


        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (pos.y <= 2.4f)
            {
                this.PostSelfNewPosition(pos);

                ///本地直接生效 不需要服务器验证
                HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent(Events.ID_LOGIC_NEW_POSITION, new Vector2(pos.x, pos.y));
            }
        }
        if (tick.Tick() == false)
        {
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
    Counter tick = Counter.Create(DATA.HERO_ALIVE_TICK);//tick for alive  30s
}
