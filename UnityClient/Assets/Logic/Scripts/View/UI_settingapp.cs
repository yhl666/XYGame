using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public sealed class UI_settingapp : UICellApp
{
    public override bool Init()
    {
        base.Init();
        //this.panel=GameObject.Find("ui_panel_setting");

        this.panel = PrefabsMgr.Load("Prefabs/ui/ui_panel_setting");
        this.panel.transform.parent = this._root._ui_root.transform;
        this.panel.transform.localPosition = Vector3.zero;
        this.panel.SetActive(true);
        //if (panel == null)
        //{
        //    Debug.LogError("setting panel is null");
        //}
        //this.btn_setting_open = GameObject.Find("btn_set").GetComponent<Button>();
        //this.btn_affiche_open = GameObject.Find("btn_setting_affiche").GetComponent<Button>();
        this.btn_affiche_open = this.panel.transform.FindChild("btn_setting_affiche").GetComponent<Button>();
        if (this.btn_affiche_open==null)
        {
            Debug.LogError("setting btn is null");
        }
        this.btn_close = this.panel.transform.FindChild("btn_setting_close").GetComponent<Button>();
        this.btn_close.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_SHOW_MENU);

            EventDispatcher.ins.PostEvent(Events.ID_SETTING_CLOSE_CLICKED,this);
 

        });



        this.btn_affiche_open.onClick.AddListener(() =>
        {

            EventDispatcher.ins.PostEvent(Events.ID_SETTING_CLOSE_CLICKED, this);
            EventDispatcher.ins.PostEvent(Events.ID_INFO_AFFICHE);
        });

        EventDispatcher.ins.AddEventListener(this, Events.ID_INFO_SETTING);

        this.Hide();

        return true;
    }

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

    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_INFO_SETTING)
        {
            Debug.Log("收到setting开启信息");
            EventDispatcher.ins.PostEvent(Events.ID_SETTING_CLICKED, this);
        }
    }

    private GameObject panel = null;
    private Button btnAudioOpen = null;
    private Button btnAudioClose = null;
    private Button btn_close = null;
    //private Button btn_setting_open = null;
    private Button btn_affiche_open = null;
}

