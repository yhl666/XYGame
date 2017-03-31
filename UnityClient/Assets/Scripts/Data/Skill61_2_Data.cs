/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


public class Skill61_2_Data : MonoBehaviour
{
    //common
    public int cd; // cd ，单位 帧数
    public int bulletLaunchDealy;//子弹延迟发射的时间 帧数
    public float damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    public Vector3 hit_rect; // 伤害 判定框大小 ，，判定框中心为子弹正中心
    public int cancel;//可取消帧数，该数据是在技能释放后开始计算的帧数
    public string animation_name;// 技能角色动画名称

    public float distance;//子弹飞行距离
    public float speed;//子弹飞行速度


    public static Skill61_2_Data ins = null;
    void Awake()
    {
        ins = this;
    }
}
