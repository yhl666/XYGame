/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


public class DaoBase
{
    public virtual void SetJson(string json)
    {


    }

    public virtual string ToJson()
    {
        return "";
    }


}
namespace DAO
{

    public sealed class User : DaoBase
    {
        public int no;
        public string name;
        public int level;
        public string time = "0";
        public string type = "2";



        public void SetHashTable(HashTable kv)
        {
            this.no = kv.GetInt("no");
            this.name = kv["name"];
            this.level = kv.GetInt("level");
            this.time = kv["time"];
        }
        public override void SetJson(string json)
        {
            HashTable kv = Json.Decode(json);
            this.SetHashTable(kv);
        }

        public override string ToJson()
        {
            return "no:" + no.ToString() + ",name:" + name + ",level:" + level.ToString() + ",type:" + type.ToString() + ",time:" + time.ToString() + ",";
        }
        public static User Create(string json)
        {
            User ret = new User();
            ret.SetJson(json);
            return ret;
        }

        public static User Create(HashTable kv)
        {
            User ret = new User();
            ret.SetHashTable(kv);
            return ret;
        }


        public static User Create()
        {
            User ret = new User();
            return ret;
        }

        private User() { }
    }







    public sealed class Equip : DaoBase
    {
        public string id;//全局id
        public string ownner;//拥有者id
        public string no;//装备id
        public int level; // 等级
        public int exp;


        //
        public string type;
        public string name;
        public string brief;
        public string detail;

        public int damage;//伤害加成
        public int defend;//防御加成
        public int mp;//魔法加成
        public int hp;//气血加成

        public float crits_ratio = 10.0f;//暴击率 加成 //0~100 %
        public float crits_damage = 10.0f;//暴击伤害 加成表示200% 倍数
        public float cure=0.0f;//治疗加成






        public ArrayList buffers = new ArrayList();//buffers id

    }





}