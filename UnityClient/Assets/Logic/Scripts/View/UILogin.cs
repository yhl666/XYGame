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





public sealed class UI_login : ViewUI
{


    public override void Update()
    {
        base.Update();

    }

    public override void OnEvent(int type, object userData)
    {
        this.txt.text = userData as string;
    }
    public override bool Init()
    {
        base.Init();

        this.txt = GameObject.Find("txt").GetComponent<Text>();

        this.txt_account = GameObject.Find("txt_account").GetComponent<Text>();
        this.txt_name = GameObject.Find("txt_name").GetComponent<Text>();
        this.txt_pwd = GameObject.Find("txt_pwd").GetComponent<Text>();

        this.btn_login = GameObject.Find("btn_login").GetComponent<Button>();
        this.btn_register = GameObject.Find("btn_register").GetComponent<Button>();




        this.btn_login.onClick.AddListener(delegate()
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

        });

        EventDispatcher.ins.AddEventListener(this, Events.ID_LOGIN_STATUS);

        return true;
    }

    Text txt;
    Text txt_name;
    Text txt_pwd;
    Text txt_account;


    Button btn_register;
    Button btn_login;
}



