﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
///  animation cache 
/// </summary>
public sealed class AnimationsCache
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
    public void AddAnimationsWithFile(string plist)
    {
        // parse plist file





    }


    public void Clear()
    {
        hash.Clear();
    }
    private AnimationsCache()
    {

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


    public static AnimationsCache ins
    {
        get
        {
            return AnimationsCache.GetInstance();
        }
    }

    private static AnimationsCache _ins = null;

    public static AnimationsCache GetInstance()
    {
        if (_ins == null)
        {
            _ins = new AnimationsCache();
        }
        return _ins;
    }


}


