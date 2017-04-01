/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public sealed class ViewMgr : SingletonGAObject<ViewMgr>
{
    public void Add(View view)
    {
        this._views.Add(view);
        view.OnEnter();
    }

    public void Remove(View view)
    {
        this._views.Remove(view);
        view.OnExit();
        view.LazyDispose();
    }

    public override void UpdateMS()
    {
        EventDispatcher.ins.PostEvent(Events.ID_BEFORE_ALLVIEW_UPDATEMS);
        for (int i = 0; i < _views.Count; i++)
        {
            //   if ((_views[i] as View).IsInValid()) continue;
            (_views[i] as View).UpdateMS();
        }

        for (int i = 0; i < _views.Count; )
        {
            View b = _views[i] as View;
            if (b.IsInValid())
            {
                this.Remove(b);
            }
            else
            {
                ++i;
            }
        }
        EventDispatcher.ins.PostEvent(Events.ID_AFTER_ALLVIEW_UPDATEMS);
    }
    public override void Update()
    {
        EventDispatcher.ins.PostEvent(Events.ID_BEFORE_ALLVIEW_UPDATE);
        for (int i = 0; i < _views.Count; i++)
        {
            (_views[i] as View).Update();
        }

        for (int i = 0; i < _views.Count; )
        {
            View b = _views[i] as View;
            if (b.IsInValid())
            {
                this.Remove(b);
            }
            else
            {
                ++i;
            }
        }
        EventDispatcher.ins.PostEvent(Events.ID_AFTER_ALLVIEW_UPDATE);
    }

    public override void OnDispose()
    {
        for (int i = 0; i < _views.Count; i++)
        {
            (_views[i] as View).Dispose();
        }
        _views.Clear();
        base.OnDispose();
    }

    public static View Create<T>(Model m) where T : new()
    {
        View v = new T() as View;
        v.BindModel(m);
        v.Init();
        return v;
    }
    private ArrayList _views = new ArrayList();
}
