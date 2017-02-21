/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;



/// <summary>
/// 提供了 全局托管脏数据的清理
/// </summary>
public sealed class AutoReleasePool
{
    public static AutoReleasePool ins
    {
        get
        {
            return AutoReleasePool.GetInstance();
        }
    }

    private static AutoReleasePool _ins = null;

    public static AutoReleasePool GetInstance()
    {
        if (_ins == null)
        {
            _ins = new AutoReleasePool();
        }
        return _ins;
    }


    public static void DestroyInstance()
    {
        _ins.Clear();
        _ins = null;
    }

    /// <summary>
    /// add object to the pool
    /// </summary>
    /// <param name="obj">GAObject</param>
    public void AddObject(GAObject obj)
    {
        if (list.Contains(obj))
        {
            Debug.LogWarning("AutoReleasePool:Obj has been in pool");
            return;
        }
        list.Add(obj);
        obj.isInAutoReleasePool = true;
    }

    /// <summary>
    ///  clear all object which in pool
    /// </summary>
    public void Clear()
    {
        foreach (GAObject obj in list)
        {
            obj.Dispose();
        }
        list.Clear();
    }



    ArrayList list = new ArrayList();


}