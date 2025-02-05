﻿/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

/// <summary>
///  manage and call all sub mgr(such as HeroMgr EnemyMgr and BulletMgr)  and model
/// </summary>
public sealed class ModelMgr : SingletonGAObject<ModelMgr>
{
    public static T Create<T>() where T : new()
    {
        T t = new T();
        Model m = t as Model;
        m.Init();
        ins.Add(m);
        return t;
    }
    public override void OnDispose()
    {

        this.lists.Clear();
        BulletMgr.DestroyInstance();
        HeroMgr.DestroyInstance();
        EnemyMgr.DestroyInstance();
        BuildingMgr.DestroyInstance();

        base.OnDispose();
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
    int tick_server_rpc = 0;
    public override void UpdateMS()
    {
        ++tick_server_rpc;
        if (tick_server_rpc > 400)
        {
            tick_server_rpc = 0;
            RpcClient.ins.SendRequest("services.example", "heart_beat", "no:" + PublicData.ins.self_user.no.ToString() + ",", (string ree) => { });
        }
        EventDispatcher.ins.PostEvent(Events.ID_BEFORE_ALLMODEL_UPDATEMS);

        BulletMgr.ins.UpdateMS();
        HeroMgr.ins.UpdateMS();
        EnemyMgr.ins.UpdateMS();
        BuildingMgr.ins.UpdateMS();

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
        EventDispatcher.ins.PostEvent(Events.ID_AFTER_ALLMODEL_UPDATEMS);
    }
    public override void Update()
    {
        ///  EventDispatcher.ins.PostEvent(Events.ID_BEFORE_ALLMODEL_UPDATEMS);

        BulletMgr.ins.Update();
        HeroMgr.ins.Update();
        EnemyMgr.ins.Update();
        BuildingMgr.ins.Update();

        foreach (Model b in lists)
        {
            // if (b.IsValid()) 

            { b.Update(); }
        }

        /*    for (int i = 0; i < lists.Count; )
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
            }*/
        ////  EventDispatcher.ins.PostEvent(Events.ID_AFTER_ALLMODEL_UPDATEMS);
    }

    public ArrayList GetModels<T>() where T : Model
    {
        System.Type t = typeof(T);
        ArrayList ret = new ArrayList();
        foreach (Model b in lists)
        {
            if (b.GetType() == t)
            {
                ret.Add(b);
            }
        }
        return ret;
    }
    public T GetModel<T>() where T : Model
    {
        System.Type t = typeof(T);
        foreach (Model b in lists)
        {
            if (b.GetType() == t)
            {
                return b as T;
            }
        }
        return null;// default(T);            
    }
    ArrayList lists = new ArrayList();

}