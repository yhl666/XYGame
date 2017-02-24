using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public sealed class UI_trump_levelupapp : UICellApp
{
    public override bool Init()
    {
        base.Init();
        //this.panel = GameObject.Find("ui_panel_trump_levelup");

        this.panel = PrefabsMgr.Load("Prefabs/ui/ui_panel_trump_levelup");
        this.panel.transform.parent = this._root._ui_root.transform;
        this.panel.transform.localPosition = Vector3.zero;
        this.panel.SetActive(true);
        this.btn_close = this.panel.transform.FindChild("btn_trump_levelup_close").GetComponent<Button>();
        this.btn_levelup = this.panel.transform.FindChild("btn_trump_levelup_levelup").GetComponent<Button>();
        EventDispatcher.ins.AddEventListener(this, Events.ID_INFO_TRUMP_LEVELUP);
        this.btn_close.onClick.AddListener(() =>
        {

            Debug.Log("send trump_levelup closed");
            EventDispatcher.ins.PostEvent(Events.ID_TRUMP_LEVELUP_CLOSE_CLICKED, this);


        });
        this.btn_levelup.onClick.AddListener(() =>
        {
            //判断升级的条件  是否升级
        });

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
        if (type == Events.ID_INFO_TRUMP_LEVELUP)
        {
            Debug.Log("UI_TRUMPLEVELUP");
            EventDispatcher.ins.PostEvent(Events.ID_TRUMP_LEVELUP_CLICKED, this);
        }
    }

    private GameObject panel = null;
    private Button btn_levelup = null;
    private Button btn_close = null;
}

