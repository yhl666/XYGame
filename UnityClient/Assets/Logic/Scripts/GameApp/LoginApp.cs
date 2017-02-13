using UnityEngine;
using System.Collections;
using UnityEngine;
using System.Collections;
using System;
public class LoginApp : AppBase
{
    public override string GetAppName()
    {
        return "LoginApp";
    }

    private bool init = false;
    // Use this for initialization

    public override bool Init()
    {
        if (init) return true;
        init = true;
        base.Init();


        ViewUI.Create<UIPublicRoot>();

        for (int i = 0; i < 100; i++)
        {
            EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
            {
                return "";
            }));

        }

        EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
        {
            ViewUI.Create<UILoginRoot>();
            Application.targetFrameRate = 40;


            return "";
        }));







        EventDispatcher.ins.AddEventListener(this, Events.ID_BTN_LOGIN);
        EventDispatcher.ins.AddEventListener(this, Events.ID_BTN_REGISTER);

        return true;
    }
    public override void OnEnter()
    {
        base.OnEnter();



    }

    private void doLogin(LoginInfo info)
    {



        for (int i = 0; i < 100; i++)
        {
            EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
            {
                return "验证账户中 请稍后";
            }));
        }

        string str = "account:" + info.account + ",";
        str = str + "pwd:" + info.pwd + ",";

        EventDispatcher.ins.PostEvent(Events.ID_LOADING_SHOW, "验证账户中 请稍后");

        RpcClient.ins.SendRequest("services.login", "login", str, (string msg) =>
        {

            if ("" == msg)
            {
                EventDispatcher.ins.PostEvent(Events.ID_LOADING_HIDE);

                EventDispatcher.ins.PostEvent(Events.ID_LOGIN_STATUS, "连接服务器失败");
                return;
            }

            var kv = Json.Decode(msg);


            if (kv["ret"] == "ok")
            {
                Debug.Log("login ok " + kv["name"]);
                PublicData.GetInstance().self_name = kv["name"];
                PublicData.GetInstance().self_account = kv["account"];
                PublicData.GetInstance().self_no = kv["no"];
                this.processWithLoginOK();
            }
            else
            {
                EventDispatcher.ins.PostEvent(Events.ID_LOADING_HIDE);

                EventDispatcher.ins.PostEvent(Events.ID_LOGIN_STATUS, kv["ret"]);

                Debug.Log(msg);
            }
        });



    }
    private void dpRegister(LoginInfo info)
    {

        string str = "name:" + info.name + ",";
        str = str + "account:" + info.account + ",";
        str = str + "pwd:" + info.pwd + ",";
        for (int i = 0; i < 100; i++)
        {
            EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
            {
                return "注册账户 请稍后";
            }));
        }


        RpcClient.ins.SendRequest("services.login", "register", str, (string msg) =>
        {

            EventDispatcher.ins.PostEvent(Events.ID_LOADING_HIDE);
            if ("" == msg)
            {
                EventDispatcher.ins.PostEvent(Events.ID_LOGIN_STATUS, "连接服务器失败");
                return;
            }
            var kv = Json.Decode(msg);

            if (kv["ret"] == "ok")
            {
                Debug.Log("register ok ");
            }
            else
            {
                EventDispatcher.ins.PostEvent(Events.ID_LOGIN_STATUS, kv["ret"]);

                Debug.Log(msg);
            }
        });


    }

    private void processWithLoginOK()
    {
        EventDispatcher.DestroyInstance();
        AppMgr.ins.Dispose();
        ModelMgr.ins.Dispose();
        ViewMgr.ins.Dispose();

        //  PublicData.GetInstance().game.Terminate();
        //   GameObject.DestroyImmediate(GameObject.Find("_ServiceCenterObject"));
        //  PublicData.GetInstance().game.GotoScene();
        SceneMgr.Load("TownScene");

    }

    public override void OnEvent(int type, object userData)
    {
        LoginInfo info = userData as LoginInfo;

        if (type == Events.ID_BTN_REGISTER)
        {
            this.dpRegister(info);
        }


        if (type == Events.ID_BTN_LOGIN)
        {

            this.doLogin(info);
        }
    }

    public override void UpdateMS()
    {
        AutoReleasePool.ins.Clear();

        ModelMgr.ins.UpdateMS();
        ViewMgr.ins.Update();
        ViewMgr.ins.UpdateMS();



    }

}
