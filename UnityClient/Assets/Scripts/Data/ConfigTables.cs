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
        public static bool IsConflict(string a, string b)
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



    /// <summary>
    /// Skill 配置表 
    /// </summary>
    public static class Skill
    {

        /// <summary>
        /// lazy init
        /// </summary>
        public static void Init()
        {//TODO init with config table
            kv = HashTable<HashTable>.Create<HashTable>();

            //状态抵抗技能冲突  击退 眩晕
            kv["Skill62_1"] = Json.Decode("time:0,");

        }
        static HashTable<HashTable> kv = null;
    }


    /// <summary>
    /// skill 冲突表
    /// </summary>
    public static class SkillConflict
    {
        /// <summary>
        /// a的冲突表中是否有b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsConflict(SkillBase a, SkillBase b)
        {
            if (kv == null)
            {
                Init();
            }

            return IsConflict(a.GetName(), b.GetName());
        }
        /// <summary>
        /// a的打断列表 是否有 b，，，
        /// a是否能打断b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsConflict(string a, string b)
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


            kv["Skill62_1"] = Skill62_1_Data.ins.cancelable_skill;// ; "Skill62_2,Skill62_3,";
            kv["Skill62_2"] = Skill62_2_Data.ins.cancelable_skill;// ; "Skill62_2,Skill62_3,";
            kv["Skill62_3"] = Skill62_3_Data.ins.cancelable_skill;// ; "Skill62_2,Skill62_3,";
            kv["Skill61_1"] = "Skill61_2,Skill61_3,";
            kv["Skill61_2"] = "Skill61_1,Skill61_3,";
            kv["Skill61_3"] = "Skill61_1,Skill61_2,";




            //key    是否能打断  values    
        }
        static HashTable kv = null;
    }





    /// <summary>
    /// hero  
    /// </summary>
    public static class Hero
    {

        /// <summary>
        /// lazy init
        /// </summary>
        public static void Init()
        {//TODO init with config table
            kv = HashTable<HashTable>.Create<HashTable>();


            string json_6 = "type:6,prefab:Spine/6/Hero6,name:异界王子,";
            string json_2 = "type:2,prefab:Prefabs/Hero2,name:唐僧,";




            kv["6"] = Json.Decode(json_6);
            kv["2"] = Json.Decode(json_2);

        }
        public static HashTable Get(string key)
        {
            if (kv == null)
            {
                Init();
            }
            return kv.Get<HashTable>(key);
        }
        public static ArrayList GetSkillsList(string hero_type)
        {
            ArrayList ret = new ArrayList();
            if(hero_type == "6")
            { /// group 1
                ret.Add("Skill62_1");
                ret.Add("Skill62_2");
                ret.Add("Skill62_3");
                ret.Add("Skill6_Final");
                ret.Add("SkillForceCancel");

                //-- group 2
                ret.Add("Skill61_1");
                ret.Add("Skill61_2");
                ret.Add("Skill61_3");
                ret.Add("Skill6_Final");
                ret.Add("SkillForceCancel");

            }
            else if (hero_type == "2")
            {
                /// group 1
                ret.Add("Skill2_1");
                ret.Add("Skill2_2");
                ret.Add("Skill2_3");
                ret.Add("Skill2_4");
                ret.Add("SkillForceCancel");

                //-- group 2
                ret.Add("Skill2_1");
                ret.Add("Skill2_2");
                ret.Add("Skill2_3");
                ret.Add("Skill2_5");
                ret.Add("SkillForceCancel");



            }

            return ret;
        }
        static HashTable<HashTable> kv = null;
    }




}