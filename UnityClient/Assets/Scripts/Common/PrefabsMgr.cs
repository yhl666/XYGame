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

        obj = (GameObject)Object.Instantiate(obj, obj.transform.position,  obj.transform.localRotation);

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
