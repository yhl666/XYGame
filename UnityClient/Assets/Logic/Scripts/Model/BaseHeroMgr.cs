using UnityEngine;
using System.Collections;

 
/// <summary>
///  sub mgr of ModelMgr
/// </summary>
public sealed class BaseHeroMgr : GAObject
{
    public BaseHero self = null;
    public int me_no = 0;
    public static BaseHero Create<T>() where T : new()
    {
        BaseHero ret = new T() as BaseHero;
        ret.Init();
        ins.Add(ret);
        return ret;
    }

    public void Add(BaseHero b)
    {
        this.lists.Add(b);
        b.OnEnter();
    }
    public void Remove(BaseHero b)
    {
        this.lists.Remove(b);
        b.OnExit();
        b.LazyDispose();
    }
    public BaseHero GetSelfBaseHero()
    {
        return self;
    }

    public override void UpdateMS()
    {
        foreach (BaseHero b in lists)
        {
            //EventDispatcher.ins.PostEvent(Events.ID_BEFORE_ONEENTITY_UPDATEMS, b);
            b.UpdateMS();
            //EventDispatcher.ins.PostEvent(Events.ID_AFTER_ONEENTITY_UPDATEMS, b);

        }

        //clear
        for (int i = 0; i < lists.Count; )
        {
            BaseHero b = lists[i] as BaseHero;
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


    public ArrayList GetBaseHeros()
    {
        return lists;
    }

    public BaseHero GetBaseHero(int no)
    {
        foreach (BaseHero hero in lists)
        {
            if (hero.no == no) return hero;
        }
        return null;
    }


    ArrayList lists = new ArrayList();

    public static BaseHeroMgr ins
    {
        get
        {
            return BaseHeroMgr.GetInstance();
        }
    }

    private static BaseHeroMgr _ins = null;

    public static BaseHeroMgr GetInstance()
    {
        if (_ins == null)
        {
            _ins = new BaseHeroMgr();
        }
        return _ins;
    }

 



}
