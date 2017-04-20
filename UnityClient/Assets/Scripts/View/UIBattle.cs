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
    public static UIBattleRoot ins = null;
    public UIBattleRoot()
    {
        ins = this;
    }
    public void Hide()
    {
        this._ui_root.SetActive(false);
    }
    public void Show()
    {
        this._ui_root.SetActive(true);
    }

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
            this._ui_child.Add(ViewUI.Create<UI_combo>(this));
            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_tower>(this));
            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_frame>(this));
            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_boss>(this));
            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_flytext>(this));
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
            this._ui_child.Add(ViewUI.Create<UI_die>(this));
            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_buffers>(this));
            return DATA.EMPTY_STRING;
        }));

        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_dirInput>(this));
            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_hps>(this));
            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_smallmap>(this));
            return DATA.EMPTY_STRING;
        }));
        return true;
    }

    public override void OnExit()
    {

        UnityEngine.Object.Destroy(this._ui_root);

        base.OnExit();
    }
    public T GetViewUI<T>() where T : ViewUI
    {
        System.Type t = typeof(T);
        foreach (ViewUI b in _ui_child)
        {
            if (b.GetType() == t)
            {
                return b as T;
            }
        }
        return null;
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
            if (PublicData.ins.inputAble == false) return;

            this.On_BtnClicked(Events.ID_BTN_JUMP);
            PublicData.ins.IS_jump = true;
        });
        this.btn_atk.onClick.AddListener(delegate()
        {
            //  this.On_BtnClicked(Events.ID_BTN_ATTACK);
            if (PublicData.ins.inputAble == false) return;

            PublicData.ins.IS_atk = true;
        });
        this.btn_left.onOver = () =>
        {
            //    EventDispatcher.ins.PostEvent(Events.ID_BTN_LEFT);
            //  HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent(Events.ID_BTN_LEFT);
            if (PublicData.ins.inputAble == false) return;

            PublicData.ins.IS_left = true;
        };
        this.btn_left.onExit = () =>
        {
            //  EventDispatcher.ins.PostEvent(Events.ID_STAND);
            //    HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent(Events.ID_STAND);
            if (PublicData.ins.inputAble == false) return;

            PublicData.ins.IS_stand = true;
        };

        this.btn_right.onOver = () =>
        {
            //   HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent(Events.ID_BTN_RIGHT);
            //   EventDispatcher.ins.PostEvent(Events.ID_BTN_RIGHT);
            if (PublicData.ins.inputAble == false) return;

            PublicData.ins.IS_right = true;
        };
        this.btn_right.onExit = () =>
        {
            //  EventDispatcher.ins.PostEvent(Events.ID_STAND);
            //  HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent(Events.ID_STAND);

            if (PublicData.ins.inputAble == false) return;

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
        if (type == Events.ID_SKILL_LEVEL_IS_UP)
        {

            ArrayList list = (userData as ArrayList);

            for (int i = 0; i < list.Count; i++)
            {
                if ((bool)list[i] == true)
                {
                    ShowButtonByNumber(i + 1);
                }
            }
        }
        if (type == Events.ID_SKILL1_COOL_INFOMATION)
        {
            Counter counter = (userData as Counter);
            float max = counter.GetMax();
            float current = counter.GetCurrent();
            if (counter.IsMax())
            {
                cd_skill1.transform.FindChild("filled").GetComponent<Image>().fillAmount = 0f;

                cd_skill1.transform.FindChild("time").GetComponent<Text>().text = "";

            }
            else
            {
                if (current <= max)
                {
                    float percent = (max - current) / max;
                    float time = (max - current) / Config.MAX_FPS;

                    cd_skill1.transform.FindChild("filled").GetComponent<Image>().fillAmount = percent;

                    cd_skill1.transform.FindChild("time").GetComponent<Text>().text = time.ToString("0.0");
                    if ((time) < 0.0000001)
                    {
                        cd_skill1.transform.FindChild("time").GetComponent<Text>().text = "";
                    }
                }
                else
                {
                    cd_skill1.transform.FindChild("filled").GetComponent<Image>().fillAmount = 0f;


                    cd_skill1.transform.FindChild("time").GetComponent<Text>().text = "";

                }
            }
        }
        if (type == Events.ID_SKILL2_COOL_INFOMATION)
        {
            Counter counter = (userData as Counter);
            float max = counter.GetMax();
            float current = counter.GetCurrent();
            if (current <= max)
            {
                float percent = (max - current) / max;
                float time = (max - current) / Config.MAX_FPS;

                cd_skill2.transform.FindChild("filled").GetComponent<Image>().fillAmount = percent;

                cd_skill2.transform.FindChild("time").GetComponent<Text>().text = time.ToString("0.0");
                if ((time) < 0.0000001)
                {
                    cd_skill2.transform.FindChild("time").GetComponent<Text>().text = "";
                }
            }
        }
        if (type == Events.ID_SKILL3_COOL_INFOMATION)
        {
            Counter counter = (userData as Counter);
            float max = counter.GetMax();
            float current = counter.GetCurrent();
            if (current <= max)
            {
                float percent = (max - current) / max;
                float time = (max - current) / Config.MAX_FPS;

                cd_skill3.transform.FindChild("filled").GetComponent<Image>().fillAmount = percent;

                cd_skill3.transform.FindChild("time").GetComponent<Text>().text = time.ToString("0.0");
                if ((time) < 0.0000001)
                {
                    cd_skill3.transform.FindChild("time").GetComponent<Text>().text = "";
                }
            }
        }
        if (type == Events.ID_SKILL1_LEVEL_INFOMATION)
        {

            int level = (int)(userData);
            skill1_level.text = "LV:" + level.ToString();

        }
        if (type == Events.ID_SKILL2_LEVEL_INFOMATION)
        {
            int level = (int)(userData);
            skill2_level.text = "LV:" + level.ToString();
        }
        if (type == Events.ID_SKILL3_LEVEL_INFOMATION)
        {
            int level = (int)(userData);
            skill3_level.text = "LV:" + level.ToString();
            if (level == 3)
            {
                //    Debug.LogError(level);    
            }

        }
        if (type == Events.ID_CHANGE_SKILL_ICON)
        {
            this.btn_skill1.GetComponent<Image>().sprite = SkillImagePath.ins.skill1;
            this.btn_skill2.GetComponent<Image>().sprite = SkillImagePath.ins.skill2;
            this.btn_skill3.GetComponent<Image>().sprite = SkillImagePath.ins.skill3;
        }
    }

    public void ShowAllLevelUpButton()
    {
        this.btn_skill1_levelup.gameObject.SetActive(true);
        this.btn_skill2_levelup.gameObject.SetActive(true);
        this.btn_skill3_levelup.gameObject.SetActive(true);
        this.btn_skill4_levelup.gameObject.SetActive(true);
        this.btn_skill5_levelup.gameObject.SetActive(true);
    }

    public void HideAllLevelUpButton()
    {
        this.btn_skill1_levelup.gameObject.SetActive(false);
        this.btn_skill2_levelup.gameObject.SetActive(false);
        this.btn_skill3_levelup.gameObject.SetActive(false);
        this.btn_skill4_levelup.gameObject.SetActive(false);
        //this.btn_skill5_levelup.gameObject.SetActive(false);
    }
    public void ShowButtonByNumber(int num)
    {
        switch (num)
        {
            case 1:
                this.btn_skill1_levelup.gameObject.SetActive(true);
                break;
            case 2:
                this.btn_skill2_levelup.gameObject.SetActive(true);
                break;
            case 3:
                this.btn_skill3_levelup.gameObject.SetActive(true);
                break;
            case 4:
                this.btn_skill4_levelup.gameObject.SetActive(true);
                break;
            case 5:
                this.btn_skill5_levelup.gameObject.SetActive(true);
                break;
        }
    }


    public override bool Init()
    {
        base.Init();


        this.btn_skill1 = GameObject.Find("btn_skill1").GetComponent<Button>();
        this.btn_skill2 = GameObject.Find("btn_skill2").GetComponent<Button>();
        this.btn_skill3 = GameObject.Find("btn_skill3").GetComponent<Button>();
        this.btn_skill4 = GameObject.Find("btn_skill4").GetComponent<Button>();
        this.btn_skill5 = GameObject.Find("btn_skill5").GetComponent<Button>();
        this.btn_skill1_levelup = GameObject.Find("btn_skill1_levelup").GetComponent<Button>();
        this.btn_skill2_levelup = GameObject.Find("btn_skill2_levelup").GetComponent<Button>();
        this.btn_skill3_levelup = GameObject.Find("btn_skill3_levelup").GetComponent<Button>();
        this.btn_skill4_levelup = GameObject.Find("btn_skill4_levelup").GetComponent<Button>();
        this.skill1_level = GameObject.Find("Text_skill_1_level").GetComponent<Text>();
        this.skill2_level = GameObject.Find("Text_skill_2_level").GetComponent<Text>();
        this.skill3_level = GameObject.Find("Text_skill_3_level").GetComponent<Text>();
        HideAllLevelUpButton();
        cd_skill1 = GameObject.Find("cool_skill1");
        cd_skill2 = GameObject.Find("cool_skill2");
        cd_skill3 = GameObject.Find("cool_skill3");
        cd_skill1.transform.FindChild("filled").GetComponent<Image>().fillAmount = 0;
        cd_skill2.transform.FindChild("filled").GetComponent<Image>().fillAmount = 0;
        cd_skill3.transform.FindChild("filled").GetComponent<Image>().fillAmount = 0;
        cd_skill1.transform.FindChild("time").GetComponent<Text>().text = "";
        cd_skill2.transform.FindChild("time").GetComponent<Text>().text = "";
        cd_skill3.transform.FindChild("time").GetComponent<Text>().text = "";
        skill1_level.text = "";
        skill2_level.text = "";
        skill3_level.text = "";
        //this.btn_skill5_levelup = GameObject.Find("btn_skill5_levelup").GetComponent<Button>();
        /*
   
   
          this.btn_skill1 = GameObject.Find("UI_Battle").transform.FindChild("ui_panel_skill").transform.FindChild("btn_skill1").GetComponent<Button>();
              this.btn_skill2 = GameObject.Find("UI_Battle").transform.FindChild("ui_panel_skill").transform.FindChild("btn_skill2").GetComponent<Button>();
              this.btn_skill3 = GameObject.Find("UI_Battle").transform.FindChild("ui_panel_skill").transform.FindChild("btn_skill3").GetComponent<Button>();
      
        
         */
        this.btn_skill1.onClick.AddListener(delegate()
        {
            //  this.On_BtnClicked(1);
            PublicData.ins.IS_s1 = 1;
        });
        this.btn_skill2.onClick.AddListener(delegate()
        {
            PublicData.ins.IS_s1 = 2;
        });
        this.btn_skill3.onClick.AddListener(delegate()
        {
            PublicData.ins.IS_s1 = 3;
        });
        this.btn_skill4.onClick.AddListener(delegate()
        {
            PublicData.ins.IS_s1 = 4;
        }); this.btn_skill5.onClick.AddListener(delegate()
        {
            PublicData.ins.IS_s1 = 5;
        });
        this.btn_skill1_levelup.onClick.AddListener(() =>
        {
            HideAllLevelUpButton();
            PublicData.ins.IS_opt = FrameCustomsOpt.level_up1;


        });
        this.btn_skill2_levelup.onClick.AddListener(() =>
        {


            HideAllLevelUpButton();
            PublicData.ins.IS_opt = FrameCustomsOpt.level_up2;

        });
        this.btn_skill3_levelup.onClick.AddListener(() =>
        {
            HideAllLevelUpButton();
            PublicData.ins.IS_opt = FrameCustomsOpt.level_up3;


        });
        this.btn_skill4_levelup.onClick.AddListener(() =>
        {
            PublicData.ins.IS_opt = FrameCustomsOpt.level_up4;
            HideAllLevelUpButton();
        });

        EventDispatcher.ins.AddEventListener(this, Events.ID_SKILL_LEVEL_IS_UP);
        EventDispatcher.ins.AddEventListener(this, Events.ID_SKILL1_COOL_INFOMATION);
        EventDispatcher.ins.AddEventListener(this, Events.ID_SKILL2_COOL_INFOMATION);
        EventDispatcher.ins.AddEventListener(this, Events.ID_SKILL3_COOL_INFOMATION);
        EventDispatcher.ins.AddEventListener(this, Events.ID_SKILL1_LEVEL_INFOMATION);
        EventDispatcher.ins.AddEventListener(this, Events.ID_SKILL2_LEVEL_INFOMATION);
        EventDispatcher.ins.AddEventListener(this, Events.ID_SKILL3_LEVEL_INFOMATION);
        EventDispatcher.ins.AddEventListener(this, Events.ID_CHANGE_SKILL_ICON);
        //  this.btn_skill2.gameObject.SetActive(false);
        //   this.btn_skill3.gameObject.SetActive(false);
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
    Button btn_skill4;
    Button btn_skill5;//切换取消 按钮

    Button btn_skill1_levelup;
    Button btn_skill2_levelup;
    Button btn_skill3_levelup;
    Button btn_skill4_levelup;
    Button btn_skill5_levelup;
    private GameObject cd_skill1;
    private GameObject cd_skill2;
    private GameObject cd_skill3;
    private Text skill1_level;
    private Text skill2_level;
    private Text skill3_level;
}
public sealed class UI_heroInfo : ViewUI
{
    bool has_vibrate = false;
    float current_a = 0f;
    float current = 1f;
    public override void UpdateMS()
    {
        base.UpdateMS();

        if (m == null)
        {
            this.m = HeroMgr.ins.GetSelfHero();
            if (m == null) return;
        }

        if (m.current_hp <= m.hp * 0.3f)
        {
            img_bleed.gameObject.SetActive(true);
            if (has_vibrate == false)
            {
                has_vibrate = true;
#if UNITY_IOS || UNITY_ANDROID
             Handheld.Vibrate();
#endif
            }
            current_a += 0.05f * current;
            if (current_a >= 1 && current > 0)
            {
                current = -1f;
            }
            else if (current_a <= 0 && current < 0)
            {
                current = 1f;
            }
            img_bleed.color = new Color(img_bleed.color.r, img_bleed.color.g, img_bleed.color.b, current_a);
        }
        else
        {
            has_vibrate = false;
            img_bleed.gameObject.SetActive(false);
        }
        txt_info1.text = m.no + " LV:" + (m.level + 1);
        txt_exp1.text = m.current_exp + "/" + m.exp;
        txt_hp1.text = m.current_hp + "/" + m.hp;
        //  txt_mp1.text = m.current_mp + "/" + m.mp;
        if (m.hp > 0)
            img_hp1.transform.localScale = new Vector3(Utils.RangeLimit(m.current_hp * 1.0f / m.hp, 0f, 1f), 1.0f, 1.0f);
        //    img_mp1.transform.localScale = new Vector3(m.current_mp * 1.0f / m.mp, 1.0f, 1.0f);
        if (m.exp > 0)
            img_exp1.transform.localScale = new Vector3(Utils.RangeLimit(m.current_exp * 1.0f / m.exp, 0f, 1f), 1.0f, 1.0f);

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

        if (m2 != null)
        {

            txt_info2.text = m2.no + " LV:" + (m2.level + 1);
            txt_exp2.text = m2.current_exp + "/" + m2.exp;
            txt_hp2.text = m2.current_hp + "/" + m2.hp;
            //  txt_mp2.text = m2.current_mp + "/" + m2.mp;

            img_hp2.transform.localScale = new Vector3(Utils.RangeLimit(m2.current_hp * 1.0f / m2.hp, 0f, 1f), 1.0f, 1.0f);
            //   img_mp2.transform.localScale = new Vector3(m2.current_mp * 1.0f / m2.mp, 1.0f, 1.0f);
            img_exp2.transform.localScale = new Vector3(Utils.RangeLimit(m2.current_exp * 1.0f / m2.exp, 0f, 1f), 1.0f, 1.0f);
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
        img_bleed = GameObject.Find("ui_panel_hero_info/bleed").GetComponent<Image>();
        img_bleed.transform.localScale = new Vector3(1f, 1f, 1f);
        // init member
        this.img_hp1 = panel1.transform.FindChild("hero_img_hp").GetComponent<Image>();
        //  this.img_mp1 = panel1.transform.FindChild("hero_img_mp").GetComponent<Image>();
        this.img_exp1 = panel1.transform.FindChild("hero_img_exp").GetComponent<Image>();

        this.txt_hp1 = panel1.transform.FindChild("hero_txt_hp").GetComponent<Text>();
        // this.txt_mp1 = panel1.transform.FindChild("hero_txt_mp").GetComponent<Text>();
        this.txt_exp1 = panel1.transform.FindChild("hero_txt_exp").GetComponent<Text>();
        this.txt_info1 = panel1.transform.FindChild("hero_txt_info").GetComponent<Text>();


        // init member
        this.img_hp2 = panel2.transform.FindChild("hero_img_hp").GetComponent<Image>();
        //  this.img_mp2 = panel2.transform.FindChild("hero_img_mp").GetComponent<Image>();
        this.img_exp2 = panel2.transform.FindChild("hero_img_exp").GetComponent<Image>();

        this.txt_hp2 = panel2.transform.FindChild("hero_txt_hp").GetComponent<Text>();
        //  this.txt_mp2 = panel2.transform.FindChild("hero_txt_mp").GetComponent<Text>();
        this.txt_exp2 = panel2.transform.FindChild("hero_txt_exp").GetComponent<Text>();
        this.txt_info2 = panel2.transform.FindChild("hero_txt_info").GetComponent<Text>();

        //init state
        txt_hp1.text = "0/0";
        //  txt_mp1.text = "0/0";
        txt_exp1.text = "0/0";

        txt_info1.text = "测试玩家 LV:50";
        //init state
        txt_hp2.text = "0/0";
        //  txt_mp2.text = "0/0";
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
    //  Text txt_mp1;
    Text txt_exp1;
    Text txt_info1;

    Image img_hp1;
    //   Image img_mp1;
    Image img_exp1;
    Image img_icon1;

    Text txt_hp2;
    //    Text txt_mp2;
    Text txt_exp2;
    Text txt_info2;

    Image img_hp2;
    //  Image img_mp2;
    Image img_exp2;
    Image img_icon2;

    GameObject panel1 = null;
    GameObject panel2 = null;

    GameObject parent;// parent is ui_panel_hero_info
    Image img_bleed;
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
            txt.text = "剩余可死亡次数:" + PublicData.ins.left_revive_times + "     " + string.Format(DATA.UI_FPSMS, (_fps / 10.0f).ToString("0"), +PublicData.GetInstance().ms);
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
    int index_ani = 0;
    GameObject animation = null;
    bool has_run = false;
    private void RunAnimationNext()
    {
        if (index_ani >= 5)
        {
            this.RunOtherWin();
            return;
        }
        string[] plists = { "hd/enemies/enemy_518/bullet/enemy_518_bul_518023/enemy_518_bul_518023.plist",
                              "hd/enemies/enemy_518/bullet/enemy_518_bul_518024/enemy_518_bul_518024.plist",
                              "hd/enemies/enemy_518/bullet/enemy_518_bul_518024/enemy_518_bul_518024.plist",
                              "hd/enemies/enemy_518/bullet/enemy_518_bul_518023/enemy_518_bul_518023.plist",
                              "hd/enemies/enemy_518/bullet/enemy_518_bul_518023/enemy_518_bul_518023.plist","","",""};

        float[] posx = { Screen.width * 0.3f, Screen.width * 0.7f, Screen.width * 0.8f, Screen.width * 0.4f, Screen.width * 0.5f, };
        float[] posy = { Screen.height * 0.3f, Screen.height * 0.7f, Screen.height * 0.2f, Screen.height * 0.6f, Screen.height * 0.5f, };

        animation = PrefabsMgr.Load("Prefabs/AnimationsUI");
        animation.transform.parent = this.panel_effects.transform;
        animation.GetComponent<AnimationstorUI>().file = plists[index_ani];
        var ani = animation.GetComponent<AnimationstorUI>();
        ani.FrameDelay = 2;
        ani.Init();
        ani.ani.Run();

        ani.ani.SetLoop(1);
        //   animation.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 200f);
        animation.transform.position = new Vector3(posx[index_ani], posy[index_ani], 0);

        animation.transform.localScale = new Vector3(5f, 5f, 5f);
        if (index_ani < 5)
        {
            CallFunc.Create(panel, 0.5f, () =>
                {
                    //  GameObject.Destroy(animation);
                    this.RunAnimationNext();
                });
        }
        else
        {

        }
        index_ani++;
    }
    bool has_move = false;
    private void RunOtherWin()
    {
        if (has_move) return;
        has_move = true;
        GameObject obj_title = panel_win.transform.FindChild("img_title").gameObject;
        GameObject obj_hero = panel_win.transform.FindChild("img_hero").gameObject;
        FadeIn.Create(obj_hero, 1f);
        ScaleTo.Create(obj_title, 0.05f, 0.7f, 0.7f).OnComptele = () =>
        {
            ScaleTo.Create(obj_title, 0.01f, 0.9f, 0.9f).OnComptele = () =>
            {
                ScaleTo.Create(obj_title, 0.01f, 1.2f, 1.2f).OnComptele = () =>
                {
                    ScaleTo.Create(obj_title, 0.03f, 1f, 1f).OnComptele = () =>
                    {

                    };
                };
            };
        };
    }

    private void RunOtherLose()
    {
        if (has_move) return;
        has_move = true;
        GameObject obj_title = panel_lose.transform.FindChild("img_title").gameObject;
        GameObject obj_hero = panel_lose.transform.FindChild("img_hero").gameObject;
        FadeIn.Create(obj_hero, 1f);
        ScaleTo.Create(obj_title, 0.05f, 0.7f, 0.7f).OnComptele = () =>
        {
            ScaleTo.Create(obj_title, 0.01f, 0.9f, 0.9f).OnComptele = () =>
            {
                ScaleTo.Create(obj_title, 0.01f, 1.2f, 1.2f).OnComptele = () =>
                {
                    ScaleTo.Create(obj_title, 0.03f, 1f, 1f).OnComptele = () =>
                    {

                    };
                };
            };
        };
    }
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_BATTLE_PVP_WAITFOR_RESULT_SHOW)
        {
            this.panel.SetActive(true);

            if (PublicData.ins.battle_result == BattleResult.Win)
            {
                panel_win.SetActive(true);
                panel_effects.SetActive(true);
                RunAnimationNext();
                AudioMgr.ins.PostEvent(AudioEvents.Events.BATTLE_WIN, false);
            }
            else if (PublicData.ins.battle_result == BattleResult.Lose)
            {
                AudioMgr.ins.PostEvent(AudioEvents.Events.BATTLE_LOSE, false);
                panel_lose.SetActive(true);
                this.RunOtherLose();
            }
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
        this.panel.transform.localScale = new Vector3(1f, 1f, 1f);
        this.btn_return = panel.transform.FindChild("btn_return").GetComponent<Button>();
        this.btn_video = panel.transform.FindChild("btn_video").GetComponent<Button>();
        this.txt_result = panel.transform.FindChild("txt_result").GetComponent<Text>();
        this.panel_win = panel.transform.FindChild("panel_win").gameObject;
        this.panel_lose = panel.transform.FindChild("panel_lose").gameObject;
        this.panel_effects = panel.transform.FindChild("panel_effects").gameObject;

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
        this.txt_result.text = "";//"等待服务器响应...";

        panel_lose.SetActive(false);
        panel_win.SetActive(false);
        panel_effects.SetActive(false);

        return true;
    }

    GameObject ani_1;



    GameObject panel = null;
    Button btn_return = null;
    Button btn_video = null;
    Text txt_result = null;
    GameObject panel_effects;
    GameObject panel_win;
    GameObject panel_lose;
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

            (list_panel[index] as GameObject).SetActive(true);
            if (buffer.enable_time)
            {
                Counter t = buffer.GetCounter();

                float current = t.GetCurrent();
                float max = t.GetMax();
                (list_img_filled[index] as Image).fillAmount = current / max;
                float timef = ((max - current) / (float)Config.MAX_FPS);

                int time = (int)((max - current) / Config.MAX_FPS);
                if (timef < 1.0f)
                {
                    (list_txt_time[index] as Text).text = timef.ToString("0.0");
                }
                else
                {
                    (list_txt_time[index] as Text).text = time.ToString();
                }
            }
            else
            {
                (list_img_filled[index] as Image).fillAmount = 0;
                (list_txt_time[index] as Text).text = "";
            }
            (list_txt[index] as Text).text = buffer.brief;
            Sprite sp = SpriteFrameCache.ins.GetSpriteFrameAuto(buffer.icon).sprite;
            (list_img[index] as Image).sprite = sp;
            ++index;
        }

        for (int i = index; i <= MAX_BUFFER_SHOW; i++)
        {
            (list_panel[i] as GameObject).SetActive(false);

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

            Image img = buf.FindChild("icon").GetComponent<Image>();
            Text txt = buf.GetComponentInChildren<Text>();
            Image img_fill = buf.FindChild("filled").GetComponent<Image>();
            Text txt_time = buf.FindChild("time").GetComponent<Text>();
            this.list_img.Add(img);
            this.list_txt.Add(txt);
            this.list_img_filled.Add(img_fill);
            this.list_txt_time.Add(txt_time);

            buf.gameObject.SetActive(false);
            this.list_panel.Add(buf.gameObject);

        }
        if (list_img.Count != list_txt.Count)
        {
            throw new ArgumentNullException("");
        }
        return true;
    }

    Counter tick = Counter.Create(0);
    private const int MAX_BUFFER_SHOW = 7;
    ArrayList list_panel = new ArrayList();//GameObject
    ArrayList list_img = new ArrayList();
    ArrayList list_txt = new ArrayList();
    ArrayList list_txt_time = new ArrayList();
    ArrayList list_img_filled = new ArrayList();
    GameObject panel = null;
}


/// <summary>
/// 漂浮字体ADT 初始化基本信息（config info）
/// 其他内容创建者勿动
/// </summary>
public class BattleFlyTextInfo : GAObject
{
    //---- config info
    public string txt = "";//显示的内容
    public Color color = Color.white;// 颜色
    public float position_world_x = 0.0f; // 世界坐标
    public float position_world_y = 0.0f; // 世界坐标

    public float time = 1.0f;//持续时间

    public Type type = Type.UP_DOWN;

    //----------const
    public static Color COLOR_HP_REDUCE = new Color(178.0f / 255.0f, 0, 1.0f, 1.0f);
    public static Color COLOR_HP_ADD = new Color(0.0f, 1.0f, 149.0f / 255.0f, 1.0f);

    public enum Type
    {
        UP_DOWN,//往正上方漂浮
        UNKNOWN = UP_DOWN,
    }
    // ------inner  use for UI_flytxt

    public float current_time = 0.0f;
    public Text txt_txt = null;

    public Counter tick = null;

    public override void UpdateMS()
    {
        if (tick.Tick())
        {

            if (type == Type.UP_DOWN)
            {
                this.UpdateMS_With_UP_DOWN();
            }
            return;
        }

        this.SetInValid();
    }

    private void UpdateMS_With_UP_DOWN()
    {
        Vector3 pos = this.txt_txt.gameObject.transform.position;

        float speed = Screen.height / 150.0f;
        this.txt_txt.gameObject.transform.position = new Vector3(pos.x, pos.y + speed, pos.z);

    }
    public static BattleFlyTextInfo Create()
    {
        BattleFlyTextInfo ret = new BattleFlyTextInfo();
        return ret;
    }
    private BattleFlyTextInfo()
    {

    }
    public override void OnDispose()
    {
        if (txt_txt != null)
        {
            GameObject.Destroy(txt_txt.gameObject);
        }
    }
}

/// <summary>
/// 漂浮字体 比如血量显示 暴击显示
/// </summary>
public sealed class UI_flytext : ViewUI
{
    public override void UpdateMS()
    {
        foreach (BattleFlyTextInfo info in lists)
        {
            info.UpdateMS();
        }
        for (int i = 0; i < lists.Count; )
        {
            BattleFlyTextInfo b = lists[i] as BattleFlyTextInfo;
            if (b.IsInValid())
            {
                this.lists.Remove(b);
                b.Dispose();
            }
            else
            {
                ++i;
            }
        }
    }
    public override void OnEvent(int type, object userData)
    {
        if (Events.ID_BATTLE_FLYTEXT == type)
        {
            //新漂浮字体
            this.AddNewFlyText(userData as BattleFlyTextInfo);
        }
    }

    public override bool Init()
    {
        base.Init();
        this.panel = GameObject.Find("ui_panel_txts");

        template_copy = panel.transform.FindChild("template_txt").gameObject;
        template_copy.SetActive(false);

        EventDispatcher.ins.AddEventListener(this, Events.ID_BATTLE_FLYTEXT);
        return true;
    }
    private void AddNewFlyText(BattleFlyTextInfo info)
    {
        info.tick = Counter.Create((int)(10));
        info.txt_txt = ((GameObject)(GameObject.Instantiate(template_copy, template_copy.transform.parent))).GetComponent<Text>();
        info.txt_txt.gameObject.SetActive(true);

        Vector3 pos = new Vector3();
        pos.x = info.position_world_x;
        pos.y = info.position_world_y;
        pos.z = info.txt_txt.gameObject.transform.position.z;
        info.txt_txt.color = info.color;
        info.txt_txt.gameObject.transform.position = Camera.main.WorldToScreenPoint(pos); ;
        info.txt_txt.text = info.txt;
        this.lists.Add(info);

    }
    ArrayList lists = new ArrayList();

    GameObject template_copy = null;
    GameObject panel = null;
}

public sealed class UI_combo : ViewUI
{
    public override void Update()
    {
        base.Update();

        var self = HeroMgr.ins.self;
        if (self == null) return;

        if (self.combo_time == 0)
        {
            combo_cache = 0;
            return;
        }
        if (combo_cache == self.combo_time)
        {
            return;
        }
        Actions[] action_pre = txt.gameObject.GetComponents<ScaleTo>();
        if (action_pre.Length > 0) return;

        foreach (Actions a in action_pre)
        {
            a.Dispose();
        }
        combo_cache = self.combo_time;
        txt.text = "连击X" + combo_cache.ToString();

        tick_hide.Reset();
        txt.gameObject.SetActive(true);
        /* ScaleTo.Create(txt.gameObject, 0.05f, 1.7f, 0.7f).OnComptele = () =>
         {
             ScaleTo.Create(txt.gameObject, 0.01f, 1.9f, 1.9f).OnComptele = () =>
             {
                 ScaleTo.Create(txt.gameObject, 0.01f, 1.2f, 1.2f).OnComptele = () =>
                 {
                     ScaleTo.Create(txt.gameObject, 0.03f, 1f, 1f).OnComptele = () =>
                     {

                     };
                 };
             };
         };

         */

        ScaleTo.Create(txt.gameObject, 0.01f, 3.0f, 3.0f).OnComptele = () =>
        {
            ScaleTo.Create(txt.gameObject, 0.03f, 1.9f, 1.9f).OnComptele = () =>
            {
                ScaleTo.Create(txt.gameObject, 0.02f, 1.2f, 1.2f).OnComptele = () =>
                {
                    ScaleTo.Create(txt.gameObject, 0.01f, 1f, 1f).OnComptele = () =>
                    {

                    };
                };
            };
        };

    }
    public override void UpdateMS()
    {
        if (tick_hide.Tick())
        {
            return;
        }
        txt.gameObject.SetActive(false);
    }
    public override bool Init()
    {
        base.Init();

        this.txt = GameObject.Find("combo").GetComponent<Text>();
        txt.gameObject.SetActive(false);
        return true;
    }

    private int combo_cache = 0;
    private Text txt;
    private Counter tick_hide = Counter.Create(40);

}

public sealed class UI_die : ViewUI
{
    private bool isDie = false;
    Counter tick = Counter.Create(120); // 30  s
    public override void Update()
    {

    }
    public override void OnEvent(int type, object userData)
    {
        ///  if (PublicData.ins.is_pve) return;
        if (type == Events.ID_DIE)
        {
            if (userData as Hero != HeroMgr.ins.self || PublicData.ins.left_revive_times <= 0)
            {
                return;
            }
            this.panel.SetActive(true);
            txt_info.text = "角色已死亡，复活倒计时";
            isDie = true;
            tick.Reset();
        }
    }
    public override void UpdateMS()
    {
        ///   if (PublicData.ins.is_pve) return;
        if (HeroMgr.ins.self != null && HeroMgr.ins.self.isDie == false) return;
        if (tick.Tick())
        {
            float t = (float)(tick.GetMax() - tick.GetCurrent()) / 40.0f;
            int time = (int)t;
            txt_info.text = "角色已死亡，复活剩余时间:" + time.ToString() + " 秒";
            return;
        }
        OnClick(1);
        return;
        txt_info.text = "请选择复活点，复活角色";

    }
    public override bool Init()
    {
        base.Init();

        this.panel = GameObject.Find("ui_panel_die");
        this.panel.transform.localScale = new Vector3(1f, 1f, 1f);
        this.btn_p1 = panel.transform.FindChild("btn_p1").GetComponent<Button>();
        this.btn_p2 = panel.transform.FindChild("btn_p2").GetComponent<Button>();
        this.btn_p3 = panel.transform.FindChild("btn_p3").GetComponent<Button>();
        this.btn_p4 = panel.transform.FindChild("btn_p4").GetComponent<Button>();
        this.txt_info = panel.transform.FindChild("info").GetComponent<Text>();

        this.btn_p1.gameObject.SetActive(false);
        this.btn_p2.gameObject.SetActive(false);
        this.btn_p3.gameObject.SetActive(false);
        this.btn_p4.gameObject.SetActive(false);

        this.btn_p1.onClick.AddListener(() =>
        {
            this.OnClick(1);
        });
        this.btn_p2.onClick.AddListener(() =>
        {
            this.OnClick(2);
        });
        this.btn_p3.onClick.AddListener(() =>
        {
            this.OnClick(3);
        });
        this.btn_p4.onClick.AddListener(() =>
        {
            this.OnClick(4);
        });

        EventDispatcher.ins.AddEventListener(this, Events.ID_DIE);

        this.panel.SetActive(false);

        return true;
    }
    private void OnClick(int index)
    {
        if (tick.IsMax())
        {
            ///执行复活动作
            ///      Debug.Log("POINT " + index);
            this.panel.SetActive(false);
            PublicData.ins.IS_revive_point = index;
        }
    }

    GameObject panel = null;
    Text txt_info = null;

    Button btn_p1 = null;
    Button btn_p2 = null;
    Button btn_p3 = null;
    Button btn_p4 = null;

}

public class UI_dirInput : ViewUI
{
    bool over = false;
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_BATTLE_PVP_WAITFOR_RESULT_SHOW)
        {
            ui.SetActive(false);
            over = true;
        }
    }
    public override void Update()
    {
        if (over) return;
        base.Update();
        //  this.ui.SetActive(_enable);


        Hero self = HeroMgr.ins.self;
        if (self == null) return;
        if (self.isDie || PublicData.ins.battle_result != BattleResult.UnKnown)
        {//死亡后不显示摇杆
            this.ui.SetActive(false);
        }
        else
        {
            this.ui.SetActive(true);
        }

        if (this.ui.activeSelf != model._enable)
        {
            ///  this.ui.SetActive(model._enable);
        }
        if (model._enable == false)
        {
            this.img_arrow.gameObject.SetActive(false);
        }
        else
        {
            this.img_arrow.gameObject.SetActive(true);
        }
        //  if (model._enable)
        {
            if (model.move)
            {
                img_bg.color = new Color(1f, 1f, 1f, 0.78f);
                img_center.color = new Color(1f, 1f, 1f, 0.78f);       
            }
            else
            {
                img_bg.color = new Color(1f, 1f, 1f, 0.176f);
                img_center.color = new Color(1f, 1f, 1f, 0.176f);
            }
            //比例变换
            float factor_x = 1f / (Screen.width / 1136f);
            float factor_y = 1f / (Screen.height / 640f);
            this.img_arrow.transform.localPosition = new Vector3(model.x_arrow * factor_x, factor_y * model.y_arrow, this.img_arrow.transform.localPosition.z);

            this.img_arrow.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, model.rotateZ_arrow));

            this.img_center.transform.localPosition = new Vector3(model.x_center * factor_x, factor_y * model.y_center, this.img_center.transform.localPosition.z);

            //  this.img_bg.transform.localPosition = new Vector3(model.x_bg , model.y_bg, this.img_bg.transform.localPosition.z);   
            this.img_bg.transform.localPosition = new Vector3(model.x_bg * factor_x, model.y_bg * factor_y, this.img_bg.transform.localPosition.z);

        }

    }
    public override bool Init()
    {

        this.ui = PrefabsMgr.Load(DATA.UI_PREFABS_FILE_DIRINPUT);
        ///  this.ui.transform.parent = this._ui_root.transform;
        DirInput m = ModelMgr.Create<DirInput>() as DirInput;


        this.BindModel(m);


        base.Init();

        //   this.ui = GameObject.Find("DirectionInput");
        this.model = base._model as DirInput;


        this.img_bg = this.ui.transform.FindChild("DirectionInput/bg").GetComponent<Image>();
        this.img_arrow = this.ui.transform.FindChild("DirectionInput/bg/arrow").GetComponent<Image>();
        this.img_center = this.ui.transform.FindChild("DirectionInput/center").GetComponent<Image>();

        EventDispatcher.ins.AddEventListener(this, Events.ID_BATTLE_PVP_WAITFOR_RESULT_SHOW);

        // EventDispatcher.ins.AddEventListener(this, "ok");
        return true;
    }

    public override void OnExit()
    {
        PrefabsMgr.Destroy(this.ui);
        base.OnExit();
    }

    private GameObject ui;
    //  private bool _enable_cache = false;
    private Image img_arrow;
    private Image img_bg;
    private Image img_center;
    private DirInput model;
}

public sealed class UI_smallmap : ViewUI
{

    public override void Update()
    {
        base.Update();
        //更新enemy 信息
        this.Sync(ref list_ememy, EnemyMgr.ins.GetEnemys(), template_enemy);
        //更新hero 信息
        this.Sync(ref list_hero, HeroMgr.ins.GetHeros(), template_hero2);
        //更新building 信息
        this.Sync(ref list_building, BuildingMgr.ins.GetBuildings(), template_building);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="list">缓存数组</param>
    /// <param name="objs">Entity 源信息数组</param>
    /// <param name="template">模板</param>
    private void Sync(ref ArrayList list, ArrayList objs, GameObject template)
    {
        Terrain terrain = AppMgr.GetCurrentApp<BattleApp>().GetCurrentWorldMap().GetTerrain();

        //更新 信息
        if (objs.Count > list.Count)//扩容
        {
            for (int ii = list.Count; ii < objs.Count; ii++)
            {
                list.Add(GameObject.Instantiate(template, template.transform.parent));
            }
        }
        int i = 0;
        for (i = 0; i < objs.Count; i++) // 更新
        {
            Entity e = objs[i] as Entity;
            GameObject obj = list[i] as GameObject;
            obj.SetActive(true);
            this.SetPosition(obj, e.x / (terrain.limit_x_right - terrain.limit_x_left), e.z / terrain.limit_z_up);
        }
        for (; i < list.Count; i++) // 隐藏多余的缓存图块
        {
            GameObject obj = list[i] as GameObject;
            obj.SetActive(false);
        }
    }
    private void SetPosition(GameObject obj, float x_ratio, float y_ratio)
    {   //小地图背景 大小是 200*100

        float x = (1136f - 200f) + (x_ratio * 200f);
        float y = (640f - 100f) + (y_ratio * 100f);

        x *= Config.SCREEN_SCALE_X;
        y *= Config.SCREEN_SCALE_Y;
        obj.transform.position = new Vector3(x, y, 0);
    }
    public override void UpdateMS()
    {

    }
    public override bool Init()
    {
        base.Init();

        this._ui = GameObject.Find("UI_Battle/ui_panel_smallmap");
        this.img_bg = _ui.transform.FindChild("bg").GetComponent<Image>();
        this.template_hero2 = _ui.transform.FindChild("hero2_template").gameObject;
        this.template_hero2.SetActive(false);

        this.template_enemy = _ui.transform.FindChild("enemy_template").gameObject;
        this.template_enemy.SetActive(false);

        this.template_building = _ui.transform.FindChild("building_template").gameObject;
        this.template_building.SetActive(false);

        return true;
    }

    ArrayList list_ememy = new ArrayList();
    ArrayList list_hero = new ArrayList();
    ArrayList list_building = new ArrayList();

    GameObject template_hero2 = null;
    GameObject template_enemy = null;
    GameObject template_building = null;

    Image img_bg = null;
}



public sealed class UI_tower : ViewUI
{
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_BATTLE_SHOW_TOWER_PANEL)
        {
            this.Show();

        }
        else if (type == Events.ID_BATTLE_HIDE_TOWER_PANEL)
        {
            this.Hide();
        }
    }
    public override bool Init()
    {
        base.Init();

        this._ui = GameObject.Find("UI_Battle/ui_panel_tower");
        this.btn_launch = _ui.transform.FindChild("btn_launch").GetComponent<Button>();

        this.btn_launch.onClick.AddListener(delegate()
        {
            if (PublicData.ins.inputAble == false) return;
            PublicData.ins.IS_opt = FrameCustomsOpt.LaunchDefendTower;
        });

        EventDispatcher.ins.AddEventListener(this, Events.ID_BATTLE_SHOW_TOWER_PANEL);
        EventDispatcher.ins.AddEventListener(this, Events.ID_BATTLE_HIDE_TOWER_PANEL);


        for (int i = 0; i <= MAX_BUFFER_SHOW; i++)
        {
            string name = "Tower" + i.ToString();

            Transform buf = this._ui.transform.FindChild(name);

            Image img = buf.FindChild("icon").GetComponent<Image>();
            Text txt = buf.GetComponentInChildren<Text>();
            Image img_fill = buf.FindChild("filled").GetComponent<Image>();
            Text txt_time = buf.FindChild("time").GetComponent<Text>();
            this.list_img.Add(img);
            this.list_txt.Add(txt);
            this.list_img_filled.Add(img_fill);
            this.list_txt_time.Add(txt_time);

            buf.gameObject.SetActive(false);
            this.list_panel.Add(buf.gameObject);

        }
        if (list_img.Count != list_txt.Count)
        {
            throw new ArgumentNullException("");
        }


        this.Hide();
        return true;
    }
    public override void OnHide()
    {
        this.btn_launch.gameObject.SetActive(false);
    }
    public override void OnShow()
    {
        this.btn_launch.gameObject.SetActive(true);
    }
    private void On_BtnClicked(int e)
    {
        //   EventDispatcher.ins.PostEvent(e);

        //  HeroMgr.ins.GetSelfHero().eventDispatcher.PostEvent(e);
    }


    Button btn_launch;




    //------------- code copy from ui_buffers
    public override void Update()
    {
        base.Update();

        if (tick.Tick()) return;
        tick.Reset();

        ArrayList buffers = BuildingMgr.ins.GetBuildings<DefendTower>();
        int index = 0;
        foreach (DefendTower buffer in buffers)
        {
            if (index > MAX_BUFFER_SHOW) break;

            (list_panel[index] as GameObject).SetActive(true);
            if (buffer.IsMaxPowerLevel() == false)
            {
                Counter t = buffer.tick_power;

                float current = t.GetCurrent();
                float max = t.GetMax();
                (list_img_filled[index] as Image).fillAmount = current / max;
                float timef = ((max - current) / (float)Config.MAX_FPS);

                int time = (int)((max - current) / Config.MAX_FPS);
                if (timef < 1.0f)
                {
                    (list_txt_time[index] as Text).text = timef.ToString("0.0");
                }
                else
                {
                    (list_txt_time[index] as Text).text = time.ToString();
                }
            }
            else
            {
                (list_img_filled[index] as Image).fillAmount = 0;
                (list_txt_time[index] as Text).text = "";
            }
            (list_txt[index] as Text).text = "防御塔x" + buffer.power_level;
            Sprite sp = SpriteFrameCache.ins.GetSpriteFrameAuto("hd/interface/items/503093.png").sprite;
            (list_img[index] as Image).sprite = sp;
            ++index;
        }

        for (int i = index; i <= MAX_BUFFER_SHOW; i++)
        {
            (list_panel[i] as GameObject).SetActive(false);

        }
        if (buffers.Count <= 0)
        {
            this.Hide();
        }

    }


    Counter tick = Counter.Create(0);
    private const int MAX_BUFFER_SHOW = 1;
    ArrayList list_panel = new ArrayList();//GameObject
    ArrayList list_img = new ArrayList();
    ArrayList list_txt = new ArrayList();
    ArrayList list_txt_time = new ArrayList();
    ArrayList list_img_filled = new ArrayList();
    GameObject panel = null;


}



public sealed class UI_boss : ViewUI
{
    public override void OnHide()
    {
        panel.SetActive(false);
    }
    public override void OnShow()
    {
        panel.SetActive(true);
    }
    public override void Update()
    {// code copy from UI_buffers
        base.Update();

        if (tick.Tick()) return;
        tick.Reset();
        if (boss == null)
        {
            boss = EnemyMgr.ins.GetEnemy<EnemyBoss>();
            this.Hide();
            return;
        }
        this.Show();
        ArrayList buffers = boss.bufferMgr.GetBuffers();

        int index = 0;
        foreach (Buffer buffer in buffers)
        {
            if (!buffer.show_ui) continue;
            if (index > MAX_BUFFER_SHOW) break;

            (list_panel[index] as GameObject).SetActive(true);
            if (buffer.enable_time)
            {
                Counter t = buffer.GetCounter();

                float current = t.GetCurrent();
                float max = t.GetMax();
                (list_img_filled[index] as Image).fillAmount = current / max;
                float timef = ((max - current) / (float)Config.MAX_FPS);

                int time = (int)((max - current) / Config.MAX_FPS);
                if (timef < 1.0f)
                {
                    (list_txt_time[index] as Text).text = timef.ToString("0.0");
                }
                else
                {
                    (list_txt_time[index] as Text).text = time.ToString();
                }
            }
            else
            {
                (list_img_filled[index] as Image).fillAmount = 0;
                (list_txt_time[index] as Text).text = "";
            }
            (list_txt[index] as Text).text = buffer.brief;
            Sprite sp = SpriteFrameCache.ins.GetSpriteFrameAuto(buffer.icon).sprite;
            (list_img[index] as Image).sprite = sp;
            ++index;
        }

        for (int i = index; i <= MAX_BUFFER_SHOW; i++)
        {
            (list_panel[i] as GameObject).SetActive(false);

        }

        txt_hp.text = boss.current_hp.ToString() + " / " + boss.hp.ToString();
        img_hp.gameObject.transform.localScale = new Vector3(Utils.RangeLimit((float)boss.current_hp / (float)boss.hp, 0f, 1f),
            img_hp.gameObject.transform.localScale.y, img_hp.gameObject.transform.localScale.z);
        txt_info.text = "Boss:唐僧 LV:10" + "   敌人总数:" + EnemyMgr.ins.GetEnemyCount().ToString();
    }

    public override bool Init()
    {
        base.Init();
        this.panel = GameObject.Find("ui_panel_boss");
        this.panel.transform.localScale = new Vector3(1f, 1f, 1f);

        for (int i = 0; i <= MAX_BUFFER_SHOW; i++)
        {
            string name = "Buffer" + i.ToString();

            Transform buf = this.panel.transform.FindChild(name);

            Image img = buf.FindChild("icon").GetComponent<Image>();
            Text txt = buf.GetComponentInChildren<Text>();
            Image img_fill = buf.FindChild("filled").GetComponent<Image>();
            Text txt_time = buf.FindChild("time").GetComponent<Text>();
            this.list_img.Add(img);
            this.list_txt.Add(txt);
            this.list_img_filled.Add(img_fill);
            this.list_txt_time.Add(txt_time);

            buf.gameObject.SetActive(false);
            this.list_panel.Add(buf.gameObject);

        }
        txt_hp = panel.transform.FindChild("panel_boss/txt_hp").GetComponent<Text>();
        txt_info = panel.transform.FindChild("panel_boss/txt_info").GetComponent<Text>();
        img_hp = panel.transform.FindChild("panel_boss/img_hp").GetComponent<Image>();

        if (list_img.Count != list_txt.Count)
        {
            throw new ArgumentNullException("");
        }
        return true;
    }

    Counter tick = Counter.Create(0);
    private const int MAX_BUFFER_SHOW = 8;
    ArrayList list_panel = new ArrayList();//GameObject
    ArrayList list_img = new ArrayList();
    ArrayList list_txt = new ArrayList();
    ArrayList list_txt_time = new ArrayList();
    ArrayList list_img_filled = new ArrayList();
    GameObject panel = null;

    Text txt_hp = null;
    Image img_hp;
    Text txt_info = null;
    EnemyBoss boss = null;
}


public sealed class UI_hps : ViewUI
{
    public override void Update()
    {
        this.Sync(EnemyMgr.ins.GetEnemys(), imgs_ememy, template_enemy);
        this.Sync(BuildingMgr.ins.GetBuildings(), imgs_buildings, template_building);
    }
    private void Sync(ArrayList list, ArrayList list_ui, GameObject template)
    {
        while (list.Count > list_ui.Count)
        {
            list_ui.Add((GameObject.Instantiate(template, _panel.transform) as GameObject));
        }
        int i = 0;
        foreach (Entity hero in list)
        {
            if (hero.IsEnemyBoss) continue;
            GameObject obj = list_ui[i] as GameObject;
            obj.SetActive(true);
            obj.transform.FindChild("hp").transform.localScale = new Vector3(Utils.RangeLimit((float)hero.current_hp / (float)hero.hp, 0f, 1f), 1f, 1f);
            obj.gameObject.transform.position = Camera.main.WorldToScreenPoint(
                new Vector3(hero.x, hero.GetReal25DY() + 1.2f, 0));
            ++i;
        }
        for (; i < list_ui.Count; i++)
        {
            (list_ui[i] as GameObject).SetActive(false);
        }
    }
    public override bool Init()
    {
        base.Init();

        this._panel = GameObject.Find("UI_Battle/ui_panel_hps").gameObject;
        this.template_enemy = _panel.transform.FindChild("template_enemy").gameObject;
        this.template_enemy.SetActive(false);

        this.template_building = _panel.transform.FindChild("template_building").gameObject;
        this.template_building.SetActive(false);
        return true;
    }


    private ArrayList imgs_ememy = new ArrayList();
    GameObject template_enemy;

    private ArrayList imgs_buildings = new ArrayList();
    GameObject template_building;
    GameObject _panel;
}

