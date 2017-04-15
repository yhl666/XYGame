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
    private void HidePanel()
    {
        this.PopOut(this.panel);
        img_half.gameObject.SetActive(false);
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
        this.input_name  = GameObject.Find("input_name");

        this.btn_login = GameObject.Find("btn_login").GetComponent<Button>();
        this.btn_register = GameObject.Find("btn_register").GetComponent<Button>();
        this.btn_close = GameObject.Find("btn_close").GetComponent<Button>();
        this.btn_close1 = GameObject.Find("btn_close1").GetComponent<Button>();
        this.btn_ok = GameObject.Find("btn_ok").GetComponent<Button>();

 

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


    Button btn_register;
    Button btn_login;

    Button btn_ok;
    Button btn_close;
    Image img_half;
    Button btn_close1;
    GameObject input_name;
    GameObject panel;

}



