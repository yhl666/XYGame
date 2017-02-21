/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public sealed class HashTable
{

    public static HashTable Create()
    {
        HashTable ret = new HashTable();
        return ret;
    }

    public static HashTable CreateWithJson(string json)
    {
        return Json.Decode(json);
    }
    private HashTable() { }
    private Hashtable kv = new Hashtable();

    public int GetInt(string key)
    {
        return int.Parse(this.Get(key));
    }

    public float GetFloat(string key)
    {
        return float.Parse(this.Get(key));
    }





    public void Set(string key, string value)
    {
        if (kv.Contains(key))
        {
            kv[key] = value; return;
        }
        kv.Add(key, value);
    }
    public string this[string key]
    {
        get
        {
            return this.Get(key);
        }

        set
        {
            this.Set(key, value);
        }
    }
    public string Get(string key)
    {
        if (kv.Contains(key))
        {
            return kv[key] as string;
        }
        return "";
    }
    public int Count
    {
        get
        {
            return kv.Count;
        }
    }

    public Hashtable GetHashtable()
    {
        return kv;
    }

    public string ToJson()
    {
        return Json.Encode(this);
    }
}

