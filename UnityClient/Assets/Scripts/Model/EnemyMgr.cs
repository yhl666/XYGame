/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


/// <summary>
///  sub mgr of ModelMgr
/// </summary>
public sealed class EnemyMgr :  SingletonGAObject<EnemyMgr>
{
    public static Enemy Create<T>() where T : new()
    {
        Enemy ret = new T() as Enemy;
        ret.Init();
        ins.Add(ret);
        return ret;
    }

    public void Add(Enemy b)
    {
        this.lists.Add(b);
        b.OnEnter();
    }
    public void Remove(Enemy b)
    {
        this.lists.Remove(b);
        b.OnExit();
        b.LazyDispose();
    }
    public override void OnDispose()
    {
        this.lists.Clear();
    }
    public override void UpdateMS()
    {
        foreach (Enemy b in lists)
        {
            b.AI_UpdateMSWithAI();
            EventDispatcher.ins.PostEvent(Events.ID_BEFORE_ONEENTITY_UPDATEMS, b);
            b.UpdateMS();
            EventDispatcher.ins.PostEvent(Events.ID_AFTER_ONEENTITY_UPDATEMS, b);
        }

        //clear
        for (int i = 0; i < lists.Count; )
        {
            Enemy b = lists[i] as Enemy;
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

    public ArrayList GetEnemys()
    {
        return lists;
    }

    public Enemy GetEnemy(int no)
    {
        foreach (Enemy e in lists)
        {
            if (e.no == no) return e;
        }
        return null;
    }
    public int GetEnemyCount()
    {
        return lists.Count;
    }
    ArrayList lists = new ArrayList();

}



