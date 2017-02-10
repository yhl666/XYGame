using UnityEngine;
using System.Collections;


/// <summary>
///  sub mgr of ModelMgr
/// </summary>
public sealed class HeroMgr : GAObject
{
    public Hero self = null;
    public int me_no = 0;
    public static Hero Create<T>() where T : new()
    {
        Hero ret = new T() as Hero;
        ret.Init();
        ins.Add(ret);
        return ret;
    }

    public void Add(Hero b)
    {
        this.lists.Add(b);
        b.OnEnter();
    }
    public void Remove(Hero b)
    {
        this.lists.Remove(b);
        b.OnExit();
        b.LazyDispose();
    }
    public Hero GetSelfHero()
    {
        return self;
    }

    public override void UpdateMS()
    {
        foreach (Hero b in lists)
        {
            EventDispatcher.ins.PostEvent(Events.ID_BEFORE_ONEENTITY_UPDATEMS, b);
            b.UpdateMS();
            EventDispatcher.ins.PostEvent(Events.ID_AFTER_ONEENTITY_UPDATEMS, b);

        }

        //clear
        for (int i = 0; i < lists.Count; )
        {
            Hero b = lists[i] as Hero;
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


    public ArrayList GetHeros()
    {
        return lists;
    }

    public Hero GetHero(int no)
    {
        foreach (Hero hero in lists)
        {
            if (hero.no == no) return hero;
        }
        return null;
    }


    ArrayList lists = new ArrayList();

    public static HeroMgr ins
    {
        get
        {
            return HeroMgr.GetInstance();
        }
    }

    private static HeroMgr _ins = null;

    public static HeroMgr GetInstance()
    {
        if (_ins == null)
        {
            _ins = new HeroMgr();
        }
        return _ins;
    }


}



