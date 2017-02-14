using UnityEngine;
using System.Collections;

public class Json
{

    /// <summary>
    /// 解析json 到 hash table
    /// 只支持 简单key value 的json
    /// "name:css,pwd:111,id:15454,"
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static HashTable Decode(string json)
    {
        if (json == "" || json == null) return null;
        HashTable ret = HashTable.Create();
        int last = 0;

        string k = "0", v = "0";
        for (int i = 0; i < json.Length; i++)
        {
            char ch = json[i];
            if (ch.Equals(':'))
            {
                k = json.Substring(last, i - last);
                last = i + 1;
            }

            if (ch.Equals(','))
            {
                v = json.Substring(last, i - last);
                last = i + 1;
                ret.Set(k, v);
            }

        }

        return ret;
    }

    /// <summary>
    /// 打包hash table 到json
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static string Encode(HashTable t)
    {
        string ret = "";
        Hashtable table = t.GetHashtable();
        foreach (DictionaryEntry kv in table)
        {
            ret += kv.Key.ToString() + ":" + kv.Value.ToString() + ",";
        }
        return ret;
    }



    public static ArrayList MultiDecode(string json)
    {
        ArrayList ret = new ArrayList();
        int last = 1;

        for (int i = 0; i < json.Length; i++)
        {
            char ch = json[i];
            if (ch.Equals('}'))
            {
                string sub = json.Substring(last, i - last);
                ret.Add(Json.Decode(sub));

                last = i + 2;
            }
        }
        if (ret.Count == 0) return null;
        return ret;
    }



    /// <summary>
    /// 打包hash table 到json
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static string MultiEncode(ArrayList list)
    {
        string ret = "";

        foreach (HashTable table in list)
        {
            ret += "{" + table.ToJson() + "}";
        }

        return ret;
    }
}
