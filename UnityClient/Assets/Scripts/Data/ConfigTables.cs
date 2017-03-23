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
        /// <summary>
        /// a的冲突表中是否有b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsConflict(Buffer a, Buffer b)
        {
            if (kv == null)
            {
                Init();
            }

            return IsConflict(a.GetName(), b.GetName());
        }
        /// <summary>
        /// a的冲突表中是否有b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsConflict(string a ,string b )
        {
            if (kv == null)
            {
                Init();
            }
            string s = kv.Get(a);
            if (s.IndexOf(b) == -1) return false;
            return true;
        }
        /// <summary>
        /// lazy init
        /// </summary>
        public static void Init()
        {//TODO init with config table
            kv = HashTable.Create();

            //状态抵抗技能冲突  击退 眩晕
            kv["BufferNegativeUnbeatable"] = "BufferHitBack,BufferSpin";
     
        }
        static HashTable kv = null;
    }

 

















}