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
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_pvpresult>(this));
            return DATA.EMPTY_STRING;
        }));

        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_buffers>(this));
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
        if (m2 == null)
        {
            if (m2 != null) return;

            ArrayList heros = HeroMgr.ins.GetHeros();
            if (heros.Count == 1) return;

            foreach (Hero hero in heros)
            {
                if (hero != m)
                {
                    //other;

                    m2 = hero;
                    panel2.SetActive(true);
                }
            }
        }


        txt_info1.text = m.no + " LV:" + m.level;
        txt_exp1.text = m.current_exp + "/" + m.exp;
        txt_hp1.text = m.current_hp + "/" + m.hp;
        txt_mp1.text = m.current_mp + "/" + m.mp;

        img_hp1.transform.localScale = new Vector3(m.current_hp * 1.0f / m.hp, 1.0f, 1.0f);
        img_mp1.transform.localScale = new Vector3(m.current_mp * 1.0f / m.mp, 1.0f, 1.0f);
        img_exp1.transform.localScale = new Vector3(m.current_exp * 1.0f / m.exp, 1.0f, 1.0f);


        if (m2 != null)
        {

            txt_info2.text = m2.no + " LV:" + m2.level;
            txt_exp2.text = m2.current_exp + "/" + m2.exp;
            txt_hp2.text = m2.current_hp + "/" + m2.hp;
            txt_mp2.text = m2.current_mp + "/" + m2.mp;

            img_hp2.transform.localScale = new Vector3(m2.current_hp * 1.0f / m2.hp, 1.0f, 1.0f);
            img_mp2.transform.localScale = new Vector3(m2.current_mp * 1.0f / m2.mp, 1.0f, 1.0f);
            img_exp2.transform.localScale = new Vector3(m2.current_exp * 1.0f / m2.exp, 1.0f, 1.0f);




        }

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


        panel1 = GameObject.Find("panel_hero");
        panel2 = GameObject.Find("panel_hero2");


        // init member
        this.img_hp1 = panel1.transform.FindChild("hero_img_hp").GetComponent<Image>();
        this.img_mp1 = panel1.transform.FindChild("hero_img_mp").GetComponent<Image>();
        this.img_exp1 = panel1.transform.FindChild("hero_img_exp").GetComponent<Image>();

        this.txt_hp1 = panel1.transform.FindChild("hero_txt_hp").GetComponent<Text>();
        this.txt_mp1 = panel1.transform.FindChild("hero_txt_mp").GetComponent<Text>();
        this.txt_exp1 = panel1.transform.FindChild("hero_txt_exp").GetComponent<Text>();
        this.txt_info1 = panel1.transform.FindChild("hero_txt_info").GetComponent<Text>();


        // init member
        this.img_hp2 = panel2.transform.FindChild("hero_img_hp").GetComponent<Image>();
        this.img_mp2 = panel2.transform.FindChild("hero_img_mp").GetComponent<Image>();
        this.img_exp2 = panel2.transform.FindChild("hero_img_exp").GetComponent<Image>();

        this.txt_hp2 = panel2.transform.FindChild("hero_txt_hp").GetComponent<Text>();
        this.txt_mp2 = panel2.transform.FindChild("hero_txt_mp").GetComponent<Text>();
        this.txt_exp2 = panel2.transform.FindChild("hero_txt_exp").GetComponent<Text>();
        this.txt_info2 = panel2.transform.FindChild("hero_txt_info").GetComponent<Text>();






        //init state
        txt_hp1.text = "0/0";
        txt_mp1.text = "0/0";
        txt_exp1.text = "0/0";

        txt_info1.text = "测试玩家 LV:50";
        //init state
        txt_hp2.text = "0/0";
        txt_mp2.text = "0/0";
        txt_exp2.text = "0/0";

        txt_info2.text = "测试玩家 LV:50";

        this.panel2.SetActive(false);
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


    Hero m = null;
    Hero m2 = null;
    Text txt_hp1;
    Text txt_mp1;
    Text txt_exp1;
    Text txt_info1;

    Image img_hp1;
    Image img_mp1;
    Image img_exp1;
    Image img_icon1;




    Text txt_hp2;
    Text txt_mp2;
    Text txt_exp2;
    Text txt_info2;

    Image img_hp2;
    Image img_mp2;
    Image img_exp2;
    Image img_icon2;









    GameObject panel1 = null;
    GameObject panel2 = null;

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
            txt.text = string.Format(DATA.UI_INFO_XY, (100.0f * self.x).ToString("0"), ((self.y + self.height) * 100.0f).ToString("0"));
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


public sealed class UI_pvpresult : ViewUI
{

    public override void Update()
    {

    }
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_BATTLE_PVP_WAITFOR_RESULT_SHOW)
        {
            this.panel.SetActive(true);
        }

        else if (type == Events.ID_BATTLE_PVP_RETULT)
        {
            string res = userData as string;
            Debug.Log("BattleApp Result " + res);
            this.btn_video.gameObject.SetActive(true);
            this.btn_return.gameObject.SetActive(true);

            HashTable kv = Json.Decode(res);
            this.txt_result.text = kv["msg"];

        }
    }
    public override bool Init()
    {
        base.Init();

        this.panel = GameObject.Find("ui_panel_result");

        this.btn_return = panel.transform.FindChild("btn_return").GetComponent<Button>();
        this.btn_video = panel.transform.FindChild("btn_video").GetComponent<Button>();
        this.txt_result = panel.transform.FindChild("txt_result").GetComponent<Text>();



        this.btn_return.onClick.AddListener(() =>
         {
             EventDispatcher.ins.PostEvent(Events.ID_BATTLE_EXIT);//dispost BattleApp

             PublicData.ins.ResetPVP();
             SceneMgr.Load("TownScene");
         });

        this.btn_video.onClick.AddListener(() =>
           {
               Debug.Log("观看录像");
               PublicData.ins.isVideoMode = true;

               if (PublicData.ins.is_pve)
               {
                   SceneMgr.Load("BattlePVE");
               }
               else
               {
                   SceneMgr.Load("BattlePVP");
               }
           });

        EventDispatcher.ins.AddEventListener(this, Events.ID_BATTLE_PVP_WAITFOR_RESULT_SHOW);
        EventDispatcher.ins.AddEventListener(this, Events.ID_BATTLE_PVP_RETULT);

        this.panel.SetActive(false);
        this.btn_video.gameObject.SetActive(false);
        this.btn_return.gameObject.SetActive(false);
        this.txt_result.text = "等待服务器响应...";
        return true;
    }


    GameObject panel = null;
    Button btn_return = null;
    Button btn_video = null;
    Text txt_result = null;
}





public sealed class UI_buffers : ViewUI
{


    public override void Update()
    {
        base.Update();

        if (tick.Tick()) return;
        tick.Reset();
        Hero self = HeroMgr.ins.self;
        if (self == null) return;

        ArrayList buffers = self.bufferMgr.GetBuffers();

        int index = 0;
        foreach (Buffer buffer in buffers)
        {
            if (!buffer.show_ui) continue;
            if (index > MAX_BUFFER_SHOW) break;

            Image img = (list_img[index] as Image);
            Text txt = (list_txt[index] as Text);

            img.gameObject.SetActive(true);
            txt.gameObject.SetActive(true);

            txt.text = buffer.brief;
            Sprite sp = SpriteFrameCache.ins.GetSpriteFrameAuto(buffer.icon).sprite;
            img.sprite =sp;
            ++index;
        }

        for (int i = index; i <= MAX_BUFFER_SHOW; i++)
        {
            (list_txt[i] as Text).gameObject.SetActive(false);
            (list_img[i] as Image).gameObject.SetActive(false);
        }

    }

    public override bool Init()
    {
        base.Init();
        this.panel = GameObject.Find("ui_panel_buffers");


        for (int i = 0; i <= MAX_BUFFER_SHOW; i++)
        {
            string name = "buffer" + i.ToString();

            Transform buf = this.panel.transform.FindChild(name);

            Image img = buf.GetComponentInChildren<Image>();
            Text txt = buf.GetComponentInChildren<Text>();

            this.list_img.Add(img);
            this.list_txt.Add(txt);

            img.gameObject.SetActive(false);
            txt.gameObject.SetActive(false);

        }
        if (list_img.Count != list_txt.Count)
        {
            throw new ArgumentNullException("");
        }
        return true;
    }

    Counter tick = Counter.Create(10);
    private const int MAX_BUFFER_SHOW = 7;

    ArrayList list_img = new ArrayList();
    ArrayList list_txt = new ArrayList();
    GameObject panel = null;
}
