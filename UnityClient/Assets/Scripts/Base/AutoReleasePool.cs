/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

/// <summary>
/// 提供了 全局托管脏数据的清理
/// </summary>
public sealed class AutoReleasePool:Singleton<AutoReleasePool>
{

    /// <summary>
    /// add object to the pool
    /// </summary>
    /// <param name="obj">GAObject</param>
    public void AddObject(GAObject obj)
    {
        if (list.Contains(obj))
        {
            Debug.LogWarning("AutoReleasePool:Obj has been in pool Type: class " + obj.GetType().ToString());
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