/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


/// <summary>
///  sub mgr of ModelMgr
/// </summary>
public sealed class HeroMgr : GAObject
{
    public Hero self = null;
    public int me_no = 0;
    public static T Create<T>() where T : new()
    {
        T ret = new T();
        (ret as Hero).Init();
        ins.Add(ret as Hero);
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


    public override void OnDispose()
    {

        this.lists.Clear();
        self = null;
        me_no = 0;
        _ins = null;
    }
    public void Remove(int b)
    {
        Hero hero = this.GetHero(b);
        if (hero == null) return;
        this.Remove(hero);
    }


    public Hero GetSelfHero()
    {
        return self;
    }

    public override void UpdateMS()
    {
        foreach (Hero b in lists)
        {
            b.AI_UpdateMSWithAI();
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="includeDie"> 是否包含死亡的Hero</param>
    /// <returns></returns>
    public ArrayList GetHeros(bool includeDie=false)
    {
        if (includeDie)
        {
            return lists;
        }
        ArrayList ret = new ArrayList();
        foreach(Hero h in lists)
        {
            if(false == h.isDie)
            {
                ret.Add(h);
            }
        }
        return ret;
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



