/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


public class Skill61_1_Data : MonoBehaviour
{
    //common
    public int cd; // cd ，单位 帧数
    public int bulletLaunchDealy;//子弹延迟发射的时间 帧数

    public float damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    public Vector2 PositionVector2; // 判定框位于人物中心的位置
    public Vector3 hit_rect;//判定框的大小
    public int cancel;//可取消帧数，该数据是在技能释放后开始计算的帧数
    public string animation_name;// 技能角色动画名称
    public float spinTime;//眩晕时间




    public static Skill61_1_Data ins = null;
    void Awake()
    {
        ins = this;
    }
}
