/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


// 支持2重消息方式，持久化 和监听一次。
//支持2种消息数据结构，string 和 Event.XXXXXX  (int)

public sealed class EventDispatcher : GAObject
{
    private EventDispatcher()
    {
        ///  EventSystem.ins.AddEvent_Update(this);
        for (int i = 0; i < Events.MAX_EVENT_LENGTH; i++)
        {
            objs_event[i] = null;
            objs_for_once_event[i] = null;
        }
    }
    Hashtable objs = new Hashtable();
    Hashtable objs_for_once = new Hashtable();
    ArrayList[] objs_event = new ArrayList[Events.MAX_EVENT_LENGTH];
    ArrayList[] objs_for_once_event = new ArrayList[Events.MAX_EVENT_LENGTH];

    public override void OnDispose()
    {
        base.OnDispose();
        ////   EventSystem.ins.RemoveEvent_Update(this);
        TimerQueue.DestroyInstance();
    }


    public static EventDispatcher ins
    {
        get
        {
            return EventDispatcher.GetInstance();
        }
    }

    private static EventDispatcher _ins = null;

    /// <summary>
    ///  return global event dispatcher
    /// </summary>
    /// <returns></returns>
    public static EventDispatcher GetInstance()
    {
        if (_ins == null)
        {
            _ins = new EventDispatcher();
        }
        return _ins;
    }

    /// <summary>
    ///  release global event dispatcher
    /// </summary>
    public static void DestroyInstance()
    {
        _ins.Dispose();
        _ins.Clear();
        _ins = null;
    }

    /// <summary>
    ///  create a local event dispatcher
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static EventDispatcher Create(string name = "UnName")
    {
        EventDispatcher ret = new EventDispatcher();
        ret._name = name;

        return ret;
    }


    public void PostEvent(string type, object userdata = null)
    {
        if (objs.ContainsKey(type))
        {
            ArrayList list = objs[type] as ArrayList;

            for (int i = 0; i < list.Count; i++)
            {
                GAObject obj = list[i] as GAObject;

                obj.OnEvent(type, userdata);
            }
        }
    }






    public void PostEventOnce(string type, object userdata = null)
    {
        //call for once
        if (objs_for_once.ContainsKey(type))
        {
            ArrayList list = objs_for_once[type] as ArrayList;

            foreach (GAObject obj in list)
            {
                obj.OnEvent(type, userdata);
            }
            objs_for_once.Remove(type);
        }

    }



    public void PostEvent(int type, object userdata = null)
    {

        if (objs_event[type] == null)
        {
            return;
        }
        ArrayList list = objs_event[type] as ArrayList;

        foreach (GAObject obj in list)
        {
            obj.OnEvent(type, userdata);
        }

    }

    public void PostEventOnce(int type, object userdata = null)
    {
        //call for once
        if (objs_for_once_event[type] != null)
        {
            ArrayList list = objs_for_once_event[type] as ArrayList;

            foreach (GAObject obj in list)
            {
                obj.OnEvent(type, userdata);
            }
            objs_for_once_event[type] = null;
        }

    }


    public void AddEventListener(GAObject target, string type)
    {
        if (objs.ContainsKey(type) == false)
        {
            objs.Add(type, new ArrayList());
        }
        ArrayList list = objs[type] as ArrayList;

        list.Add(target);
    }


    public void AddEventListener(GAObject target, int type)
    {
        if (objs_event[type] == null)
        {
            objs_event[type] = new ArrayList();
        }
        ArrayList list = objs_event[type] as ArrayList;

        list.Add(target);
    }

    public void AddEventListenerOnce(GAObject target, int type)
    {
        if (objs_for_once_event[type] == null)
        {
            objs_for_once_event[type] = new ArrayList();
        }
        ArrayList list = objs_for_once_event[type] as ArrayList;

        list.Add(target);
    }
    public void AddEventListenerOnce(GAObject target, string type)
    {
        if (objs_for_once.ContainsKey(type) == false)
        {
            objs_for_once.Add(type, new ArrayList());
        }
        ArrayList list = objs_for_once[type] as ArrayList;

        list.Add(target);
    }

    public void RemoveEventListener(GAObject target, string type)
    {
        if (objs.ContainsKey(type) == false)
        {
            return;
        }
        ArrayList list = objs[type] as ArrayList;

        list.Remove(target);
    }

    public void RemoveEventListener(GAObject target, int type)
    {
        if (objs_event[type] == null)
        {
            return;
        }
        ArrayList list = objs_event[type] as ArrayList;

        list.Remove(target);
    }





    public void Clear()
    {
        objs.Clear();
        objs_event = null;
        objs_for_once_event = null;
        objs_for_once.Clear();

    }

    public void AddFuncToMainThread(VoidFuncVoid func)
    {
        this._queue_funcs.Enqueue(func);
    }

    private void ProcessOtherThreadFunc()
    {
        _queue_funcs.Lock();

        while (_queue_funcs.UnSafeEmpty() == false)
        {
            VoidFuncVoid func = _queue_funcs.UnSafeDequeue() as VoidFuncVoid;
            func();
        }

        _queue_funcs.UnLock();
    }

    public override void Update()
    {
        this.ProcessOtherThreadFunc();
        TimerQueue.ins.Tick();
    }
    private ThreadSafeQueue _queue_funcs = new ThreadSafeQueue();

    private string _name = "Global";
    public string name
    {
        get
        {
            return _name;
        }
    }
};
