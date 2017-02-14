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
            this._ui_child.Add(ViewUI.Create<UI_worldchat>(this));

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






public sealed class UI_worldchat : ViewUI
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
            EventDispatcher.ins.PostEvent(Events.ID_WORLDCHAT_CLOSE_BTN_CLICKED);

            this.HideWhole();
        });



        this.cell_btn.onClick.AddListener(() =>
            {
                ShowWhole();
                EventDispatcher.ins.PostEvent(Events.ID_WORLDCHAT_CELL_BTN_CLICKED);
            });


        this.hideSmall();

        this.HideWhole();

        EventDispatcher.ins.AddEventListener(this, Events.ID_RPC_WORLD_CHAT_NEW_MSG);
        return true;
    }


    private void funccc()
    {

        GameObject obj = GameObject.Instantiate(this.template_copy, this.list_content) as GameObject;
        obj.SetActive(true);
        lists.Add(obj);
        current_size += 1;
        float max_height = current_size * 110;

        this.list_content.sizeDelta = new Vector2(0, max_height);



        Text name = obj.transform.FindChild("worldchat_cell_txt_head").transform.gameObject.GetComponent<Text>();
        name.text = "<color=green>[世界]</color><color=#9420D2FF>测试玩家:</color>" + current_size.ToString();


        //re position
        int i = this.lists.Count;
        foreach (GameObject one in this.lists)
        {


            one.transform.localPosition = new Vector3(one.transform.localPosition.x, 0 + i * 100, one.transform.localPosition.z);


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

            scale =( txt_msg.text.Length+1)/ 25.0f;

            obj.transform.FindChild("worldchat_cell_bg1").transform.localScale = new Vector3(1.0f, scale, 1.0f);


            //re position
            int i = this.lists.Count;
            foreach (GameObject one in this.lists)
            {


                one.transform.localPosition = new Vector3(one.transform.localPosition.x, 0 + i * 60, one.transform.localPosition.z);


                i--;
            }












        }
    }




    /// <summary>
    /// 显示完整的界面
    /// </summary>
    private void ShowWhole()
    {
        this.obj_whold.SetActive(true);


    }
    private void HideWhole()
    {
        this.obj_whold.SetActive(false);


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

    private RectTransform list_content = null;
    private Button list_btn_send = null;
    private Button list_btn_close = null;

    private GameObject template_copy = null;
    ArrayList lists = new ArrayList();

    private InputField input = null;
    private Counter cell_counter = Counter.Create(400);//显示  10 秒

    private int MAX_REMAIN_COUNTS = 10; //最大消息保留条数
}
