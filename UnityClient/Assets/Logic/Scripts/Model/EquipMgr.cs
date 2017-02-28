using UnityEngine;
using System.Collections;
using DAO;

/// <summary>
///  sub mgr of EquipMgr
/// </summary>
public sealed class EquipMgr : GAObject
{
    public void Add(Equip obj)
    {
        this.lists.Add(obj);
    }
    public void Remove(Equip obj)
    {
        this.lists.Remove(obj);
    }


    public override void OnDispose()
    {
        this.lists.Clear();
        _ins = null;
    }
    public void Remove(int no)
    {
        Equip obj = this.GetEquip(no);
        if (obj == null) return;
        this.Remove(obj);
    }


    public ArrayList GetEquips()
    {
        return lists;
    }

    public Equip GetEquip(int no)
    {
        foreach (Equip obj in lists)
        {
            if (obj.no == no) return obj;
        }
        return null;
    }


    ArrayList lists = new ArrayList();

    public static EquipMgr ins
    {
        get
        {
            return EquipMgr.GetInstance();
        }
    }

    private static EquipMgr _ins = null;

    public static EquipMgr GetInstance()
    {
        if (_ins == null)
        {
            _ins = new EquipMgr();
        }
        return _ins;
    }


}


