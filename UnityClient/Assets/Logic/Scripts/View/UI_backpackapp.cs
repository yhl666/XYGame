using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public sealed class UI_backpackapp : UICellApp
{
    public override bool Init()
    {
        base.Init();
        //this.panel = GameObject.Find("ui_panel_backpack");
        this.panel = PrefabsMgr.Load("Prefabs/ui/ui_panel_backpack");
        this.panel.transform.parent = this._root._ui_root.transform;
        this.panel.transform.localPosition =Vector3.zero;
        this.panel.SetActive(true);
        this.btn_close = this.panel.transform.FindChild("btn_backpack_close").GetComponent<Button>();
        showdetailmaterial = GameObject.Find("backpack_panel_detail_material");
        showDetailEquip=GameObject.Find("backpack_panel_detail_equipment");
        EventDispatcher.ins.AddEventListener(this, Events.ID_INFO_BACKPACK);
        this.btn_close.onClick.AddListener(() =>
        {
            EventDispatcher.ins.PostEvent(Events.ID_BACKPACK_CLOSE_CLICKED, this);
        });
        for (int i = 1; i < 25; i++)
        {
            Button btn_temp = GameObject.Find("backpack_panel_button" + i.ToString()).GetComponentInChildren<Button>();
            //Debug.Log("backpack_panel_button" + i.ToString());
            if (btn_temp!=null)
            {
                btndetails.Add(btn_temp);
            }
        }
        //Debug.Log("count==="+btndetails.Count.ToString());
        //Button hidematerial = GameObject.Find("btn_backpack_panel_detail_hide_material").GetComponent<Button>();
        for (int i = 0; i < btndetails.Count; i++)
        {

            //Debug.Log("init"+i.ToString()+"button");
            (btndetails[i] as Button).onClick.AddListener(()=>
            {
                //EventDispatcher.ins.PostEvent(Events.ID_INFO_BACKPACK_SHOW);

                showdetailmaterial.SetActive(true);
                //showdetailmaterial.transform.position = ;

                //hidematerial.onClick.AddListener(() =>
                //{
                //    showdetailmaterial.SetActive(true);
                //    showdetailmaterial.transform.position = (btndetails[i] as Button).transform.position;
                //});
                //showdetailmaterial.GetComponentInChildren<Button>().onClick.AddListener(() =>
                //{
                //    Debug.Log("hide detail");
                //    showdetailmaterial.SetActive(false);
                //});
                Button hidedetail = GameObject.Find("btn_backpack_panel_detail_hide_material").GetComponent<Button>();
                hidedetail.onClick.AddListener(()=>
                {
                    showdetailmaterial.SetActive(false);
                }
            );
            }
        );
        }
        showDetailEquip.SetActive(false);
        showdetailmaterial.SetActive(false);
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
        if (type == Events.ID_INFO_BACKPACK)
        {
            Debug.Log("UI_backpack");
            EventDispatcher.ins.PostEvent(Events.ID_BACKPACK_CLICKED, this);
            //this.Show();
        }
    }
    private GameObject panel = null;
    GameObject showdetailmaterial = null;
    private GameObject showDetailEquip = null;
    private Button btn_close = null;
    
    private ArrayList btndetails =new ArrayList();
}

