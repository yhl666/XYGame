﻿using UnityEngine;
using System.Collections;

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

