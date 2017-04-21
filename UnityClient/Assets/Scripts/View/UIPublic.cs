/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
                this._ui_child.Add(ViewUI.Create<UI_console>(this));
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
            else if (ret.IndexOf(DATA.ADD_STRING) >= 0)
            {
                this.txt.text = string.Format(DATA.UI_LOADING, percent.ToString(), ii.ToString()) + ret.Replace(DATA.ADD_STRING, "");
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



public sealed class UI_console : ViewUI
{//UI调试界面 

    class DebugInfo
    {
        public int index = 0;
        public string what;
        public LogType type;
        public string stack_trace;
        public GameObject obj;
        public static DebugInfo Create(string what, LogType type, string stack_trace, int index, GameObject obj = null)
        {
            DebugInfo ret = new DebugInfo();
            ret.what = what;
            ret.index = index;
            ret.obj = obj;
            ret.type = type;
            ret.stack_trace = stack_trace;
            return ret;
        }
        public void Dispose()
        {
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
            obj = null;
            what = null;
            stack_trace = null;
        }
    }
    public override void OnEvent(int type, object userData)
    {

    }

    public override void Update()
    {
    }
    public override void OnDispose()
    {
        base.OnDispose();
        Application.logMessageReceived -= PushLog;

    }
    private void ShowConsole()
    {
        scroll.verticalScrollbar.value = 0;
        this.panel.SetActive(true);
        this.btn_show.gameObject.SetActive(false);
    }
    private void HideConsole()
    {
        this.panel.SetActive(false);
        this.btn_show.gameObject.SetActive(true);
    }
    public override bool Init()
    {
        base.Init();
        this._ui = GameObject.Find("ui_panel_console");
        if(Config.DEBUG_EnableDebugWindow==false)
        {
            this._ui.SetActive(false);
            return true;
        }
        this.panel = this._ui.transform.FindChild("ui_panel").gameObject;
        this.content = this.panel.transform.FindChild("scrollview/Viewport/Content").gameObject;

        this.template = this.panel.transform.FindChild("scrollview/Viewport/Content/one").gameObject;
        this.template.SetActive(false);
        this.scroll = this.panel.transform.FindChild("scrollview").GetComponent<ScrollRect>();

        this.txt_all_info = this.panel.transform.FindChild("txt_all_info").GetComponent<Text>();
        this.btn_clear = this.panel.transform.FindChild("btn_clear").GetComponent<Button>();
        this.btn_close = this.panel.transform.FindChild("btn_close").GetComponent<Button>();
        this.btn_clearstack = this.panel.transform.FindChild("btn_clearstack").GetComponent<Button>();
        this.txt_stacktrace = this.panel.transform.FindChild("txt_stacktrace").GetComponent<Text>();
        this.btn_show = this._ui.transform.FindChild("btn_show").GetComponent<Button>();

        Application.logMessageReceived += PushLog;

        this.btn_clear.onClick.AddListener(() =>
        {
            this.Clear();
        });
        this.btn_clearstack.onClick.AddListener(() =>
        {
            this.txt_stacktrace.text = "调用栈:";
        });
        this.btn_show.onClick.AddListener(() =>
        {
            this.ShowConsole();
        });
        this.btn_close.onClick.AddListener(() =>
        {
            this.HideConsole();
        });
        this.HideConsole();
        return true;
    }
    void PushLog(string what, string stackTrace, LogType type)
    {
        ++current_index;
        this.SyncOne(DebugInfo.Create(what, type, stackTrace, current_index));
    }
    int current_debuginfo_index = 0;

    private void SyncAll()
    {
        this.content.GetComponent<RectTransform>().sizeDelta = new Vector2(100, list_debuginfo.Count * 35 + 100);
        scroll.verticalScrollbar.value = 0;
        txt_all_info.text = " Log:" + count_log + " Error:" + count_error + "  Warning:" + count_warning + "  Exception:" + count_exception;
    }
    private void SyncOne(DebugInfo info)
    {
        if (Config.DEBUG_EnableAutoClean && list_debuginfo.Count > Config.DEBUG_MaxCount)
        {//TODO 考虑复用Node
            DebugInfo first = list_debuginfo.First.Value;
            list_debuginfo.RemoveFirst();
            first.Dispose();
        }
        list_debuginfo.AddLast(info);
        string what = info.what;
        LogType type = info.type;
        string stackTrace = info.stack_trace;

        string text = "";

        if (type == LogType.Log)
        {
            text = info.index + " [Log]:" + what;
        }
        else if (type == LogType.Error)
        {
            text = info.index + "[Error]:" + what;
        }
        else if (type == LogType.Warning)
        {
            text = info.index + " [Warning]:" + what;
        }
        else if (type == LogType.Exception)
        {
            text = info.index + " [Exception]:" + what;
        }
        Color color = Color.white;
        if (type == LogType.Log)
        {
            color = Color.white;
            ++count_log;
        }
        else if (type == LogType.Error)
        {
            color = Color.red;
            ++count_error;
        }
        else if (type == LogType.Exception)
        {
            color = Color.red;
            ++count_exception;
        }
        else if (type == LogType.Warning)
        {
            color = Color.yellow;
            ++count_warning;
        }
        GameObject obj = GameObject.Instantiate(this.template, this.content.transform) as GameObject;
        obj.SetActive(true);
        info.obj = obj;
        Text txt = obj.transform.FindChild("what").GetComponent<Text>();
        txt.text = text;
        txt.color = color;
        Button btn = obj.GetComponentInChildren<Button>();
        if (type == LogType.Error || type == LogType.Exception)
        {
            txt_stacktrace.text = "调用栈: " + info.index + "\n" + info.stack_trace;
        }
        else
        {
            txt_stacktrace.text = "调用栈:";
        }
        btn.onClick.AddListener(() =>
        {
            txt_stacktrace.text = "调用栈: " + info.index + "\n" + info.stack_trace;  //stackTrace;
        });

        int i = 0;
        foreach (DebugInfo info1 in list_debuginfo)
        {
            GameObject one = info1.obj;
            one.transform.localPosition = new Vector3(one.transform.localPosition.x, -140 - i * 30, one.transform.localPosition.z);
            i++;
        }
        this.content.GetComponent<RectTransform>().sizeDelta = new Vector2(100, list_debuginfo.Count * 30 + 100);

        scroll.verticalScrollbar.value = 0;

        txt_all_info.text = "Total:  Log:" + count_log + " Error:" + count_error + "  Warning:" + count_warning + "  Exception:" + count_exception;
    }
    private void Clear()
    {
        count_log = 0;
        count_error = 0;
        count_exception = 0;
        count_warning = 0;
        txt_stacktrace.text = "调用栈:";
        foreach (DebugInfo info1 in list_debuginfo)
        {
            info1.Dispose();
        }
        list_debuginfo.Clear();
        this.SyncAll();
    }
    GameObject panel = null;
    Text txt_stacktrace;
    Text txt_all_info;
    ScrollRect scroll = null;
    LinkedList<DebugInfo> list_debuginfo = new LinkedList<DebugInfo>();
    // private Text txt_info;
    GameObject template = null;
    private GameObject content = null;
    Button btn_clear;
    Button btn_clearstack;
    Button btn_close;
    Button btn_show;
    int current_index = 0;
    int count_log = 0;
    int count_error = 0;
    int count_exception = 0;
    int count_warning = 0;

}

