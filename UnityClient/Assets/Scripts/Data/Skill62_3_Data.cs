/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

[System.Serializable]
public class Skill62_3_DataLevelAble
{
    //common
    public string cancelable_skill;//能取消（打断）的技能名字， 逗号隔开

    public int cd; // cd ，单位 帧数
    public float damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    public Vector3 hit_rect; // 伤害 判定框大小 ，，判定框中心为角色正中心
    public int cancel;//可取消帧数，该数据是在技能释放后开始计算的帧数
    public string animation_name;// 技能角色动画名称
    public string hit_animation_name;// 子弹动画名字
    public Vector3 delta_xyz; // 伤害判定 初试偏移量 中心点为角色中心点

    public int start_jump; // 起跳持续时间， 单位 帧数
    public float jump_speed;// 起跳速度
    public float gravity_ratio;//重力比例  以默认值1.0 为参照比例

    public float spin_time; // 眩晕时间 单位秒

    public int sector_angle;//扇形角度
    public float sector_radius;//扇形半径
}


public class Skill62_3_Data : MonoBehaviour
{

    public Skill62_3_DataLevelAble level_1;
    public Skill62_3_DataLevelAble level_2;
    public Skill62_3_DataLevelAble level_3;
    void Awake()
    {
        _ins = this;
    }
    public static Skill62_3_DataLevelAble ins
    {
        get
        {
            return Get(1);//默认一级
        }
    }
    static Skill62_3_Data _ins = null;
    public static Skill62_3_DataLevelAble Get(int level)
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
