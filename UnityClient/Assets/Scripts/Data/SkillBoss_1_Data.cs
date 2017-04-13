/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

[AddComponentMenu("DATA/BossSkill1")]
public class SkillBoss_1_Data: MonoBehaviour
{
    //common
    public int cd; // cd ，单位 帧数
    public float damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    public Vector3 hit_rect; // 伤害 判定框大小 ，，判定框中心为子弹正中心
    public Vector3 delta_xyz;//评定框偏移量
    public string hit_animation_name_1;//第一次 攻击特效动画名字
    public string hit_animation_name_2;//第二次 攻击特效动画名字
    public string hit_animation_name_3;//第三次 攻击特效动画名字

    public float distance;//距离名表店多近时 停止冲锋
    public float run_speed_ratio;//冲向玩家速度比例

    public string animation_name_1; // 第1次攻击 角色动画名字
    public string animation_name_2; // 第2次攻击 角色动画名字
    public string animation_name_3; // 第3次攻击 角色动画名字

    public static SkillBoss_1_Data ins = null;
    void Awake()
    {
        ins = this;
    }
}
