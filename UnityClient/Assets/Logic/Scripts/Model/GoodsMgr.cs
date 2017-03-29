/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using DAO;

/// <summary>
///  sub mgr of GoodsMgr
/// </summary>
public sealed class GoodsMgr :  SingletonGAObject<GoodsMgr>
{
    public void Add(Goods obj)
    {
        this.lists.Add(obj);
    }
    public void Remove(Goods obj)
    {
        this.lists.Remove(obj);
    }


    public override void OnDispose()
    {
        this.lists.Clear();
    }
    public void Remove(int no)
    {
        Goods obj = this.GetGoods(no);
        if (obj == null) return;
        this.Remove(obj);
    }


    public ArrayList GetGoods()
    {
        return lists;
    }

    public Goods GetGoods(int no)
    {
        foreach (Goods obj in lists)
        {
            if (obj.no == no) return obj;
        }
        return null;
    }


    ArrayList lists = new ArrayList();

}



