/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
///  animation cache 
/// </summary>
public sealed class AnimationsCache : Singleton<AnimationsCache>
{
    public Animations AddAnimatons(string name, Animations ani)
    {
        if (hash.Contains(name) == true)
        {
            hash[name] = ani;
            return null;
        }
        hash.Add(name, ani);
        return ani.Clone();
    }

    public Animations GetAnimations(string name)
    {
        if (hash.Contains(name))
        {
            return (hash[name] as Animations).Clone();
        }
        return null;
    }

    /// <summary>
    ///  add animations with plist file 
    /// </summary>
    /// <param name="plist"></param>
    public Animations AddAnimationsWithFile(string plist)
    {
        string name = Utils.GetFileName(plist);

        var ani =GetAnimations(name);
        if (ani == null)
        {
            ani =AddAnimatons(name, Animations.CreateWithFile(plist));
        }
        return ani;
    }

    public void Clear()
    {
        hash.Clear();
    }
    public static void PrintCacheStatus()
    {
        Debug.Log("AnimationsCache: " + ins.hash.Count + " in Cache");
        foreach (DictionaryEntry kv in ins.hash)
        {
            Debug.Log("AnimationsCache: " + kv.Key);

        }
    }

    private Hashtable hash = new Hashtable();

}
