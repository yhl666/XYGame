/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public sealed class PrefabsMgr
{
    public static GameObject Load(string asset)
    {
          GameObject obj = Resources.Load(asset, typeof(GameObject)) as GameObject;
          if (obj == null)
          {
              Debug.LogError("PrefabsMgr.Load " + asset);
          }

          obj = (GameObject)Object.Instantiate(obj, obj.transform.position, obj.transform.localRotation);
      //  return PrefabsCache.ins.GetPrefabs(asset);
        return obj;
    }


    public static GameObject LoadWithoutCache(string asset)
    {
        GameObject obj = Resources.Load(asset, typeof(GameObject)) as GameObject;
        if (obj == null)
        {
            Debug.LogError("PrefabsMgr.Load " + asset);
            return null;
        }

        //  obj = (GameObject)Object.Instantiate(obj, obj.transform.position, obj.transform.localRotation);

        return obj;
    }

    public static void Destroy(GameObject obj)
    {
        UnityEngine.Object.Destroy(obj);
    }

    public static void DestroyImmediate(GameObject obj)
    {
        UnityEngine.Object.DestroyImmediate(obj);
    }

}


/// <summary>
///  prefabs cache 
/// </summary>
public sealed class PrefabsCache : Singleton<PrefabsCache>
{
    public void AddPrefabs(string name, GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogError("Can not be null");
            return;
        }
        if (hash.Contains(name) == true)
        {
            hash[name] = obj;
            return;
        }
        hash.Add(name, obj);
    }

    public GameObject GetPrefabs(string file, bool auto_add = true)
    {
        if (hash.Contains(file))
        {
            GameObject obj = hash[file] as GameObject;
            return (GameObject)Object.Instantiate(obj, obj.transform.position, obj.transform.localRotation);
        }
        else if (auto_add)
        {
            GameObject obj = PrefabsMgr.LoadWithoutCache(file);
            if (obj == null) return null;
            this.AddPrefabs(file, obj);
            return GetPrefabs(file, false);
        }
        return null;
    }

    public void AddPrefabsWithFile(string file)
    {
     //   var ani = GetPrefabs(file);
       // if (ani == null)
        {
            AddPrefabs(file, PrefabsMgr.LoadWithoutCache(file));
        }
        //   return ani;
    }

    public void Clear()
    {
        hash.Clear();
    }
    public static void PrintCacheStatus()
    {
        Debug.Log("PrefabsCache: " + ins.hash.Count + " in Cache");
        foreach (DictionaryEntry kv in ins.hash)
        {
            Debug.Log("PrefabsCache: " + kv.Key);
        }
    }

    private Hashtable hash = new Hashtable();
}
