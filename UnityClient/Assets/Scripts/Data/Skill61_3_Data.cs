/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


public class Skill61_3_Data : MonoBehaviour
{
    //common
    public int cd; // cd ，单位 帧数
    public int delayFrame;//延迟施法事件
    public float damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    public Vector3 hit_rect; // 伤害 判定框大小 ，，判定框中心为角色正中心
    public int cancel;//可取消帧数，该数据是在技能释放后开始计算的帧数
    public string animation_name;// 技能角色动画名称
    public float lastTime;//减速持续时间

    //special
    public int slowPrecent;//减速百分比



    public static Skill61_3_Data ins = null;
    void Awake()
    {
        ins = this;
    }
}
