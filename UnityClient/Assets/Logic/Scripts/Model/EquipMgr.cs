/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using DAO;

/// <summary>
///  sub mgr of EquipMgr
/// </summary>
public sealed class EquipMgr :  SingletonGAObject<EquipMgr>
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
 
    public DAO.Equip GetTestEquip()
    {
        DAO.Equip dao = DAO.Equip.Create();
        dao.id = 2300;
        dao.type = "equip";//装备类型
        dao.name = "屠龙刀";// 装备名字
        dao.brief = "传说中的武器";//简介
        dao.detail = "倚天剑1段\n攻击:+10\n魔法+5\n\n一定几率造成屠龙效果。\n一定几率造成100%额外伤害。\n攻击时吸血10%。";//详细简介

        dao.damage = 20;//伤害加成
        dao.defend = 30;//防御加成
        dao.mp = 10;//魔法加成
        dao.hp = 230;//气血加成

     //   dao.buffers.Add("BufferEquipTest1");
    //    dao.buffers.Add("BufferEquipTest2");
        return dao;
    }

    public EquipMgr()
    {
        for (int i = 1300; i < 1308; i++)
        {
            DAO.Equip dao = DAO.Equip.Create(i);
            this.Add(dao);

        }
    }
}


