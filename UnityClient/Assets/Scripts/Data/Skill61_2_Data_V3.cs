using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Skill61_2_Data_V3 : MonoBehaviour
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
    public float scale_x;//特效大小的X
    public float scale_y;//特效大小的y
    public bool immediateDisappear;//是否立即消失
    public int playTimes;//播放时间

    public static Skill61_2_Data_V3 ins = null;
    void Awake()
    {
        ins = this;
    }
}