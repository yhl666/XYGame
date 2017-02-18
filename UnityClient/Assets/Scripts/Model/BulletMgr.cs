using UnityEngine;
using System.Collections;
using System;

public sealed class BulletMgr : GAObject
{
    public static Bullet Create<T>(Entity owner, BulletConfigInfo info = null) where T : new()
    {
        return InitHelper(new T() as Bullet, owner, info);
    }

    /// <summary>
    ///  for config able suport
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="_class"> the class name</param>
    /// <returns></returns>
    public static Bullet Create(Entity owner, string _class, BulletConfigInfo info = null)
    {
        Type t = Type.GetType(_class);
        if (t == null)
        {
            Debug.LogError("UnKnown type:" + _class);
            return null;
        }
        return InitHelper(Activator.CreateInstance(t) as Bullet, owner, info);
    }
    private static Bullet InitHelper(Bullet ret, Entity owner, BulletConfigInfo info)
    {
        if (ret == null) return null;

        ret.owner = owner;
        if (info != null)
        {
            (ret as BulletConfig).LoadConfig(info);
        }
        ret.Init();
        BulletMgr.ins.Add(ret);
        ViewMgr.Create<ViewBullet>(ret);
        return ret;
    }

    public void Add(Bullet b)
    {
        this.lists.Add(b);
        b.OnEnter();

    }
    public void Remove(Bullet b)
    {
        this.lists.Remove(b);
        b.OnExit();
        b.LazyDispose();
    }

    public override void OnDispose()
    {

        this.lists.Clear();
        _ins = null;
    }
    public override void UpdateMS()
    {
        EventDispatcher.ins.PostEvent(Events.ID_BEFORE_ALLBULLET_UPDATEMS);
        foreach (Bullet b in lists)
        {
            if (b.IsValid())
            {
                EventDispatcher.ins.PostEvent(Events.ID_BEFORE_ONEBULLET_UPDATEMS, b);
                b.UpdateMS();
                EventDispatcher.ins.PostEvent(Events.ID_AFTER_ONEBULLET_UPDATEMS, b);
            }
        }
        // clear all complete buffer
        for (int i = 0; i < lists.Count; )
        {
            Bullet b = lists[i] as Bullet;
            if (b.IsInValid())
            {
                this.Remove(b);
            }
            else
            {
                ++i;
            }
        }
        EventDispatcher.ins.PostEvent(Events.ID_AFTER_ALLBULLET_UPDATEMS);
    }



    ArrayList lists = new ArrayList();

    public static BulletMgr ins
    {
        get
        {
            return BulletMgr.GetInstance();
        }
    }

    private static BulletMgr _ins = null;

    public static BulletMgr GetInstance()
    {
        if (_ins == null)
        {
            _ins = new BulletMgr();
        }
        return _ins;
    }

}
