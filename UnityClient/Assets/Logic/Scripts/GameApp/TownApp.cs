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

            EventDispatcher.ins.PostEvent(Events.ID_LOADING_SHOW);
            Application.targetFrameRate = 40;



            this.worldMap = ModelMgr.Create<LogicWorldMap>();


            string seed = (new System.Random(Convert.ToInt32((DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds))).Next(0xfffffff).ToString();


            RpcClient.ins.SendRequest("services.login", "login", "name:" + seed + ",", (string msg) =>
            {
                if (msg == "")
                {
                    Debug.Log("login error");
                }
                else
                {
                    EventDispatcher.ins.PostEvent(Events.ID_LOADING_HIDE);

                }
            });



            return "本地资源加载完成，连接服务器...";
        }));








        return true;
    }
    BaseHero self = null;
    public override void OnEnter()
    {
        base.OnEnter();



    }


    int tick = 0;
    public override void UpdateMS()
    {
        AutoReleasePool.ins.Clear();

        ModelMgr.ins.UpdateMS();
        ViewMgr.ins.Update();
        ViewMgr.ins.UpdateMS();


        tick++;
        if (HeroMgr.ins.self == null) return;

        var random = new System.Random();

        /*if(random.Next(40)==10)
        {
            RpcClient.ins.SendRequest("services.room", "new_position", "no:" + HeroMgr.ins.self.no + ",x:" + random.NextDouble()*24 +
              ",y:" + random.NextDouble() * 2.5 + ",", (string msg) =>
              {
                  if (msg != "")
                  {


                      Debug.Log(" new postion ok ");
                  }


              });
        }*/
        Vector3 pos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            //   Debug.Log(pos.x + "          " + pos.y);
            Vector3 pos_world = Camera.main.ScreenToWorldPoint(pos);


            //    Debug.Log(pos_world.x + "          " + pos_world.y);
            ///本地直接生效 不需要服务器验证
            HeroMgr.ins.self.eventDispatcher.PostEvent(Events.ID_LOGIC_NEW_POSITION, new Vector2(pos_world.x, pos_world.y));


            RpcClient.ins.SendRequest("services.room", "new_position", "no:" + HeroMgr.ins.self.no + ",x:" + pos_world.x.ToString() +
                 ",y:" + pos_world.y.ToString() + ",", (string msg) =>
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



    }


}
