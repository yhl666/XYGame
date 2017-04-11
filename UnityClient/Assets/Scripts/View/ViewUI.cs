/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


public class ViewUI : View
{
    public void Show()
    {
        this.OnShow();
    }
    public void Hide()
    {
        this.OnHide();
    }
    public virtual void OnShow()
    {

    }
    public virtual void OnHide()
    {

    }
    protected void PopIn(GameObject host)
    {
        host.SetActive(true);
        ScaleTo.Create(host, 0.05f, 0.7f, 0.7f).OnComptele = () =>
        {
            ScaleTo.Create(host, 0.01f, 0.9f, 0.9f).OnComptele = () =>
            {
                ScaleTo.Create(host, 0.01f, 1.2f, 1.2f).OnComptele = () =>
                {
                    ScaleTo.Create(host, 0.03f, 1f, 1f).OnComptele = () =>
                    {

                    };
                };
            };
        };
    }
    protected void PopOut(GameObject host)
    {
        ScaleTo.Create(host, 0.1f, 0.0f, 0.0f).OnComptele = () =>
        {

            if (host.GetComponent<Actions>() == null)
            {
                host.SetActive(false);
            }
        };
    }


    public override void Update()
    {
        if (!_enable) return;
        base.Update();
    }

    public override bool Init()
    {
        base.Init();
        return true;
    }

    public override void OnEvent(string type, object userData)
    {

    }

    public void SetEnable(bool e)
    {
        this._enable = e;
    }

    public static ViewUI Create<T>(ViewUI _ui_root = null) where T : new()
    {
        ViewUI ret = new T() as ViewUI;
        ret._root = _ui_root;
        ret.Init();
        return ret;
    }
    protected ViewUI _root = null;
    public GameObject _ui_root = null;
    protected GameObject _ui = null;
    protected bool _enable = false;
}
