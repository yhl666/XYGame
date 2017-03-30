/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;



public class Vector
{
    public bool Contains(GAObject obj)
    {
        return this.list.Contains(obj) ;
    }
    public void PushBack(GAObject obj)
    {
        this.list.Add(obj);
    }
    public void PushFront(GAObject obj)
    {
        this.list.Insert(0, obj);
    }
    /// <summary>
    /// 移除容器中第一个匹配项
    /// </summary>
    /// <param name="obj"></param>
    public void Remove(GAObject obj)
    {
        this.list.Remove(obj);
    }
    /// <summary>
    /// 移除容器中所有匹配项
    /// </summary>
    /// <param name="obj"></param>
    public void RemoveAll(GAObject obj)
    {
        for (int i = 0; i < list.Count; )
        {
            GAObject b = list[i] as GAObject;
            if (b == obj)
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
    /// 清空
    /// </summary>
    public void Clear()
    {
        list.Clear();
    }
    public int Count()
    {
        return list.Count;
    }
    public Vector Clone()
    {
        Vector ret = new Vector();
        ret.list = list.Clone() as ArrayList;
        return ret;
    }
    public GAObject this[int index]
    {
        get
        {
            return list[index] as GAObject;
        }

        set
        {
            list[index] = value as GAObject;
        }
    }

    /// <summary>
    /// 相同元素的个数
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetCount(GAObject obj)
    {
        int ret = 0;
        foreach (GAObject obj1 in list)
        {
            if (obj == obj1) ++ret;
        }
        return ret;
    }

    public Vector()
    {
        list.Capacity = 100;
    }

    private ArrayList list = new ArrayList();
}
