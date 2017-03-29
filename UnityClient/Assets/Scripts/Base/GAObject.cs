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
        this.OnDispose();
    }

    /// <summary>
    /// 触发 OnDisposoe事件， 和SetInValid 一样，不过是吧清理任务交给了 AutoReleasePool 统一清理
    /// </summary>
    public void LazyDispose()
    {
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



    ///----------------------- some helper function--------------

    public void log(string what)
    {
        Debug.Log(this.GetType().ToString() + ":" + what);
    }

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
            return Singleton<T>.GetInstance();
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
