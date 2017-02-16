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

    public class User : DaoBase
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
            return "no:" + no.ToString() + ",name:" + name + ",level:" + level.ToString() + ",type:" + type .ToString() +  ",time:" + time .ToString()+  ",";
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