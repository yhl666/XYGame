using UnityEngine;
using System.Collections;


public class DAO
{
    public virtual void SetJson(string json)
    {


    }

    public virtual string ToJson()
    {
        return "";
    }


}



public class User : DAO
{
    public int no;
    public string name;
    public int level;
    public string timestamp = "0";
    public string type = "2";

    public override void SetJson(string json)
    {
        HashTable kv = Json.Decode(json);


        this.no = kv.GetInt("no");
        this.name = kv["name"];
        this.level = kv.GetInt("level");
        this.timestamp = kv["time"];
    }


}
