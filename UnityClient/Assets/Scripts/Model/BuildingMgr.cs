/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

/// <summary>
///  manage and call all sub mgr(such as HeroMgr EnemyMgr and BulletMgr)  and model
/// </summary>
public sealed class BuildingMgr : SingletonGAObject<BuildingMgr>
{
    public static T Create<T>() where T : new()
    {
        T t = new T();
        Building m = t as Building;
        m.Init();
        ins.Add(m);

        return t;
    }
    public override void OnDispose()
    {
        this.lists.Clear();
    }
    public void Add(Building b)
    {
        this.lists.Add(b);
        b.OnEnter();
    }
    public ArrayList GetBuildings()
    {
        return lists;
    }
    public ArrayList GetBuildings<T>() where T : Building, new()
    {
        System.Type t = typeof(T);
        ArrayList ret = new ArrayList();
        foreach (Building b in lists)
        {
            if (b.GetType() == t)
            {
                ret.Add(b);
            }
        }
        return ret;
    }
    public T GetBuilding<T>() where T : Building, new()
    {
        System.Type t = typeof(T);
        foreach (Building b in lists)
        {
            if (b.GetType() == t)
            {
                return b as T;
            }
        }
        return null;// default(T);            
    }
    public bool HasBuilding()
    {
        return lists.Count > 0;
    }
    public int GetBuildingsCount()
    {
        return lists.Count;
    }

    public void Remove(Building b)
    {
        this.lists.Remove(b);
        b.OnExit();
        b.LazyDispose();
    }

    public override void UpdateMS()
    {
        foreach (Building b in lists)
        {
            b.AI_UpdateMSWithAI();
            EventDispatcher.ins.PostEvent(Events.ID_BEFORE_ONEENTITY_UPDATEMS, b);
            b.UpdateMS();
            EventDispatcher.ins.PostEvent(Events.ID_AFTER_ONEENTITY_UPDATEMS, b);
        }

        for (int i = 0; i < lists.Count; )
        {
            Building b = lists[i] as Building;
            if (b.IsInValid())
            {
                this.Remove(b);
            }
            else
            {
                ++i;
            }
        }
        ///    EventDispatcher.ins.PostEvent(Events.ID_AFTER_ALLMODEL_UPDATEMS);
    }
    public override void Update()
    {
        ///  EventDispatcher.ins.PostEvent(Events.ID_BEFORE_ALLMODEL_UPDATEMS);
        foreach (Building b in lists)
        {
            // if (b.IsValid()) 

            { b.Update(); }
        }
        ////  EventDispatcher.ins.PostEvent(Events.ID_AFTER_ALLMODEL_UPDATEMS);
    }

    ArrayList lists = new ArrayList();

}