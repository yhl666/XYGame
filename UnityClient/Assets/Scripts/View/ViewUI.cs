using UnityEngine;
using System.Collections;


public class ViewUI : View
{
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

    public static ViewUI Create<T>(ViewUI _ui_root=null) where T : new()
    {
        ViewUI ret = new T() as ViewUI;
        ret._root = _ui_root;
        ret.Init();
        ViewMgr.ins.Add(ret);
        return ret;
    }
    protected ViewUI _root = null;
    protected GameObject _ui_root = null;
    protected GameObject _ui = null;
    protected bool _enable = false;
}


