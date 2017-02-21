﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Threading;

public sealed class UIPublicRoot : ViewUI
{

    public override void Update()
    {
        base.Update();
    }

    public override bool Init()
    {
        base.Init();

        this._ui_root = PrefabsMgr.Load(DATA.UI_PREFABS_FILE_PUBLIC);

        this._ui_child.Add(ViewUI.Create<UI_loading>(this));

        EventDispatcher.ins.PostEvent(Events.ID_ADD_ASYNC, new Func<string>(() =>
            {
                this._ui_child.Add(ViewUI.Create<UI_wait>(this));
                this._ui_child.Add(ViewUI.Create<UI_pushmsg>(this));
                this._ui_child.Add(ViewUI.Create<UI_globaldialog>(this));


                return DATA.EMPTY_STRING;
            }));


        return true;
    }

    ArrayList _ui_child = new ArrayList();

}

/// <summary>
///  this is base ui and driverother loading async task
/// </summary>
public sealed class UI_loading : ViewUI
{
    //游戏加载界面
    public override void OnEvent(string type, object userData)
    {
        switch (type)
        {
            case "loading":
                {
                    this.OnEvent(Events.ID_LOADING_NOW, userData);
                } break;
            case "loadingOK":
                {
                    this.OnEvent(Events.ID_LOADING_OK, userData);
                } break;
            case "addAsync":
                {
                    this.OnEvent(Events.ID_ADD_ASYNC, userData);
                } break;
        }
    }


    public override void OnEvent(int type, object userData)
    {
        switch (type)
        {
            case Events.ID_LOADING_NOW:
                {
                    if (this._enable == false)
                    {
                        this._enable = true;
                        this._ui.SetActive(_enable);
                    }

                } break;
            case Events.ID_LOADING_OK:
                {
                    this._enable = false;
                    if (shouldHideWhenComplete) { this._ui.SetActive(_enable); }
                } break;

            case Events.ID_ADD_ASYNC:
                {
                    funcs.Enqueue(userData);
                } break;
            case Events.ID_LOADING_SHOW:
                {
                    if (userData != null)
                    {
                        this.txt.text = userData as string;

                    }
                    this.shouldHideWhenComplete = false;
                    this._ui.SetActive(true);
                } break;
            case Events.ID_LOADING_HIDE:
                {
                    this.shouldHideWhenComplete = true;
                    this._ui.SetActive(false);
                } break;

            case Events.ID_LOADING_SYNC_STRING:
                {
                    this.txt.text = userData as string;
                } break;
        }


    }


    public override void Update()
    {

        if (funcs.Count > 0)
        {
            if (MAX_COUNT < funcs.Count)
            {
                MAX_COUNT = funcs.Count;
            }

            // Thread.Sleep(250);


            this._enable = true;
            this._ui.SetActive(_enable);

            Func<string> SyncFunc = funcs.Dequeue() as Func<string>;
            int percent = (MAX_COUNT - funcs.Count) * 100 / MAX_COUNT;

            //TODO here can invoke work in other thead
            // will consider some func can only run in  UnityEngine main thread ?
            string ret = SyncFunc();
            ii++;
            if (ret == DATA.EMPTY_STRING)
            {
                this.txt.text = string.Format(DATA.UI_LOADING, percent.ToString(), ii.ToString());
            }
            else
            {
                this.txt.text = ret;
            }
            this.img.transform.localScale = new Vector3(1.0f, percent / 100.0f, 1.0f);


            if (funcs.Count <= 0)
            {
                this._enable = false;
                if (shouldHideWhenComplete) { this._ui.SetActive(_enable); }

            }
        }

    }

    public override bool Init()
    {
        base.Init();

        this._ui = GameObject.Find("loading");
        this.txt = GameObject.Find("load_txt").GetComponent<Text>();
        this.img = GameObject.Find("load_img").GetComponent<Image>();



        EventDispatcher.ins.AddEventListener(this, "loading");
        EventDispatcher.ins.AddEventListener(this, "loadingOK");
        EventDispatcher.ins.AddEventListener(this, "addAsync");

        EventDispatcher.ins.AddEventListener(this, Events.ID_ADD_ASYNC);
        EventDispatcher.ins.AddEventListener(this, Events.ID_LOADING_NOW);
        EventDispatcher.ins.AddEventListener(this, Events.ID_LOADING_OK);
        EventDispatcher.ins.AddEventListener(this, Events.ID_LOADING_SHOW);
        EventDispatcher.ins.AddEventListener(this, Events.ID_LOADING_HIDE);
        EventDispatcher.ins.AddEventListener(this, Events.ID_LOADING_SYNC_STRING);


        this._ui.SetActive(_enable);
        return true;
    }


    private Queue funcs = new Queue();
    private Text txt;
    private Image img;
    private int ii = 0;
    private bool shouldHideWhenComplete = true;
    private int MAX_COUNT = 0;

}



public sealed class UI_pushmsg : ViewUI
{


    public override void Update()
    {
        base.Update();

        if (tick.Tick()) return;


        this.img_bg.SetActive(false);

    }
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_PUBLIC_PUSH_MSG)
        {
            this.img_bg.SetActive(true);
            tick.Reset();
            string str = userData as string;
            if (str.Length > 30)
            {

                this.txt.text = (userData as string).Substring(0, 30);
            }
            else
            {
                this.txt.text = str;
            }
        }

    }
    public override bool Init()
    {
        base.Init();

        this.txt = GameObject.Find("txt_pushmsg").GetComponent<Text>();
        this.img_bg = GameObject.Find("img_pushmsg");


        EventDispatcher.ins.AddEventListener(this, Events.ID_PUBLIC_PUSH_MSG);
        this.img_bg.SetActive(false);
        return true;
    }

    private Text txt = null;
    private GameObject img_bg = null;
    Counter tick = Counter.Create(120);

}


public sealed class UI_wait : ViewUI
{//进入游戏后 网络等待界面

    public override void OnEvent(int type, object userData)
    {
        /*  string str;

          if (userData == null)
          {
              str = DATA.UI_WAIT_INFO_DEFAULT;
          }
          else
          {
              str = userData as string;
          }
          switch (type)
          {
              case Events.ID_UI_WAIT:
                  {
                      this._enable = true;

                  } break;
              case Events.ID_UI_NOWAIT:
                  {
                      this._enable = false;
                  } break;

          }
          this._ui.SetActive(_enable);
          this.txt_info.text = str;*/
    }

    public override void Update()
    {



    }

    public override bool Init()
    {
        base.Init();

        /*  this._ui = GameObject.Find("img_wait");
          this.txt_info = GameObject.Find("wait_info").GetComponent<Text>();
          EventDispatcher.ins.AddEventListener(this, Events.ID_UI_NOWAIT);
          EventDispatcher.ins.AddEventListener(this, Events.ID_UI_WAIT);
          */
        //  this._ui.SetActive(_enable);
        return true;
    }

    private Text txt_info;

}


/*
public sealed class UI_push_msg : ViewUI
{

    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_PUSH_MSG)
        {

            this.txt_info.text = userData as string;

            ResetCounter();
        }
    }

    public override void UpdateMS()
    {
        CounterTick();
    }

    public override bool Init()
    {
        base.Init();


        this.txt_info = GameObject.Find("push_msg").GetComponent<Text>();
        EventDispatcher.ins.AddEventListener(this, Events.ID_PUSH_MSG);

        this.txt_info.gameObject.SetActive(false);
        return true;
    }
    private void ResetCounter()
    {
        this.txt_info.gameObject.SetActive(true);
        counter = 0;
    }
    private void CounterTick()
    {
        counter++;
        if (counter > 80)
        {
            this.txt_info.gameObject.SetActive(false);
        }
    }

    private Text txt_info;
    private int counter = 0;
}


*/





public sealed class GlobalDialogInfo
{
    public VoidFuncVoid _OnYes = null;
    public VoidFuncVoid _OnNo = null;
    public int timeout = 0xfffffff;
    public string txt_info = "";
    public string txt_title = "提醒";

    public string btn_yes_txt = "是";
    public string btn_no_txt = "否";

}


public sealed class UI_globaldialog : ViewUI
{


    public override void Update()
    {
        base.Update();



    }
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_PUBLIC_GLOBALDIALOG_SHOW)
        {
            this.info = userData as GlobalDialogInfo;
            this.PopIn(this.panel);

            this.txt_info.text = info.txt_info;
            this.txt_title.text = info.txt_title;

            this.txt_btn_no.text = info.btn_no_txt;
            this.txt_btn_yes.text = info.btn_yes_txt;
        }

    }
    public override bool Init()
    {
        base.Init();

        this.panel = GameObject.Find("ui_panel_globaldialog");
        this.btn_yes = panel.transform.FindChild("btn_yes").GetComponent<Button>();
        this.btn_no = panel.transform.FindChild("btn_no").GetComponent<Button>();
        this.txt_info = panel.transform.FindChild("txt_msg").GetComponent<Text>();
        this.txt_title = panel.transform.FindChild("txt_title").GetComponent<Text>();

        this.txt_btn_yes = btn_yes.transform.FindChild("txt").GetComponent<Text>();
        this.txt_btn_no = btn_yes.transform.FindChild("txt").GetComponent<Text>();
    

        this.btn_yes.onClick.AddListener(() =>
        {
            this.ClickYes();
            this.info = null;
        });
        this.btn_no.onClick.AddListener(() =>
        {
            this.ClickNo();
            this.info = null;
        });


        EventDispatcher.ins.AddEventListener(this, Events.ID_PUBLIC_GLOBALDIALOG_SHOW);
        this.panel.SetActive(false);
        return true;
    }

    private void ClickYes()
    {
        this.PopOut(this.panel);
        if (info == null) return;
        if (info._OnYes == null) return;
        info._OnYes();
    }
    private void ClickNo()
    {
        this.PopOut(this.panel);
        if (info == null) return;
        if (info._OnNo == null) return;
        info._OnNo();
    }

    private Text txt_info = null;
    private Text txt_title = null;

    private Text txt_btn_yes = null;
    private Text txt_btn_no = null;

    private Button btn_yes = null;
    private Button btn_no = null;
    private GameObject panel = null;

    private GlobalDialogInfo info = null;

}

