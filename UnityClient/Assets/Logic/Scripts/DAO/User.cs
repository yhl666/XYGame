/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;



namespace DAO
{

    public sealed class User : DaoBase
    {
        public int no;//全局no
        public string name;//名字
        public int level;//等级
        public string time = "0";//上次登录时间戳
        public string type = "2";//英雄类型 
        public int exp = 0;//经验

        public int no_dec = 0;//穿上的饰品no
        public int no_atk = 0;// 穿上的武器no
        public int no_def = 0;// 穿上的防具no

        public int id_dec = 0;//穿上的饰品id
        public int id_atk = 0;// 穿上的武器id
        public int id_def = 0;// 穿上的防具id



        public void SetHashTable(HashTable kv)
        {
            this.no = kv.GetInt("no");
            this.name = kv["name"];
            this.level = kv.GetInt("level");
            this.time = kv["time"];
            this.no_atk = kv.GetInt("no_atk");
            this.no_def = kv.GetInt("no_def");
            this.no_dec = kv.GetInt("no_dec");

            this.id_atk = kv.GetInt("id_atk");
            this.id_def = kv.GetInt("id_def");
            this.id_dec = kv.GetInt("id_dec");

        }
        public override void SetJson(string json)
        {
            HashTable kv = Json.Decode(json);
            this.SetHashTable(kv);
        }

        public override string ToJson()
        {
            return "no:" + no.ToString() + ",name:" + name + ",level:" + level.ToString() + ",type:" + type.ToString() + ",time:" + time.ToString()
                + ",no_atk:" + this.no_atk + ",no_def:" + this.no_def + ",no_dec:" + no_dec +
                 ",id_atk:" + this.id_atk + ",id_def:" + this.id_def + ",id_dec:" + id_dec + ",";

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







}