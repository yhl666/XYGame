/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com
配置表 
 */
using UnityEngine;
using System.Collections;
namespace ConfigTables
{
    /// <summary>
    /// buffer 冲突表
    /// </summary>
    public static class BufferConflict
    {
        public static bool IsConflict(Buffer a, Buffer b)
        {
            if (kv == null)
            {
                Init();
            }

            string s = kv.Get(a.GetName());
            if (s.IndexOf(b.GetName()) == -1) return false;
            return true;
        }

        public static void Init()
        {//TODO init with config table
            kv = HashTable.Create();

            kv["BufferNegativeUnbeatable"] = "BufferHitBack";

        }
        static HashTable kv = null;
    }


























}