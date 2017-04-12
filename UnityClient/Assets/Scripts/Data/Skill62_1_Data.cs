/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


[System.Serializable]
public class Skill62_1_DataLevelAble
{
    //common
    public string cancelable_skill;//能取消（打断）的技能名字， 逗号隔开

    public int cd; // cd ，单位 帧数
    public int last_time;//持续时间 单位 帧数
    public float damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    public Vector3 hit_rect; // 伤害 判定框大小 ，，判定框中心为角色正中心
    public int cancel;//可取消帧数，该数据是在技能释放后开始计算的帧数
    public string animation_name;// 技能角色动画名称
    //special
    public float added_speed_percent;//速度提成百分比，，10 表示提升10% 120 表示提升120%
    public float attach_distance;//吸附效果 距离在多少时结束  
    public float attach_last_time;//吸附效果持续时间
}
public class Skill62_1_Data : MonoBehaviour
{
    //common
    /*   public string cancelable_skill;//能取消（打断）的技能名字， 逗号隔开

       public int cd; // cd ，单位 帧数
       public int last_time;//持续时间 单位 帧数
       public float damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
       public Vector3 hit_rect; // 伤害 判定框大小 ，，判定框中心为角色正中心
       public int cancel;//可取消帧数，该数据是在技能释放后开始计算的帧数
       public string animation_name;// 技能角色动画名称
       //special
       public float added_speed_percent;//速度提成百分比，，10 表示提升10% 120 表示提升120%
       public float attach_distance;//吸附效果 距离在多少时结束  
       public float attach_last_time;//吸附效果持续时间
       */

    public Skill62_1_DataLevelAble level_1;
    public Skill62_1_DataLevelAble level_2;
    public Skill62_1_DataLevelAble level_3;
    void Awake()
    {
        _ins = this;
    }
    public static Skill62_1_DataLevelAble ins
    {
        get
        {
            return Get(1);//默认一级
        }
    }
    static Skill62_1_Data _ins = null;
    public static Skill62_1_DataLevelAble Get(int level)
    {
        if (level == 1)
        {
            return _ins.level_1;
        }
        else if (level == 2)
        {
            return _ins.level_2;
        }
        else if (level == 3)
        {
            return _ins.level_3;
        }
        else
        {
            return Get(1);//默认返回等级1
        }
        return null;
    }
}
