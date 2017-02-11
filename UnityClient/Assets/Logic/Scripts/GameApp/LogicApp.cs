﻿using UnityEngine;
using System.Collections;
using System;
public class LogicApp : AppBase
{
    public static LogicApp ins = null;
    private bool init = false;
    // Use this for initialization

    public LogicApp()
    {
        ins = this;
    }


    public override bool Init()
    {
        if (init) return true;

        base.Init();
        current_app = this;
        Application.targetFrameRate = 40;
        init = true;
        Debug.Log("new App");

        this.worldMap = ModelMgr.Create<BattleWorldMap>();


        string seed = (new System.Random(Convert.ToInt32((DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds))).Next(9999).ToString();


        RpcClient.ins.SendRequest("services.login", "login", "name:" + seed + ",", (string msg) =>
        {
            if (msg == "")
            {
                Debug.Log("error");
            }
            else
            {
                HashTable kv = Json.Decode(msg);
                Debug.Log(" login " + kv["res"]);
            }


        });

        return true;
    }
    BaseHero self = null;
    public override void OnEnter()
    {
        base.OnEnter();



    }
    // Update is called once per frame
    public override void Update()
    {
        ModelMgr.ins.UpdateMS();
        ViewMgr.ins.UpdateMS();
        if (HeroMgr.ins.self == null) return;



        Vector3 pos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            //   Debug.Log(pos.x + "          " + pos.y);
            Vector3 pos_world = Camera.main.ScreenToWorldPoint(pos);


            //    Debug.Log(pos_world.x + "          " + pos_world.y);

            //     ---  BaseHeroMgr.ins.self.eventDispatcher.PostEvent(Events.ID_LOGIC_NEW_POSITION, new Vector2(pos_world.x, pos_world.y));


            RpcClient.ins.SendRequest("services.room", "new_position", "no:" + HeroMgr.ins.self.no + ",x:" + pos_world.x.ToString() +
                 ",y:" + pos_world.y.ToString() + ",", (string msg) =>
            {
                if (msg != "")
                {


                    Debug.Log(" new postion ok ");
                }


            });

        }



    }


}
