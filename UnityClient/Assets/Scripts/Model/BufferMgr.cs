/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


/// <summary>
/// each Entity hold a BufferMgr which can manage itself 's buffer life cycle
/// </summary>
public sealed class BufferMgr : GAObject
{
    public void Add(Buffer b)
    {
        //处理唯一性
        if (b.isOnlyOne)
        {
            Buffer has = GetBuffer(b.GetName());
            if ( has!= null)
            {
                has.OnMerge(b);
                return;
            }
        }
        // 处理冲突
        foreach (Buffer buffer in lists)
        {
            if (buffer.IsConflict(b))
            {
                //冲突 ， 不添加
                return;
            }
        }
        if (b.has_view && b.plist != "")
        {
            ViewMgr.Create<ViewBuffer>(b);
        }

        this.lists.Add(b);
        b.mgr = this;
        b.target = owner;
        b.OnEnter();
    }
    public void Remove(Buffer b)
    {
        this.lists.Remove(b);
        b.OnExit();
        b.LazyDispose();
    }

    public override void UpdateMS()
    {
        foreach (Buffer b in lists)
        {
            if (b.IsValid())
            {
                EventDispatcher.ins.PostEvent(Events.ID_BEFORE_ONEBUFFER_UPDATEMS, b);
                b.UpdateMS();
                EventDispatcher.ins.PostEvent(Events.ID_AFTER_ONEBUFFER_UPDATEMS, b);
            }
        }
        // clear all complete buffer
        for (int i = 0; i < lists.Count; )
        {
            Buffer b = lists[i] as Buffer;
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
    public ArrayList GetBuffers()
    {
        return lists;
    }
    public Buffer GetBuffer(string name)
    {
        foreach (Buffer buf in lists)
        {
            if (buf.GetName() == name) return buf;
        }
        return null;
    }
    public Buffer GetBuffer(int id)
    {
        foreach (Buffer buf in lists)
        {
            if (buf.GetId() == id) return buf;
        }
        return null;
    }
    public bool IsExist(Buffer b)
    {
        foreach (Buffer bb in lists)
        {
            if (bb.GetId() == b.GetId() && bb.target == b.target) return true;
        }
        return false;
    }

    ArrayList lists = new ArrayList();

    /*   public static BufferMgr ins
       {
           get
           {
               return BufferMgr.GetInstance();
           }
       }

       private static BufferMgr _ins = null;

       public static BufferMgr GetInstance()
       {
           if (_ins == null)
           {
               _ins = new BufferMgr();
           }
           return _ins;
       }*/


    public static BufferMgr Create(Entity owner)
    {
        BufferMgr ret = new BufferMgr();
        ret.owner = owner;
        return ret;
    }

    public Buffer Create<T>(Entity owner) where T : new()
    {
        return InitHelper(new T() as Buffer, owner);
    }

    //helper function for create Buffer
    public static T CreateHelper<T>(Entity owner) where T : Buffer, new()
    {
        T t = new T();
        t.owner = owner;
        t.Init();
        return t;
    }
    /// <summary>
    ///  for config able suport
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="_class"> the class name</param>
    /// <returns></returns>
    public Buffer Create(string _class, Entity owner)
    {
        System.Type t = System.Type.GetType(_class);
        if (t == null)
        {
            Debug.LogError("UnKnown type:" + _class);
            return null;
        }
        return InitHelper(System.Activator.CreateInstance(t) as Buffer, owner);
    }
    private Buffer InitHelper(Buffer ret, Entity owner)
    {
        if (ret == null) return null;
        ret.target = this.owner;
        ret.owner = owner;
        ret.mgr = this;
        ret.Init();
        this.Add(ret);

    
        return ret;
    }


    public Buffer Create<T>() where T : new()
    {
        return Create<T>(this.owner);
    }

    public Buffer Create(string _class)
    {
        return Create(_class, this.owner);
    }
    private Buffer InitHelper(Buffer ret)
    {
        return this.InitHelper(ret, this.owner);
    }

    private BufferMgr()
    {
    }

    Entity owner = null;
}
