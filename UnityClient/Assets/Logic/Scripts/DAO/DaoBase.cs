/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;



namespace DAO
{
    public class DaoBase
    {
        public virtual void SetJson(string json)
        {


        }

        public virtual string ToJson()
        {
            return "";
        }

        public virtual void InitWithStatic()
        {

        }

    }


    /// <summary>
    /// 装备
    /// </summary>
    public sealed class Equip : DaoBase
    {
        public int id;//装备id
        public int ownner;//拥有者id
        public int no;//全局id
        public int level; // 等级
        public int exp;


        //静态数据
        public string type="equip";//装备类型
        public string name;// 装备名字
        public string brief;//简介
        public string detail;//详细简介

        public int damage;//伤害加成
        public int defend;//防御加成
        public int mp;//魔法加成
        public int hp;//气血加成

        public float crits_ratio = 10.0f;//暴击率 加成 //0~100 %
        public float crits_damage = 10.0f;//暴击伤害 加成表示200% 倍数
        public float cure = 0.0f;//治疗加成

        public string png;//图片文件名




        public ArrayList buffers = new ArrayList();//buffers id

    

        public void SetHashTable(HashTable kv)
        {
            this.no = kv.GetInt("no");
            this.ownner = kv.GetInt("ownner");
            this.level = kv.GetInt("level");
            this.exp = kv.GetInt("exp");
            this.id = kv.GetInt("id");
            this.InitWithStatic();
        }
        public override void InitWithStatic()
        {
            this.png = "hd/interface/items/" + id.ToString() + ".png";
        }
        public override void SetJson(string json)
        {
            HashTable kv = Json.Decode(json);
            this.SetHashTable(kv);
        }

        public override string ToJson()
        {
            return "no:" + no.ToString() + ",ownner:" + ownner + ",level:" + level.ToString() + ",id:" + id.ToString() + ",exp:" + exp.ToString() + ",";
        }
        public static Equip Create(string json)
        {
            Equip ret = new Equip();
            ret.SetJson(json);
            return ret;
        }

        public static Equip Create(HashTable kv)
        {
            Equip ret = new Equip();
            ret.SetHashTable(kv);
            return ret;
        }

        public static Equip Create(int id)
        {   
            Equip dao= new Equip();
            dao.id = id;
            dao.InitWithStatic();
            return dao;
        }
        public static Equip Create()
        {
            Equip dao = new Equip();
            return dao;
        }
    }


    //道具
    public sealed class Goods : DaoBase
    {
        public int id;//id
        public int ownner;//拥有者id
        public int no;//全局id
        public int count; // 数量


        //静态数据
        public string type;//类型
        public string name;// 名字
        public string brief;//简介
        public string detail;//详细简介


        public string png;//图片文件名
        public VoidFuncObject _OnUse = null;


        public override void SetJson(string json)
        {
            base.SetJson(json);
        }
        public override string ToJson()
        {
            return base.ToJson();
        }
    
    }



}