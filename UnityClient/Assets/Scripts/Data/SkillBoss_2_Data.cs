/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

[AddComponentMenu("DATA/BossSkill2")]
public class SkillBoss_2_Data: MonoBehaviour
{
    //common
    public int cd; // cd ，单位 帧数
    public float damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    public Vector3 hit_rect; // 伤害 判定框大小 ，，判定框中心为子弹正中心
    public Vector3 delta_xyz;//长矛初始偏移量
    public string hit_animation_name;// 长矛动画
    public string animation_name; // 角色动画名字
    public float scale_xy;//长矛动画缩放比例
  //  public float distance;//子弹飞行距离
  //  public float speed;//长矛 最大飞行速度
    public int max_buf_level;//buf 最大叠加层数
    public int per_buf_damage_percent;//每层buf  攻击力加成百分比
    public int auto_atk_interval;//自动攻击间隔 单位帧


    public static SkillBoss_2_Data ins = null;
    void Awake()
    {
        ins = this;
    }
}
