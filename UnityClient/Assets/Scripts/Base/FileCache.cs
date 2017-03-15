/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
///  FILE cache 
/// </summary>
public sealed class FileCache
{
    public Files AddFiles(string name, Files f)
    {
        if (hash.Contains(name) == true)
        {
            hash[name] = f;
            return null;
        }
        hash.Add(name, f);
        return f;
    }

    public Files GetFiles(string name, bool auto_add = true)
    {
        if (hash.Contains(name))
        {
            return (hash[name] as Files).ResetLines();
        }
        if(auto_add)
        {
            Files file = Files.Create(name);
            if(file ==null)
            {
                return null;
            }
            hash.Add(name, file);
            return file;
        }
        return null;
    }

    public void Clear()
    {
        hash.Clear();
    }
    private FileCache()
    {

    }
    public static void PrintCacheStatus()
    {
        Debug.Log("FileCache: " + ins.hash.Count + " in Cache");
        foreach (DictionaryEntry kv in ins.hash)
        {
            Debug.Log("FileCache: " + kv.Key + " Size=" + (kv.Value as string).Length * sizeof (char));

        }
    }

    private Hashtable hash = new Hashtable();


    public static FileCache ins
    {
        get
        {
            return FileCache.GetInstance();
        }
    }

    private static FileCache _ins = null;

    public static FileCache GetInstance()
    {
        if (_ins == null)
        {
            _ins = new FileCache();
        }
        return _ins;
    }
    public static void DestroyInstance()
    {
        _ins.Clear();
        _ins = null;
    }
}
