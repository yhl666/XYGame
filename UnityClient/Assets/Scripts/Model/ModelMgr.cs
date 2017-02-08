using UnityEngine;
using System.Collections;

/// <summary>
///  manage and call all sub mgr(such as HeroMgr EnemyMgr and BulletMgr)  and model
/// </summary>
public sealed class ModelMgr : GAObject
{
    public static T Create<T>() where T : new()
    {
        T t = new T();
        Model m =t as Model;
        m.Init();
        ins.Add(m);
        return t;
    }

    public void Add(Model b)
    {
        this.lists.Add(b);
        b.OnEnter();
    }
    public void Remove(Model b)
    {
        this.lists.Remove(b);
        b.OnExit();
        b.LazyDispose();
    }

    public override void UpdateMS()
    {

        BulletMgr.ins.UpdateMS();
        HeroMgr.ins.UpdateMS();
        EnemyMgr.ins.UpdateMS();

        foreach (Model b in lists)
        {
            // if (b.IsValid()) 
            { b.UpdateMS(); }
        }

        for (int i = 0; i < lists.Count; )
        {
            Model b = lists[i] as Model;
            if (b.IsInValid())
            {
                this.Remove(b);
            }
            else
            {
                ++i;
            }
        }
    }
  

    ArrayList lists = new ArrayList();

    public static ModelMgr ins
    {
        get
        {
            return ModelMgr.GetInstance();
        }
    }

    private static ModelMgr _ins = null;

    public static ModelMgr GetInstance()
    {
        if (_ins == null)
        {
            _ins = new ModelMgr();
        }
        return _ins;
    }
}


