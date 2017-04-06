/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;


//顶级父类，用于消息 和 内存管理 
/*
 * 
 *
 */
/// <summary>
/// -------------------------------------------------------------------------------------------------
/// 3种内存管理方式，对外接口是
/// Dispose()  触发OnDispose事件， 立刻删除该对象
/// SetInValid()  标记为脏数据 由各自 Mgr处理完成后清理
/// LazyDispose()  触发 OnDisposoe事件， 和SetInValid 一样，不过是吧清理任务交给了 AutoReleasePool 统一清理
/// 
/// 3个接口任一一个被调用，那么就会被标记为 脏数据 非法数据，接口是IsInValid() IsValid() 外部通过这2个评定当前对象是否 可操作
/// 
/// 
/// 
/// 目前在Mgr中的实现方式是 各自的对象只用SetInValid 接口来标记 各自Mgr UpdateMS完成后开始清理出Mgr 
/// 然后调用 LazyDispose 来释放，具体可看源码
/// --------------------------------------------------------------------------------------------------------
/// 2种消息方式。OnEvent 的 string 和int 重载
/// 详见  EventDispatcher.cs
/// </summary>
public class GAObject : IDisposable
{
    // every frame will be call
    public virtual void Update()
    {

    }

    //every logic frame will be call(Frame Sync to All clients)
    public virtual void UpdateMS()
    {

    }

    //use this to init you class
    /// <summary>
    /// 
    /// 对象构造初始化
    /// </summary>
    /// <returns></returns>
    public virtual bool Init()
    {
        return true;
    }
    public GAObject()
    {
        track_list.PushBack(this);
    }
    public virtual void OnEvent(string type, object userData)
    {

    }
    public virtual void OnEvent(int type, object userData)
    {

    }

    // you can override  this interface to release your Res ,Rem. must call base.OnExit when you override
    /// <summary>
    /// 逻辑上退出 事件
    /// </summary>
    public virtual void OnExit()
    {

    }
    /// <summary>
    ///   释放后的事件
    /// </summary>
    public virtual void OnDispose()
    {

    }
    /// <summary>
    /// 逻辑上enter 事件
    /// </summary>
    public virtual void OnEnter()
    {

    }


    // you can use this interface to  notify class to release//OnExit will be call
    // use this or Release()
    /// <summary>
    /// 触发OnDispose事件， 立刻删除该对象
    /// </summary>
    public void Dispose()
    {
        track_list.Remove(this);
        this.OnDispose();
    }

    /// <summary>
    /// 触发 OnDisposoe事件， 和SetInValid 一样，不过是吧清理任务交给了 AutoReleasePool 统一清理
    /// </summary>
    public void LazyDispose()
    {
        track_list.Remove(this);
        AutoReleasePool.ins.AddObject(this);
    }

    /// <summary>
    ///   触发OnExit事件，  标记为脏数据 由各自 Mgr处理完成后清理
    /// </summary>
    public void SetInValid()
    {
        this.isValid = false;
    }

    /// <summary>
    /// 判断是否有效对象接口，
    /// </summary>
    /// <returns></returns>
    public bool IsValid()
    {
        if (isInAutoReleasePool) return false;
        return isValid;
    }
    public bool IsInValid() { return !IsValid(); }

    private bool isValid = true;
    public bool isInAutoReleasePool = false;
    /// <summary>
    /// 深拷贝 接口
    /// </summary>
    /// <returns></returns>
    public virtual GAObject Clone()
    {
        return null;
    }

    ///--------------------------------------------------------- some helper function--------------

    public void log(string what)
    {
        Debug.Log(this.GetType().ToString() + ":" + what);
    }

    public static void PrintMemoryTrack()
    {
        Debug.LogWarning("[GAObject MemoryTrack]:------------Start------------");

        int count = track_list.Count();

        for (int i = 0; i < count; i++)
        {
            GAObject obj = track_list[i] as GAObject;
            Debug.LogWarning(string.Format("[GAObject MemoryTrack]: class {0} alive in HashCode:{1}", obj.GetType().ToString(), obj.GetHashCode()));
        }

        Debug.LogWarning(string.Format("[GAObject MemoryTrack]: {0} GAObject alive", count));
        Debug.LogWarning("[GAObject MemoryTrack]:------------End------------");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="subClass">是否输出继承关系的类</param>
    public static void PrintMemoryTrack<T>(bool subClass = true)
    {
        int count = track_list.Count();
        int c = 0;
        Type typeT = typeof(T);
        Debug.LogWarning("[GAObject MemoryTrack Of Type=" + typeT.ToString() + "]:------------Start------------");
        for (int i = 0; i < count; i++)
        {
            GAObject obj = track_list[i] as GAObject;
            Type t = obj.GetType();
            if ((t != typeT) && (subClass && t != typeT))
            {
                if (t.IsSubclassOf(typeT) == false && typeT.IsSubclassOf(t) == false)
                {
                    continue;
                }

            }
            ++c;
            Debug.LogWarning(string.Format("[GAObject MemoryTrack]: class {0} alive in HashCode:{1}", obj.GetType().ToString(), obj.GetHashCode()));
        }

        Debug.LogWarning(string.Format("[GAObject MemoryTrack Of Type]: {0} GAObject  Of Type(" + typeT.ToString() + ") alive", c));
        Debug.LogWarning("[GAObject MemoryTrack Of Type=" + typeT.ToString() + "]:------------End------------");
    }

    static Vector track_list = new Vector();

    public static GAObject Create(string _class_name)
    {
        System.Type t = System.Type.GetType(_class_name);
        if (t == null)
        {
            Debug.LogError("UnKnown type:" + _class_name);
            return null;
        }
        if (t.IsSubclassOf(t_static) == false)
        {
            Debug.LogError("class " + _class_name + " not the subclass of GAObject");
            return null;
        }
        return (System.Activator.CreateInstance(t) as GAObject);
    }
    public static T Create<T>(string _class_name) where T : GAObject, new()
    {
        return Create(_class_name) as T;
    }
    static System.Type t_static = typeof(GAObject);
};


public class Singleton<T> where T : new()
{
    public static T ins
    {
        get
        {
            return Singleton<T>.GetInstance();
        }
    }

    private static T _ins = default(T);

    public static T GetInstance()
    {
        if (_ins == null)
        {
            _ins = new T();
        }
        return _ins;
    }
    public static void DestroyInstance()
    {
        _ins = default(T);
    }
}


public class SingletonGAObject<T> : GAObject where T : GAObject, new()
{
    public static T ins
    {
        get
        {
            return GetInstance();
        }
    }

    private static T _ins = null;

    public static T GetInstance()
    {
        if (_ins == null)
        {
            _ins = new T();
            _ins.Init();
        }
        return _ins;
    }
    public static void DestroyInstance()
    {
        _ins.Dispose();
        _ins = null;
    }
}
