/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


public sealed class UIBattleRoot : ViewUI
{

    public override void Update()
    {
        base.Update();
    }

    public override bool Init()
    {
        base.Init();

        // this._ui_root = PrefabsMgr.Load(DATA.UI_PREFABS_FILE_BATTLE);
        this._ui_root = GameObject.Find("UI_Battle");


        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
            {
                this._ui_child.Add(ViewUI.Create<UI_joystick>(this));
                return DATA.EMPTY_STRING;
            }));



        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_skills>(this));
            return DATA.EMPTY_STRING;
        }));



        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_heroInfo>(this));
            return DATA.EMPTY_STRING;
        }));

        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_xy>(this));
            return DATA.EMPTY_STRING;
        }));



        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_frame>(this));
            return DATA.EMPTY_STRING;
        }));



        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_fpsms>(this));
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



public sealed class UI_btn_child : ViewUI
{

    public override void Update()
    {
        base.Update();

    }

    public override void OnEvent(int type, object userData)
    {
        /* if (type == Event.ID_CHAT_BTN_close || type == Event.ID_CHAT_BTN_send)
         {
             this.btn.gameObject.SetActive(true);
         }*/
    }
    public override bool Init()
    {
        base.Init();

        this.btn = GameObject.Find("btn_child_root").GetComponent<Button>();
        this.btn.onClick.AddListener(delegate()
        {

            this.On_BtnClicked();
        });




        return true;
    }



    private void On_BtnClicked()
    {

    }
    Button btn;

}




public sealed class UI_joystick : ViewUI
{

    public override void Update()
    {
        base.Update();


    }

    public override void OnEvent(int type, object userData)
    {
        /* if (type == Event.ID_CHAT_BTN_close || type == Event.ID_CHAT_BTN_send)
         {
             this.btn.gameObject.SetActive(true);
         }*/
    }
    public override bool Init()
    {
        base.Init();

        this.btn_right = GameObject.Find("btn_right").GetComponent<GAButton>();
        this.btn_left = GameObject.Find("btn_left").GetComponent<GAButton>();
        this.btn_jump = GameObject.Find("btn_jump").GetComponent<Button>();
        this.btn_atk = GameObject.Find("btn_atk").GetComponent<Button>();


        /*     this.btn_right.onClick.AddListener(delegate()
             {
                 this.On_BtnClicked(Events.ID_BTN_RIGHT);
             });
             this.btn_left.onClick.AddListener(delegate()
             {
                 this.On_BtnClicked(Events.ID_BTN_LEFT);
             });

             */
        this.btn_jump.onClick.AddListener(delegate()
        {
            this.On_BtnClicked(Events.ID_BTN_JUMP);
            PublicData.ins.IS_jump = true;
        });
        this.btn_atk.onClick.AddListener(delegate()
        {
            //  this.On_BtnClicked(Events.ID_BTN_ATTACK);
            PublicData.ins.IS_atk = true;
        });
        this.btn_left.onOver = () =>
        {
            //    EventDispatcher.ins.PostEvent(Events.ID_BTN_LEFT);
            //  HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent(Events.ID_BTN_LEFT);
            PublicData.ins.IS_left = true;
        };
        this.btn_left.onExit = () =>
        {
            //  EventDispatcher.ins.PostEvent(Events.ID_STAND);
            //    HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent(Events.ID_STAND);
            PublicData.ins.IS_stand = true;
        };

        this.btn_right.onOver = () =>
        {
            //   HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent(Events.ID_BTN_RIGHT);
            //   EventDispatcher.ins.PostEvent(Events.ID_BTN_RIGHT);
            PublicData.ins.IS_right = true;
        };
        this.btn_right.onExit = () =>
        {
            //  EventDispatcher.ins.PostEvent(Events.ID_STAND);
            //  HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent(Events.ID_STAND);
            PublicData.ins.IS_stand = true;
        };

        return true;
    }



    private void On_BtnClicked(int e)
    {
        //   EventDispatcher.ins.PostEvent(e);

        //  HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent(e);
    }

    GAButton btn_right;
    GAButton btn_left;
    Button btn_atk;
    Button btn_jump;

}







public sealed class UI_skills : ViewUI
{

    public override void Update()
    {
        base.Update();


    }

    public override void OnEvent(int type, object userData)
    {

    }
    public override bool Init()
    {
        base.Init();

        this.btn_skill1 = GameObject.Find("btn_skill1").GetComponent<Button>();
        this.btn_skill2 = GameObject.Find("btn_skill2").GetComponent<Button>();
        this.btn_skill3 = GameObject.Find("btn_skill3").GetComponent<Button>();
        /*
   
   
          this.btn_skill1 = GameObject.Find("UI_Battle").transform.FindChild("ui_panel_skill").transform.FindChild("btn_skill1").GetComponent<Button>();
              this.btn_skill2 = GameObject.Find("UI_Battle").transform.FindChild("ui_panel_skill").transform.FindChild("btn_skill2").GetComponent<Button>();
              this.btn_skill3 = GameObject.Find("UI_Battle").transform.FindChild("ui_panel_skill").transform.FindChild("btn_skill3").GetComponent<Button>();
      
         
         
         */
        this.btn_skill1.onClick.AddListener(delegate()
        {
            //  this.On_BtnClicked(1);
            PublicData.ins.IS_s1 = true;
        });
        this.btn_skill2.onClick.AddListener(delegate()
        {
            this.On_BtnClicked(2);
        });
        this.btn_skill3.onClick.AddListener(delegate()
        {
            this.On_BtnClicked(3);
        });



        this.btn_skill2.gameObject.SetActive(false);
        this.btn_skill3.gameObject.SetActive(false);
        return true;
    }



    private void On_BtnClicked(int e)
    {
        Debug.Log(" skill ckicled" + e);
        if (e == 1)
        {
            //  ModelMgr.ins.self.eventDispatcher.PostEvent(Events.ID_LAUNCH_SKILL1);

            ///  EventDispatcher.ins.PostEvent(Events.ID_LAUNCH_SKILL1);
            ///    HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent(Events.ID_LAUNCH_SKILL1);


        }
    }


    Button btn_skill1;
    Button btn_skill2;
    Button btn_skill3;

}






public sealed class UI_heroInfo : ViewUI
{

    public override void UpdateMS()
    {
        base.UpdateMS();

        if (m == null)
        {
            this.m = HeroMgr.ins.GetSelfHero();
            if (m == null) return;
        }
        txt_info.text = m.name + "LV:" + m.level;
        txt_exp.text = m.current_exp + "/" + m.exp;
        txt_hp.text = m.current_hp + "/" + m.hp;
        txt_mp.text = m.current_mp + "/" + m.mp;

        img_hp.transform.localScale = new Vector3(m.current_hp * 1.0f / m.hp, 1.0f, 1.0f);
        img_mp.transform.localScale = new Vector3(m.current_mp * 1.0f / m.mp, 1.0f, 1.0f);
        img_exp.transform.localScale = new Vector3(m.current_exp * 1.0f / m.exp, 1.0f, 1.0f);

    }

    public override void OnEvent(int type, object userData)
    {

    }
    public override bool Init()
    {
        base.Init();

        //    _ui_root = PrefabsMgr.Load("Prefabs/PanelHero");

        _ui_root = GameObject.Find("UI_Battle").transform.FindChild("ui_panel_hero_info").FindChild("panel_hero").gameObject;
        parent = GameObject.Find("UI_Battle").transform.FindChild("ui_panel_hero_info").gameObject;

        _ui_root.transform.SetParent(parent.transform);


        // init member
        this.img_hp = _ui_root.transform.FindChild("hero_img_hp").GetComponent<Image>();
        this.img_mp = _ui_root.transform.FindChild("hero_img_mp").GetComponent<Image>();
        this.img_exp = _ui_root.transform.FindChild("hero_img_exp").GetComponent<Image>();

        this.txt_hp = _ui_root.transform.FindChild("hero_txt_hp").GetComponent<Text>();
        this.txt_mp = _ui_root.transform.FindChild("hero_txt_mp").GetComponent<Text>();
        this.txt_exp = _ui_root.transform.FindChild("hero_txt_exp").GetComponent<Text>();
        this.txt_info = _ui_root.transform.FindChild("hero_txt_info").GetComponent<Text>();


        //init state
        txt_hp.text = "0/0";
        txt_mp.text = "0/0";
        txt_exp.text = "0/0";

        txt_info.text = "测试玩家 LV:50";


        this.m = HeroMgr.ins.GetSelfHero();

        return true;
    }



    private void On_BtnClicked(int e)
    {
        Debug.Log(" skill ckicled" + e);
        if (e == 1)
        {
            //  ModelMgr.ins.self.eventDispatcher.PostEvent(Events.ID_LAUNCH_SKILL1);

            EventDispatcher.ins.PostEvent(Events.ID_LAUNCH_SKILL1);



        }
    }


    Hero m;

    Text txt_hp;
    Text txt_mp;
    Text txt_exp;
    Text txt_info;

    Image img_hp;
    Image img_mp;
    Image img_exp;
    Image img_icon;


    GameObject parent;// parent is ui_panel_hero_info
}







public sealed class UI_xy : ViewUI
{


    public override void Update()
    {
        base.Update();

        var self = HeroMgr.ins.self;
        if (self != null)
        {
            txt.text = string.Format(DATA.UI_INFO_XY, self.x.ToString("0"), self.y.ToString("0"));
        }
    }

    public override bool Init()
    {
        base.Init();

        this.txt = GameObject.Find("xy").GetComponent<Text>();

        return true;
    }


    private Text txt;

}



public sealed class UI_fpsms : ViewUI
{

    public override void Update()
    {
        if (this.isVideoMode) return;
        base.Update();
        ++counter;
        _fps += 1.0f / Time.deltaTime;
        if (counter >= 10)
        {
            txt.text = string.Format(DATA.UI_FPSMS, (_fps / 10.0f).ToString("0"), +PublicData.GetInstance().ms);
            _fps = 0.0f;
            counter = 0;
        }
    }

    public override bool Init()
    {
        base.Init();

        this.txt = GameObject.Find("fps_ms").GetComponent<Text>();
        this.isVideoMode = PublicData.GetInstance().isVideoMode;
        if (this.isVideoMode)
        {
            this.txt.text = DATA.UI_PLAYING_VIDEOMODE;
        }
        return true;
    }


    private Text txt;
    private float _fps = 0.0f;
    private int counter = 0;
    private bool isVideoMode = false;
}


public sealed class UI_frame : ViewUI
{

    public override void Update()
    {
        base.Update();
        int cur = AppMgr.GetCurrentApp<BattleApp>().GetCurrentFrame();


        int sec_total = cur / 40;

        int min = sec_total / 60;

        int sec = sec_total - min * 60;
        string s_sec, s_min;
        if (sec < 10)
        {
            s_sec = "0" + sec.ToString();
        }
        else
        {
            s_sec = sec.ToString();
        }

        if (min < 10)
        {
            s_min = "0" + min.ToString();
        }
        else
        {
            s_min = min.ToString();
        }
        txt.text = " 时间: " + s_min + ":" + s_sec + "   " + string.Format(DATA.UI_FRAME, cur);

    }

    public override bool Init()
    {
        base.Init();

        this.txt = GameObject.Find("frame").GetComponent<Text>();

        return true;
    }


    private Text txt;

}

