/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


public sealed class UITownRoot : ViewUI
{

    public override void Update()
    {
        base.Update();
    }

    public override bool Init()
    {
        base.Init();

        // this._ui_root = PrefabsMgr.Load(DATA.UI_PREFABS_FILE_BATTLE);
        this._ui_root = GameObject.Find("UI_Town");

        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_Townxy>(this));
            return DATA.EMPTY_STRING;
        }));


        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_component>(this));

            return DATA.EMPTY_STRING;
        }));


        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_time>(this));
            return DATA.EMPTY_STRING;
        }));


        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_names>(this));

            return DATA.EMPTY_STRING;
        }));

        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_worldchatapp>(this));

            return DATA.EMPTY_STRING;
        }));



        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_townmenuapp>(this));

            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_townteamapp>(this));

            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_equipsystemapp>(this));

            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_friendsapp>(this));

            return DATA.EMPTY_STRING;
        }));

        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_townpvpapp>(this));

            return DATA.EMPTY_STRING;
        }));

        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_trumpapp>(this));

            return DATA.EMPTY_STRING;
        }));

        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_trump_levelupapp>(this));

            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_backpackapp>(this));

            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_settingapp>(this));

            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_afficheapp>(this));

            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.PostEvent("addAsync", new Func<string>(() =>
        {
            this._ui_child.Add(ViewUI.Create<UI_characterinfoapp>(this));

            return DATA.EMPTY_STRING;
        }));
        EventDispatcher.ins.AddEventListener(this, Events.ID_VIEW_NEW_CELLAPP_VIEW);


        return true;
    }


    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_VIEW_NEW_CELLAPP_VIEW)
        {

        }
    }



    private void AddCellApp(UICellApp view)
    {

    }
    public override void OnExit()
    {

        UnityEngine.Object.Destroy(this._ui_root);

        base.OnExit();
    }


    protected ArrayList _ui_child = new ArrayList();

}

public sealed class UI_Townxy : ViewUI
{


    public override void Update()
    {
        base.Update();

        var self = HeroMgr.ins.self;
        if (self != null)
        {
            txt.text = string.Format(DATA.UI_INFO_XY, (self.x * 100.0f).ToString("0"), (self.y * 100.0f).ToString("0"));
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




public sealed class UI_time : ViewUI
{
    private Counter tick = Counter.Create();

    public override void Update()
    {
        if (tick.Tick()) return;

        base.Update();

        var time = System.DateTime.Now;

        int min = time.Minute;
        int hour = time.Hour;
        int sec = time.Second;

        string s_sec, s_min, s_hour;
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
        if (hour < 10)
        {
            s_hour = "0" + hour.ToString();
        }
        else
        {
            s_hour = hour.ToString();
        }
        txt.text = " 时间: " + s_hour + ":" + s_min + ":" + s_sec;

    }

    public override bool Init()
    {
        base.Init();

        this.txt = GameObject.Find("time").GetComponent<Text>();

        return true;
    }


    private Text txt;

}


public sealed class UI_names : ViewUI
{
    public override void Update()
    {//TODO 优化性能

        ArrayList heros = HeroMgr.ins.GetHeros();

        if (heros.Count == 0) return;

        while (heros.Count > texts.Count)
        {
            texts.Add((GameObject.Instantiate(txt_template, _panel.transform) as GameObject).
                GetComponent<Text>());
        }

        int i = 0;

        for (i = 0; i < heros.Count; i++)
        {
            Text txt = texts[i] as Text;
            Hero hero = heros[i] as Hero;

            txt.text = hero.name;

            txt.gameObject.transform.position = Camera.main.WorldToScreenPoint(
                new Vector3(hero.x, hero.y + 1.2f, 0));

            if (hero != HeroMgr.ins.self)
            {
                txt.color = Color.white;
            }
            else
            {
                txt.color = color;
            }
        }
        for (; i < texts.Count; i++)
        {
            (texts[i] as Text).text = "";
        }

    }

    public override bool Init()
    {
        base.Init();

        this._panel = this._root._ui_root.transform.FindChild("ui_panel_hero_names").gameObject;
        this.txt_template = _panel.transform.FindChild("NameTemplate").gameObject;
        color = this.txt_template.GetComponent<Text>().color;
        return true;
    }


    private ArrayList texts = new ArrayList();
    GameObject _panel;
    GameObject txt_template;
    Color color;
}



public class UICellApp : ViewUI
{
    public virtual void Show()
    {

    }
    public virtual void Hide()
    {

    }

    protected void PopIn()
    {
        this.panel.SetActive(true);
        ScaleTo.Create(this.panel, 0.05f, 0.7f, 0.7f).OnComptele = () =>
        {
            ScaleTo.Create(this.panel, 0.01f, 0.9f, 0.9f).OnComptele = () =>
            {
                ScaleTo.Create(this.panel, 0.01f, 1.2f, 1.2f).OnComptele = () =>
                {
                    ScaleTo.Create(this.panel, 0.03f, 1f, 1f).OnComptele = () =>
                    {

                    };
                };
            };
        };
    }
    protected void PopOut()
    {
        ScaleTo.Create(this.panel, 0.1f, 0.0f, 0.0f).OnComptele = () =>
        {
            if (this.panel.gameObject.GetComponent<Actions>() == null)
            {
                this.panel.SetActive(false);
            }
        };
    }

    protected void LoadWithPrefabs(string file)
    {
        this.panel = PrefabsMgr.Load(file);
        this.panel.transform.parent = this._root._ui_root.transform;
        this.panel.transform.localPosition = Vector3.zero;
    }

    /* public void BindCellApp(CellApp app)
     {
         this.app = app;
     }

     protected CellApp app = null;*/

    protected GameObject panel = null;


}


public sealed class UI_worldchatapp : UICellApp
{


    public override void Update()
    {
        base.Update();

        if (cell_counter.Tick() == false)
        {
            this.hideSmall();
        }
    }

    private void hideSmall()
    {
        cell_bg.SetActive(false);
        this.cell_txt.text = "";

    }
    private int current_size = 0;

    public override bool Init()
    {
        base.Init();

        this.cell_txt = GameObject.Find("worldcht_cell_txt").GetComponent<Text>();
        this.obj_whold = GameObject.Find("worldchat_whole");

        this.cell_bg = GameObject.Find("worldchat_cell_bg");
        this.cell_btn = GameObject.Find("worldchat_cell_btn").GetComponent<Button>();
        this.list_content = GameObject.Find("list_content").GetComponent<RectTransform>();

        this.list_btn_send = GameObject.Find("worldchat_btn_send").GetComponent<Button>();
        this.list_btn_close = GameObject.Find("worldchat_btn_close").GetComponent<Button>();
        this.scroll = GameObject.Find("worldchat_scrollview").GetComponent<ScrollRect>();

        this.input = GameObject.Find("worldchat_inputfield").GetComponent<InputField>();

        this.template_copy = GameObject.Instantiate(this.list_content.FindChild("one").gameObject, this.list_content.transform) as GameObject;


        this.template_copy.SetActive(false);

        this.list_content.FindChild("one").gameObject.SetActive(false);

        this.list_btn_send.onClick.AddListener(() =>
        {

            if (input.text != "")
            {
                EventDispatcher.ins.PostEvent(Events.ID_WORLDCHAT_SEND_BTN_CLICKED, input.text);
                input.text = "";
            }

        });


        this.list_btn_close.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_WORLDCHAT_CLOSE_BTN_CLICKED, this);


        });


        this.cell_btn.onClick.AddListener(() =>
            {

                EventDispatcher.ins.PostEvent(Events.ID_WORLDCHAT_CELL_BTN_CLICKED, this);
            });


        this.hideSmall();

        this.obj_whold.SetActive(false);

        EventDispatcher.ins.AddEventListener(this, Events.ID_RPC_WORLD_CHAT_NEW_MSG);
        return true;
    }


    private void ReSize()
    {
        //re position
        int i = this.lists.Count;
        foreach (GameObject one in this.lists)
        {
            one.transform.localPosition = new Vector3(one.transform.localPosition.x, 0 + i * 60, one.transform.localPosition.z);

            i--;
        }
    }
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_RPC_WORLD_CHAT_NEW_MSG)
        {//新消息通知

            if (current_size >= MAX_REMAIN_COUNTS)
            {

                GameObject.Destroy(lists[0] as GameObject);
                lists.RemoveAt(0);
                current_size -= 1;

            }
            cell_bg.SetActive(true);
            cell_counter.Reset();
            var hash = userData as HashTable;

            cell_txt.text = packMsg(hash["type"], hash["name"], hash["msg"]);


            string time1 = hash["time"];

            string time2 = hash["time2"];
            time2 = time2.Replace("-", ":");

            GameObject obj = GameObject.Instantiate(this.template_copy, this.list_content) as GameObject;
            obj.SetActive(true);
            lists.Add(obj);
            current_size += 1;
            float max_height = current_size * 60;

            this.list_content.sizeDelta = new Vector2(0, max_height);


            Text name = obj.transform.FindChild("worldchat_cell_txt_head").transform.gameObject.GetComponent<Text>();
            name.text = "<color=green>[世界]" + "</color><color=#9420D2FF>" + hash["name"] + ":</color>";// +current_size.ToString();

            Text txt_time = obj.transform.FindChild("worldchat_cell_txt_time").transform.gameObject.GetComponent<Text>();
            txt_time.text = "<color=#9420D2FF>" + time1 + " " + time2 + "</color>";


            Text txt_msg = obj.transform.FindChild("worldchat_cell_txt").transform.gameObject.GetComponent<Text>();
            txt_msg.text = hash["msg"];
            float scale = 1.0f;

            scale = (txt_msg.text.Length + 1) / 25.0f;

            obj.transform.FindChild("worldchat_cell_bg1").transform.localScale = new Vector3(1.0f, scale, 1.0f);


            obj.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);

            this.ReSize();

        }
    }



    /// <summary>
    /// 显示完整的界面
    /// </summary>
    public override void Show()
    {
        this.obj_whold.SetActive(true);
        ScaleTo.Create(this.obj_whold, 0.1f, 1.0f).OnComptele = () =>
        {

            scroll.verticalScrollbar.value = 0;
        };

    }
    public override void Hide()
    {

        ScaleTo.Create(this.obj_whold, 0.1f, 0.0f).OnComptele = () =>
            {
                this.obj_whold.SetActive(false);

            };

    }

    private string packMsg(string type, string name, string msg)
    {
        return makeColor(type, "green") + makeColor(name + ":", "#9420D2FF") + makeColor(msg, "white");
    }

    private string makeColor(string what, string color)
    {
        return "<color=" + color + ">" + what + "</color>";
    }



    private GameObject obj_whold = null;
    private Text txt;
    private Text cell_txt = null;
    private GameObject cell_bg = null;
    private Button cell_btn = null;

    private RectTransform list_content = null;// list view 的 content
    private Button list_btn_send = null;
    private Button list_btn_close = null;

    private GameObject template_copy = null;
    ArrayList lists = new ArrayList();//消息GameObject 容器

    private InputField input = null;
    private Counter cell_counter = Counter.Create(400);//显示  10 秒
    private ScrollRect scroll = null;

    private int MAX_REMAIN_COUNTS = 10; //最大消息保留条数
}


public sealed class UI_townmenuapp : UICellApp
{
    public override void Update()
    {
        base.Update();
    }

    public override bool Init()
    {
        base.Init();


        this.btn_close = GameObject.Find("btn_menu_close").GetComponent<Button>();
        this.btn_show = GameObject.Find("btn_menu_show").GetComponent<Button>();
        this.panel = GameObject.Find("ui_panel_menus");

        this.btn_friends = GameObject.Find("btn_friend").GetComponent<Button>();
        this.btn_equipsystem = GameObject.Find("btn_equipstrength").GetComponent<Button>();

        this.btn_backpack = GameObject.Find("btn_pack").GetComponent<Button>();
        this.btn_trump = GameObject.Find("btn_magic").GetComponent<Button>();
        this.btn_setting = GameObject.Find("btn_set").GetComponent<Button>();
        this.btn_character_info = GameObject.Find("btn_maininfo").GetComponent<Button>();
        this.btn_close.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_MENU_CLOSE_CLICKED, this);


        });


        this.btn_show.onClick.AddListener(() =>
        {

            EventDispatcher.ins.PostEvent(Events.ID_TOWN_MENU_CLICKED, this);
        });

        this.btn_friends.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_MENU_CLOSE_CLICKED, this);

            EventDispatcher.ins.PostEvent(Events.ID_TOWN_FRIENDS_CLICK);

        });

        this.btn_backpack.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_MENU_CLOSE_CLICKED, this);
            EventDispatcher.ins.PostEvent(Events.ID_INFO_BACKPACK);

        });
        this.btn_trump.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_MENU_CLOSE_CLICKED, this);
            EventDispatcher.ins.PostEvent(Events.ID_INFO_TRUMP);

        });
        this.btn_setting.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_MENU_CLOSE_CLICKED, this);
            EventDispatcher.ins.PostEvent(Events.ID_INFO_SETTING);

        });
        this.btn_character_info.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_MENU_CLOSE_CLICKED, this);
            EventDispatcher.ins.PostEvent(Events.ID_INFO_CHARACTER_INFO);
        });
        this.btn_equipsystem.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_MENU_CLOSE_CLICKED, this);
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_EQUIPSYSTEM_BTN_OTHER_SHOW_CLICKED);
        });
        this.Hide();
        return true;
    }



    public override void OnEvent(int type, object userData)
    {

    }



    /// <summary>
    /// 显示完整的界面
    /// </summary>
    public override void Show()
    {
        this.panel.SetActive(true);

        ScaleTo.Create(this.panel, 0.1f, 1f, 1f).OnComptele = () =>
        {

        };

    }
    public override void Hide()
    {

        ScaleTo.Create(this.panel, 0.1f, 1f, 0.0f).OnComptele = () =>
        {
            this.panel.SetActive(false);

        };

    }

    private Button btn_character_info = null;
    private Button btn_setting = null;
    private Button btn_trump = null;
    private Button btn_backpack = null;
    private Button btn_close = null;
    private Button btn_show = null;
    private Button btn_friends = null;
    private Button btn_equipsystem = null;

}







public sealed class UI_friendsapp : UICellApp
{
    public override void Update()
    {
        base.Update();
    }

    public override bool Init()
    {
        base.Init();

        this.panel = GameObject.Find("ui_panel_friends");


        // init friends list
        this.panel_list = panel.transform.FindChild("friends_list");

        this.btn_close = panel_list.transform.FindChild("btn_friends_close").GetComponent<Button>();
        this.btn_add = panel_list.transform.FindChild("btn_add_friends").GetComponent<Button>();

        this.list_content = GameObject.Find("friends_list_content").GetComponent<RectTransform>();

        this.txt_count = panel_list.transform.FindChild("txt_friends_count").GetComponent<Text>();


        // init friend detail

        this.panel_detail = panel.transform.FindChild("friends_detail");

        this.btn_detail_close = panel_detail.transform.FindChild("btn_friends_detail_close").GetComponent<Button>();
        this.btn_detail_delete = panel_detail.transform.FindChild("btn_friends_delete").GetComponent<Button>();
        this.btn_detail_send = panel_detail.transform.FindChild("btn_friends_sendmsg").GetComponent<Button>();
        this.btn_detail_pk = panel_detail.transform.FindChild("btn_friends_pk").GetComponent<Button>();


        this.txt_detail_name = panel_detail.transform.FindChild("base").FindChild("base_txt_name").GetComponent<Text>();
        this.txt_detail_level = panel_detail.transform.FindChild("base").FindChild("base_txt_level").GetComponent<Text>();
        this.txt_detail_time = panel_detail.transform.FindChild("base").FindChild("base_txt_time").GetComponent<Text>();



        // init friend srarch

        this.panel_search = panel.transform.FindChild("friends_search");
        this.btn_search_add = this.panel_search.transform.FindChild("btn_search_friends").GetComponent<Button>();
        this.btn_search_close = this.panel_search.transform.FindChild("btn_friends_search_close").GetComponent<Button>();

        this.input_name = this.panel_search.transform.FindChild("friends_search_inputfield_name").GetComponent<InputField>();
        this.input_no = this.panel_search.transform.FindChild("friends_search_inputfield_no").GetComponent<InputField>();



        this.template_copy = GameObject.Instantiate(this.list_content.FindChild("one").gameObject, this.list_content.transform) as GameObject;

        this.template_copy.SetActive(false);

        this.list_content.FindChild("one").gameObject.SetActive(false);




        this.btn_add.onClick.AddListener(() =>
        {//添加好友

            this.panel_list.gameObject.SetActive(false);
            this.panel_search.gameObject.SetActive(true);

        });



        this.btn_close.onClick.AddListener(() =>
        { // 关闭好友列表
            EventDispatcher.ins.PostEvent(Events.ID_FRIENDS_CLOSE_CLICKED, this);


        });
        this.btn_detail_delete.onClick.AddListener(() =>
        { // 删除好友
            Debug.Log("删除好友");

            EventDispatcher.ins.PostEvent(Events.ID_FRIENDS_DELETE_CLICKED, this);

        });
        this.btn_detail_send.onClick.AddListener(() =>
        { // 私聊好友

            Debug.Log("私聊好友");
        });
        this.btn_detail_pk.onClick.AddListener(() =>
        { // 切磋
            Debug.Log("切磋好友");
            EventDispatcher.ins.PostEvent(Events.ID_FRIENDS_PVP_CLICKED, current_detail_user);

        });

        this.btn_detail_close.onClick.AddListener(() =>
        { // 关闭好友详细详细
            this.panel.SetActive(true);
            this.panel_detail.gameObject.SetActive(false);

            this.panel_list.gameObject.SetActive(true);

        });

        this.btn_search_close.onClick.AddListener(() =>
        { // 关闭搜索
            this.panel_search.gameObject.SetActive(false);
            this.panel_list.gameObject.SetActive(true);

        });
        this.btn_search_add.onClick.AddListener(() =>
            {

                if (input_no.text != "")
                {
                    RpcClient.ins.SendRequest("services.friends", "add_by_no", "no:" + PublicData.ins.self_user.no + ",who:" + input_no.text + ",", (string msg) =>
                    {
                        input_name.text = "";
                        input_no.text = "";
                        Debug.Log(msg);
                        HashTable kv = Json.Decode(msg);
                        if (kv["ret"] == "ok")
                        {
                            DAO.User who = DAO.User.Create(kv);


                            EventDispatcher.ins.PostEvent(Events.ID_ADD_FRIEND_SUCCESS, who);
                        }
                        else
                        {
                            EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, kv["msg"]);
                        }
                    });

                }
                else if (input_name.text != "")
                {

                    RpcClient.ins.SendRequest("services.friends", "add_by_name", "no:" + PublicData.ins.self_user.no + ",name:" + input_name.text + ",", (string msg) =>
                    {
                        input_name.text = "";
                        input_no.text = "";
                        Debug.Log(msg);
                        HashTable kv = Json.Decode(msg);
                        if (kv["ret"] == "ok")
                        {
                            DAO.User who = DAO.User.Create(kv);

                            EventDispatcher.ins.PostEvent(Events.ID_ADD_FRIEND_SUCCESS, who);
                            this.panel_search.gameObject.SetActive(false);
                            this.panel_list.gameObject.SetActive(true);
                        }
                        else
                        {
                            EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, kv["msg"]);
                        }
                    });
                }
                else
                {

                    EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "请输入玩家名字或者ID");

                }



            });



        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_FRIENDS_CLICK);
        EventDispatcher.ins.AddEventListener(this, Events.ID_FRIEND_SYNC_VIEW);



        this.panel_detail.gameObject.SetActive(false);
        this.panel_search.gameObject.SetActive(false);

        this.Hide();
        return true;
    }



    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_TOWN_FRIENDS_CLICK)
        {

            EventDispatcher.ins.PostEvent(Events.ID_FRIENDS_SHOW_CLICKED, this);


        }
        else if (type == Events.ID_FRIEND_SYNC_VIEW)
        {
            //刷新界面
            ArrayList list = userData as ArrayList;
            this.list = list;
            this.SyncList();

            this.panel_search.gameObject.SetActive(false);
            this.panel_detail.gameObject.SetActive(false);
            this.panel_list.gameObject.SetActive(true);

        }

    }


    private void ShowDetail(DAO.User user)
    {
        //show detail
        this.current_detail_user = user;

        this.panel_list.gameObject.SetActive(false);
        this.panel_detail.gameObject.SetActive(true);

        //sync ui

        this.txt_detail_level.text = user.level.ToString();
        this.txt_detail_name.text = user.name;
        this.txt_detail_time.text = user.time;

    }
    /// <summary>
    /// 显示完整的界面
    /// </summary>
    public override void Show()
    {
        this.PopIn();

    }
    public override void Hide()
    {
        this.PopOut();

    }
    private void SyncList()
    {
        int i = 0;
        this.Clear();

        for (i = 0; i < list.Count; i++)
        {
            DAO.User user = list[i] as DAO.User;


            GameObject obj = GameObject.Instantiate(this.template_copy, this.list_content) as GameObject;
            obj.SetActive(true);
            ones.Add(obj);

            float max_height = list.Count * 100;

            this.list_content.sizeDelta = new Vector2(0, max_height);


            obj.transform.FindChild("txt_name").transform.gameObject.GetComponent<Text>().text = user.name;
            obj.transform.FindChild("txt_time").transform.gameObject.GetComponent<Text>().text = user.time;
            obj.transform.FindChild("txt_level").transform.gameObject.GetComponent<Text>().text = user.level.ToString();
            obj.transform.FindChild("btn_head").transform.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {//re bind function
                //    EventDispatcher.ins.PostEvent(Events.ID_FRIENDS_DETAIL_SHOW, user);
                this.ShowDetail(user);

            });


            obj.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);

            this.ReSize();
        }
        this.txt_count.text = "好友数量:" + list.Count.ToString() + "/50";

    }

    private void ReSize()
    {
        //re position
        int i = this.ones.Count;
        foreach (GameObject one in this.ones)
        {
            one.transform.localPosition = new Vector3(one.transform.localPosition.x, -60 + i * 100, one.transform.localPosition.z);

            i--;
        }


    }
    private void Clear()
    {
        foreach (GameObject obj in ones)
        {
            GameObject.Destroy(obj);
        }
        ones.Clear();
    }


    private Button btn_close = null;
    private Button btn_add = null;
    private ArrayList list = null;
    private ArrayList ones = new ArrayList();
    private RectTransform list_content = null;
    private GameObject template_copy = null;

    private Text txt_count = null; // 好友数量

    private Transform panel_list = null; //好友列表
    private Transform panel_detail = null; // 好友详细
    private Transform panel_search = null; // 查找好友

    private Button btn_search_add = null;
    private Button btn_search_close = null;

    private InputField input_name = null;
    private InputField input_no = null;

    private Button btn_detail_close = null;
    private Button btn_detail_delete = null;
    private Button btn_detail_send = null;
    private Button btn_detail_pk = null;


    private Text txt_detail_name = null;
    private Text txt_detail_level = null;
    private Text txt_detail_time = null;

    public DAO.User current_detail_user = null;//当前显示详细信息的no


}





public sealed class UI_component : UICellApp
{
    public override void Update()
    {
        base.Update();
    }

    public override bool Init()
    {
        base.Init();

        this.panel = GameObject.Find("ui_panel_components");

        this.btn_pvp_random_queue = panel.transform.FindChild("btn_pvp").GetComponent<Button>();
        this.btn_pve_random_queue = panel.transform.FindChild("btn_activity").GetComponent<Button>();
        this.btn_town_team = panel.transform.FindChild("btn_rank").GetComponent<Button>();

        this.btn_pvp_random_queue.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_COMPONENT_BTN_PVP_QUEUE_CLICKED);
        });
        this.btn_pve_random_queue.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_COMPONENT_BTN_PVE_QUEUE_CLICKED);
        });
        this.btn_town_team.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_BTN_COMPONENT_TEAM_CLICKED);
        });
        return true;
    }
    public override void OnEvent(int type, object userData)
    {

    }

    private Button btn_pvp_random_queue = null;//进入1v1 随机匹配队列
    private Button btn_pve_random_queue = null;//进入1v1 随机匹配队列
    private Button btn_town_team = null;//组队系统

}




public sealed class UI_townpvpapp : UICellApp
{
    public override void Update()
    {
        base.Update();

        if (this.panel.activeSelf)
        {
            this.total_time += Time.deltaTime;

            int sec_total = (int)total_time;

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
            txt_time.text = (" 已等待时间: " + s_min + ":" + s_sec);

        }
    }

    public override bool Init()
    {
        base.Init();


        this.panel = GameObject.Find("ui_panel_pvp_queue");

        this.btn_close = panel.transform.FindChild("btn_close").GetComponent<Button>();
        this.txt_time = panel.transform.FindChild("txt_time").GetComponent<Text>();



        this.btn_close.onClick.AddListener(() =>
        {//离开队列
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_BTN_BATTLE_LEAVE_QUEUE_CLICKED, this);

        });

        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_COMPONENT_BTN_PVP_QUEUE_CLICKED);
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_COMPONENT_BTN_PVE_QUEUE_CLICKED);

        this.panel.SetActive(false);
        this.panel.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
        return true;
    }



    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_TOWN_COMPONENT_BTN_PVP_QUEUE_CLICKED)
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_BTN_PVP_RAMDON_QUEUE_CLICKED, this);

        }
        else if (type == Events.ID_TOWN_COMPONENT_BTN_PVE_QUEUE_CLICKED)
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_BTN_PVE_RAMDON_QUEUE_CLICKED, this);
        }
    }

    public override void Show()
    {
        total_time = 0.0f;
        this.panel.SetActive(true);

        ScaleTo.Create(this.panel, 0.1f, 1f, 1f).OnComptele = () =>
        {

        };

    }
    public override void Hide()
    {

        ScaleTo.Create(this.panel, 0.1f, 1f, 0.0f).OnComptele = () =>
        {
            this.panel.SetActive(false);

        };

    }
    private Button btn_close = null;
    private Text txt_time = null;
    private float total_time = 0.0f;
}




public sealed class UI_equipsystemapp : UICellApp
{
    public override void Update()
    {
        base.Update();
    }

    public override bool Init()
    {
        base.Init();

        this.LoadWithPrefabs("Prefabs/ui/ui_panel_equipsystem");

        this.btn_close = panel.transform.FindChild("btn_close").GetComponent<Button>();
        this.btn_strengthen = panel.transform.FindChild("btn_strengthent").GetComponent<Button>();




        this.btn_close.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_EQUIPSYSTEM_BTN_CLOSE_CLICKED, this);
        });



        this.btn_strengthen.onClick.AddListener(() =>
        {
            Debug.Log("强化");
        });



        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_EQUIPSYSTEM_BTN_OTHER_SHOW_CLICKED);
        this.panel.SetActive(false);

        this.Hide();
        return true;
    }



    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_TOWN_EQUIPSYSTEM_BTN_OTHER_SHOW_CLICKED)
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_EQUIPSYSTEM_BTN_SHOW_CLICKED, this);
        }
        else if (type == Events.ID_FRIEND_SYNC_VIEW)
        {

        }

    }



    /// <summary>
    /// 显示完整的界面
    /// </summary>
    public override void Show()
    {
        this.PopIn();

    }
    public void Show(DAO.Equip dao)
    {

    }
    public override void Hide()
    {
        this.PopOut();

    }
    private void Sync()
    {


    }

    private void ReSize()
    {
        //re position of needs

    }



    Button btn_close = null;
    Button btn_strengthen = null;
    ArrayList needs_img = new ArrayList();
    ArrayList needs_txt = new ArrayList();

}



public sealed class UI_townteamapp : UICellApp
{

    float total_time = 0f;
    enum State
    {
        UI_PVE,//主界面
        UI_INFO, // 组队信息 界面
        UI_JOIN,//加入房间界面
        UI_LIST,
    }
    State state = State.UI_PVE;
    private TownTeaminfo info = null;//reference
    public override void Update()
    {
        base.Update();


        if (state == State.UI_INFO)
        {
            this.total_time += Time.deltaTime;

            int sec_total = (int)total_time;

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
            txt_info_time.text = (" 已等待时间: " + s_min + ":" + s_sec);
        }
    }

    public override bool Init()
    {
        base.Init();

        this.panel = GameObject.Find("ui_panel_team");
        this.panel_pve = panel.transform.FindChild("panel_pve").gameObject;
        this.panel_info = panel.transform.FindChild("panel_info").gameObject;
        this.panel_join = panel.transform.FindChild("panel_join").gameObject;
        this.panel_list = panel.transform.FindChild("panel_list").gameObject;

        this.btn_close = panel.transform.FindChild("panel_pve/btn_close").GetComponent<Button>();
        this.btn_close1 = panel.transform.FindChild("panel_pve/btn_close_1").GetComponent<Button>();

        this.btn_create = panel.transform.FindChild("panel_pve/btn_create").GetComponent<Button>();
        this.btn_search = panel.transform.FindChild("panel_pve/btn_search").GetComponent<Button>();
        this.btn_join = panel.transform.FindChild("panel_pve/btn_join").GetComponent<Button>();
        this.btn_random = panel.transform.FindChild("panel_pve/btn_random").GetComponent<Button>();
        this.btn_single = panel.transform.FindChild("panel_pve/btn_single").GetComponent<Button>();

        this.players.Add(panel.transform.FindChild("panel_info/panel_player1").gameObject);
        this.players.Add(panel.transform.FindChild("panel_info/panel_player2").gameObject);
        this.btn_info_ok = panel.transform.FindChild("panel_info/btn_ok").GetComponent<Button>();
        this.btn_info_skill = panel.transform.FindChild("panel_info/btn_skill").GetComponent<Button>();
        this.btn_info_close = panel.transform.FindChild("panel_info/btn_close1").GetComponent<Button>();
        this.txt_info_time = panel.transform.FindChild("panel_info/info/txt_time").GetComponent<Text>();

        this.btn_join_ok = panel.transform.FindChild("panel_join/btn_ok").GetComponent<Button>();
        this.btn_join_close = panel.transform.FindChild("panel_join/btn_close1").GetComponent<Button>();
        this.txt_content = panel.transform.FindChild("panel_list/txt_list_count").GetComponent<Text>();

        this.btn_list_close = panel.transform.FindChild("panel_list/btn_close").GetComponent<Button>();
        this.btn_list_sync = panel.transform.FindChild("panel_list/btn_sync").GetComponent<Button>();
        this.btn_list_sync.onClick.AddListener(() =>
        {
            this.Clear();
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_BTN_MAIN_SEARCH, this);
        });
        this.btn_list_close.onClick.AddListener(() =>
        {
            this.Clear();
            this.ShowPVEPanel();
        });
        this.list_content = GameObject.Find("team_list_content").GetComponent<RectTransform>();

        //   this.txt_count = panel_list.transform.FindChild("team_list_content").GetComponent<Text>();


        this.template_copy = GameObject.Instantiate(this.list_content.FindChild("one").gameObject, this.list_content.transform) as GameObject;

        this.template_copy.SetActive(false);

        this.list_content.FindChild("one").gameObject.SetActive(false);

        this.btn_close.onClick.AddListener(() =>
        {
            this.Hide();
        });
        this.btn_close1.onClick.AddListener(() =>
        {
            this.Hide();
        });
        this.btn_random.onClick.AddListener(() =>
        {
            //  EventDispatcher.ins.PostEvent(Events.ID_TOWN_COMPONENT_BTN_PVE_QUEUE_CLICKED); //模拟 随机组队 点击 实现功能
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_BTN_MAIN_RANDOM, this);
        });
        this.btn_create.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_BTN_MAIN_CREATE, this);
        });
        this.btn_single.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_BTN_MAIN_SINGLE, this);
        });
        this.btn_join.onClick.AddListener(() =>
        {
            // EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_BTN_MAIN_JOIN, this);
            this.panel_join.transform.FindChild("input").GetComponent<InputField>().text = "";
            this.state = State.UI_JOIN;
            this.SyncPanelVisible();
        });
        this.btn_search.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_BTN_MAIN_SEARCH, this);
        });

        this.btn_info_skill.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_BTN_SKILL);   // 切换技能
        });
        this.btn_info_ok.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_BTN_OK); // 开始游戏 准备
        });
        this.btn_info_close.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_BTN_CLOSE);//退出队伍
        });

        this.btn_join_ok.onClick.AddListener(() =>
        {
            InputField inp = this.panel_join.transform.FindChild("input").GetComponent<InputField>();
            if (inp.text == "")
            {
                EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "请输入你想加入的房间号!");
                return;
            }
            join_team_no = inp.text;
            inp.text = "";
            EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_BTN_MAIN_JOIN, this);
        });
        this.btn_join_close.onClick.AddListener(() =>
        {
            this.panel_join.transform.FindChild("input/Text").GetComponent<Text>().text = "";
            this.state = State.UI_PVE;
            this.SyncPanelVisible();
        });

        this.Hide();
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_BTN_COMPONENT_TEAM_CLICKED);//按钮被点击
        EventDispatcher.ins.AddEventListener(this, Events.ID_TOWN_TEAM_SYNC_INFO);

        this.SyncPanelVisible();
        return true;
    }
    bool show = false;
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_TOWN_BTN_COMPONENT_TEAM_CLICKED && show == false)
        {
            this.state = State.UI_PVE;
            this.Show();

        }
        else if (type == Events.ID_TOWN_TEAM_SYNC_INFO)
        {
            this.Sync(userData as TownTeaminfo);
        }
    }
    /// <summary>
    /// 显示完整的界面
    /// </summary>
    public override void Show()
    {
        show = true;
        this.PopIn();
        this.SyncPanelVisible();
        EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_BTN_SHOW, this);
    }
    public void SyncPanelVisible()
    {
        this.panel_info.SetActive(false);
        this.panel_pve.SetActive(false);
        this.panel_join.SetActive(false);
        this.panel_list.SetActive(false);
        if (this.state == State.UI_PVE)
        {
            this.panel_pve.SetActive(true);
        }
        else if (this.state == State.UI_INFO)
        {
            this.panel_info.SetActive(true);
        }
        else if (this.state == State.UI_JOIN)
        {
            this.panel_join.SetActive(true);
        }
        else if (this.state == State.UI_LIST)
        {
            this.panel_list.SetActive(true);
        }
        total_time = 0f;
    }
    public override void Hide()
    {
        show = false;
        this.PopOut();
        EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_BTN_ALL_CLOSE, this);
    }
    public void ShowPVEPanel()
    {
        this.state = State.UI_PVE;
        this.SyncPanelVisible();
    }
    public void ShowInfoPanel()
    {
        this.state = State.UI_INFO;
        this.SyncPanelVisible();
    }

    public void ShowSearchList(ArrayList list)
    {
        this.state = State.UI_LIST;
        this.SyncPanelVisible();
        this.SyncList(list);
    }

    private void SyncList(ArrayList list)
    {
        int i = 0;
        this.Clear();
        int count_join_able = 0;
        for (i = 0; i < list.Count; i++)
        {
            HashTable kv = list[i] as HashTable;

            string captain = kv["captain_name"];
            string no = kv["no"];
            string other = kv["other"];

            GameObject obj = GameObject.Instantiate(this.template_copy, this.list_content) as GameObject;
            obj.SetActive(true);
            ones.Add(obj);

            float max_height = list.Count * 100;

            this.list_content.sizeDelta = new Vector2(0, max_height);

            obj.transform.FindChild("info").transform.gameObject.GetComponent<Text>().text = "队长:" + captain + "   房间号:" + no;
            if ("0" == other)
            {
                count_join_able++;
                obj.transform.FindChild("state").transform.gameObject.GetComponent<Text>().text = "状态:可加入";
                obj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {//re bind function
                    //模拟手动输入房间号 加入
                    join_team_no = no;
                    EventDispatcher.ins.PostEvent(Events.ID_TOWN_TEAM_BTN_MAIN_JOIN, this);
                });
            }
            else
            {
                obj.transform.FindChild("state").transform.gameObject.GetComponent<Text>().text = "状态:不可加入";
                obj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {//re bind function
                    EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "该房间已满!");
                });
            }
             obj.transform.localScale = new Vector3(1f,1f, 1.0f);
            this.ReSize();
        }
        this.txt_content.text = "房间数量:" + count_join_able.ToString() + "/" + (list.Count.ToString());
    }

    private void ReSize()
    {
        //re position
        int i = this.ones.Count;
        foreach (GameObject one in this.ones)
        {
            one.transform.localPosition = new Vector3(one.transform.localPosition.x, -150 + i * 100, one.transform.localPosition.z);
            i--;
        }
    }
    private void Clear()
    {
        foreach (GameObject obj in ones)
        {
            GameObject.Destroy(obj);
        }
        ones.Clear();
    }

    public void Sync(TownTeaminfo info)
    {
        TownTeamPlayer captain = info.GetCaptain();
        this.SyncOnePlayer(captain, 0);
        if (PublicData.ins.self_user.no == captain.no)
        {
            btn_info_ok.transform.GetComponentInChildren<Text>().text = "开始游戏";
        }
        else
        {
            btn_info_ok.transform.GetComponentInChildren<Text>().text = "准备";
        }
        ArrayList ps = info.GetPlayerExceptCaptain();
        int i = 1;
        for (i = 1; i <= ps.Count; i++)
        {
            this.SyncOnePlayer(ps[i - 1] as TownTeamPlayer, i);
        }
        for (; i < players.Count; i++)
        {
            GameObject view = players[i] as GameObject;
            view.SetActive(false);
        }
        this.panel_info.transform.FindChild("info/txt_no").GetComponent<Text>().text = "房间号:" + info.no;

        this.info = info;
        this.state = State.UI_INFO;
        this.SyncPanelVisible();
    }
    private void SyncOnePlayer(TownTeamPlayer p, int index)
    {
        //    if (index > players.Count) return;
        GameObject view = players[index] as GameObject;
        //  if (view == null) return;
        view.SetActive(true);
        if (p.IsCaptain)
        {
            view.transform.FindChild("txt_name").GetComponent<Text>().text = "队长:" + p.name;
            view.transform.FindChild("txt_state").GetComponent<Text>().text = "";// "状态:" + p.state_string;
        }
        else
        {
            view.transform.FindChild("txt_name").GetComponent<Text>().text = "队员:" + p.name;
            view.transform.FindChild("txt_state").GetComponent<Text>().text = "状态:" + p.state_string;
        }

        view.transform.FindChild("txt_skill").GetComponent<Text>().text = "技能组:" + p.SkillGroup;
    }
    ArrayList players = new ArrayList();
    GameObject panel_pve;//主界面
    GameObject panel_info;//队伍信息
    GameObject panel_join;//加入队伍 输入房间号界面
    GameObject panel_list;
    Button btn_close;
    Button btn_close1;
    Button btn_create;
    Button btn_random;
    Button btn_single;
    Button btn_join;
    Button btn_search;

    Button btn_ok;


    Button btn_info_ok;//队伍信息 开始游戏 准备
    Button btn_info_close; // 队伍信息 退出队伍
    Button btn_info_skill; // 队伍信息 切换技能


    Button btn_join_ok;
    Button btn_join_close;

    public string join_team_no;
    Text txt_info_time;



    private ArrayList ones = new ArrayList();
    private RectTransform list_content = null;
    private GameObject template_copy = null;
    Text txt_content;

    Button btn_list_close;
    Button btn_list_sync;
}

