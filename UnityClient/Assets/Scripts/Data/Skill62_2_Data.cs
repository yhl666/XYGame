/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
/*
public class Skill62_2_Data : MonoBehaviour
{

    //common
    public string cancelable_skill;//能取消（打断）的技能名字， 逗号隔开

    public int cd; // cd ，单位 帧数
    public float damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    public Vector3 hit_rect; // 伤害 判定框大小 ，，判定框中心为角色正中心
    public int cancel;//可取消帧数，该数据是在技能释放后开始计算的帧数
    public string animation_name;// 技能角色动画名称
    public string hit_animation_name;// 子弹动画名字
    public int tick_delay;//延时多少帧 释放伤害判定
    public Vector3 delta_xyz; // 伤害判定 初试偏移量 中心点为角色中心点


    public static Skill62_2_Data ins = null;
    void Awake()
    {
        ins = this;
    }
}
*/

[System.Serializable]
public class Skill62_2_DataLevelAble  
{

    //common
    public string cancelable_skill;//能取消（打断）的技能名字， 逗号隔开

    public int cd; // cd ，单位 帧数
    public int damage;//真实伤害数值
    public int hp_reduce;//每次伤害减少的 血量
    public float time;//伤害间隔，单位秒
    public Vector3 hit_rect; // 伤害 判定框大小 ，，判定框中心为角色正中心
    //   public string animation_name;// 技能角色动画名称
    public string hit_animation_name;// 子弹动画名字
    public Vector3 delta_xyz; // 伤害判定 初试偏移量 中心点为角色中心点

    [HideInInspector]
    public float damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    [HideInInspector]
    public int cancel;//可取消帧数，该数据是在技能释放后开始计算的帧数
    [HideInInspector]
    public string animation_name;// 技能角色动画名称
    [HideInInspector]
    public int tick_delay;//延时多少帧 释放伤害判定
}


//重做
public class Skill62_2_Data : MonoBehaviour
{
    public Skill62_2_DataLevelAble level_1;
    public Skill62_2_DataLevelAble level_2;
    public Skill62_2_DataLevelAble level_3;
    void Awake()
    {
        _ins = this;
    }
    public static Skill62_2_DataLevelAble ins
    {
        get
        {
            return Get(1);//默认一级
        }
    }
    static Skill62_2_Data _ins = null;
    public static Skill62_2_DataLevelAble Get(int level)
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
