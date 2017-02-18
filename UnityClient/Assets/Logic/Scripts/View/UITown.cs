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
            this._ui_child.Add(ViewUI.Create<UI_friendsapp>(this));

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

        return true;
    }


    private ArrayList texts = new ArrayList();
    GameObject _panel;
    GameObject txt_template;
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
            this.panel.SetActive(false);

        };
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



    private Button btn_close = null;
    private Button btn_show = null;
    private Button btn_friends = null;

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
                    RpcClient.ins.SendRequest("services.friends", "add_by_no", "no:1,name: " + input_name.text + ",", (string msg) =>
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

                    RpcClient.ins.SendRequest("services.friends", "add_by_name", "no:1,name:" + input_name.text + ",", (string msg) =>
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
        EventDispatcher.ins.AddEventListener(this, Events.ID_VIEW_SYNC_FRIENDS_LIST);



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
        else if (type == Events.ID_VIEW_SYNC_FRIENDS_LIST)
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
        this.current_detail_user = user ;

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

    public DAO.User current_detail_user= null;//当前显示详细信息的no


}

