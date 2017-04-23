/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


public sealed class UILoginRoot : ViewUI
{

    public override void Update()
    {
        base.Update();
    }

    public override bool Init()
    {
        base.Init();

        // this._ui_root = PrefabsMgr.Load(DATA.UI_PREFABS_FILE_BATTLE);
        //   this._ui_root = GameObject.Find("UI_Login");

        this._ui_root = PrefabsMgr.Load(DATA.UI_PREFABS_FILE_LOGIN);

        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_login>(this));
            return DATA.EMPTY_STRING;
        }));

        return true;
    }

    public override void OnExit()
    {

        UnityEngine.Object.Destroy(this._ui_root);

        base.OnExit();
    }
    protected ArrayList _ui_child = new ArrayList();

}



enum LoginState
{
    Login,
    Register,
    None,
}

public sealed class UI_login : ViewUI
{

    LoginState state = LoginState.None;
    public override void Update()
    {
        base.Update();

    }

    public override void OnEvent(int type, object userData)
    {
        this.txt.text = userData as string;
    }
    private void ShowLogin()
    {
        img_half.gameObject.SetActive(true);
        this.PopIn(this.panel);
        txt.text = "登录";
        txt_btn_ok.text = "确定";
        input_name.SetActive(false);
    }
    private void ShowRegister()
    {
        img_half.gameObject.SetActive(true);
        this.PopIn(this.panel);
        txt.text = "注册";
        txt_btn_ok.text = "确定";
        input_name.SetActive(true);
    }
    private void SyncServer()
    {
        RpcClient.ins.Disconnect();
        RpcClient.ins.ReConnect();
        txt_server.text =
             "战斗服务器IP:" + Config.LOGIC_SERVER_IP + ":" + Config.LOGIC_SERVER_PORT + "   " +
            "逻辑服务器IP:" + Config.SERVER_IP + ":" + Config.SERVER_PORT;
    }
    private void HidePanel()
    {
        this.PopOut(this.panel);
        img_half.gameObject.SetActive(false);
    }
    private void SyncConsole()
    {
        if (Config.DEBUG_EnableDebugWindow)
        {
            this.txt_console.text = "关闭Console";
        }
        else
        {
            this.txt_console.text = "开启Console";
        }
    }
    public override bool Init()
    {
        base.Init();

        this.txt = GameObject.Find("txt_info").GetComponent<Text>();
        this.txt_btn_ok = GameObject.Find("txt_btn_ok").GetComponent<Text>();
        this.img_half = GameObject.Find("img_half").GetComponent<Image>();

        this.txt_account = GameObject.Find("txt_account").GetComponent<Text>();
        this.txt_name = GameObject.Find("txt_name").GetComponent<Text>();
        this.txt_pwd = GameObject.Find("txt_pwd").GetComponent<Text>();

        this.panel = GameObject.Find("ui_panel_login");
        this.input_name = GameObject.Find("input_name");
        this.txt_console = GameObject.Find("txt_console").GetComponent<Text>();

        this.btn_login = GameObject.Find("btn_login").GetComponent<Button>();
        this.btn_register = GameObject.Find("btn_register").GetComponent<Button>();
        this.btn_close = GameObject.Find("btn_close").GetComponent<Button>();
        this.btn_close1 = GameObject.Find("btn_close1").GetComponent<Button>();
        this.btn_ok = GameObject.Find("btn_ok").GetComponent<Button>();
        this.btn_remote = GameObject.Find("btn_remote").GetComponent<Button>();
        this.btn_local = GameObject.Find("btn_local").GetComponent<Button>();
        this.btn_console = GameObject.Find("btn_console").GetComponent<Button>();
        if (Config.DEBUG_LoadDebugWindow == false)
        {
            this.btn_console.gameObject.SetActive(false);
        }

        this.btn_console.onClick.AddListener(delegate()
        {
            Config.DEBUG_EnableDebugWindow = !Config.DEBUG_EnableDebugWindow;
            this.SyncConsole();
        });
        this.SyncConsole();
        this.txt_server = GameObject.Find("txt_ip").GetComponent<Text>();
        this.SyncServer();
        this.btn_local.onClick.AddListener(delegate()
        {
            Config.SERVER_IP = "127.0.0.1";
            Config.LOGIC_SERVER_IP = "127.0.0.1";
            this.SyncServer();
        });
        this.btn_remote.onClick.AddListener(delegate()
         {
             Config.SERVER_IP = "115.159.203.16";
             Config.LOGIC_SERVER_IP = "115.159.203.16";
             this.SyncServer();
         });
        this.btn_login.onClick.AddListener(delegate()
        {
            this.state = LoginState.Login;
            this.ShowLogin();
        });
        this.btn_register.onClick.AddListener(delegate()
        {
            this.state = LoginState.Register;
            this.ShowRegister();
        });
        this.btn_close.onClick.AddListener(delegate()
        {
            this.state = LoginState.None;
            this.HidePanel();
        });
        this.btn_close1.onClick.AddListener(delegate()
        {
            this.state = LoginState.None;
            this.HidePanel();
        });
        this.btn_ok.onClick.AddListener(delegate()
        {
            LoginInfo info = new LoginInfo();

            info.name = txt_name.text;
            info.account = txt_account.text;
            info.pwd = txt_pwd.text;

            if (info.account == "")
            {
                EventDispatcher.ins.PostEvent(Events.ID_LOGIN_STATUS, "请输入账户"); return;
            }
            if (info.pwd == "")
            {
                EventDispatcher.ins.PostEvent(Events.ID_LOGIN_STATUS, "请输入密码"); return;
            }
            if (this.state == LoginState.Register && info.name == "")
            {
                EventDispatcher.ins.PostEvent(Events.ID_LOGIN_STATUS, "请输入昵称"); return;
            }
            if (this.state == LoginState.Register)
            {
                EventDispatcher.ins.PostEvent(Events.ID_BTN_REGISTER, info);
            }
            else if (this.state == LoginState.Login)
            {
                EventDispatcher.ins.PostEvent(Events.ID_BTN_LOGIN, info);
            }
        });





        /* this.btn_login.onClick.AddListener(delegate()
         {

             LoginInfo info = new LoginInfo();

             info.name = txt_name.text;
             info.account = txt_account.text;
             info.pwd = txt_pwd.text;

             if (info.account == "")
             {
                 EventDispatcher.ins.PostEvent(Events.ID_LOGIN_STATUS, "请输入账户"); return;
             }

             if (info.pwd == "")
             {
                 EventDispatcher.ins.PostEvent(Events.ID_LOGIN_STATUS, "请输入密码"); return;
             }


             EventDispatcher.ins.PostEvent(Events.ID_BTN_LOGIN, info);

         });
         this.btn_register.onClick.AddListener(delegate()
         {
             LoginInfo info = new LoginInfo();

             info.name = txt_name.text;
             info.account = txt_account.text;
             info.pwd = txt_pwd.text;

             if (info.account == "")
             {
                 EventDispatcher.ins.PostEvent(Events.ID_LOGIN_STATUS, "请输入账户"); return;
             }

             if (info.pwd == "")
             {
                 EventDispatcher.ins.PostEvent(Events.ID_LOGIN_STATUS, "请输入密码"); return;
             }


             if (info.name == "")
             {
                 EventDispatcher.ins.PostEvent(Events.ID_LOGIN_STATUS, "请输入昵称"); return;
             }

             EventDispatcher.ins.PostEvent(Events.ID_BTN_REGISTER, info);

         });*/

        EventDispatcher.ins.AddEventListener(this, Events.ID_LOGIN_STATUS);
        this.panel.SetActive(false);
        img_half.gameObject.SetActive(false);
        this.panel.transform.localScale = new Vector3(0f, 0f, 1f);
        return true;
    }
    Text txt_btn_ok;
    Text txt;
    Text txt_name;
    Text txt_pwd;
    Text txt_account;
    Text txt_server;
    Text txt_console;

    Button btn_register;
    Button btn_login;

    Button btn_ok;
    Button btn_close;
    Image img_half;
    Button btn_close1;
    GameObject input_name;
    GameObject panel;

    Button btn_remote;
    Button btn_local;
    Button btn_console;
}



