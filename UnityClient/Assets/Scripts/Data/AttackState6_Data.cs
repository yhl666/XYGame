/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class AttackState6_Data : MonoBehaviour
{ //普通攻击配置信息

    //common

    //---------以下是 第一阶段 的配置信息
    public int level1_cd; // cd ，单位 帧数
    public float level1_damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    public Vector3 level1_hit_rect; // 伤害 判定框大小 ，，判定框中心为角色正中心
    public string level1_animation_name;// 技能角色动画名称
    public int level1_cancel;//可取消帧数，该数据是在技能释放后开始计算的帧数
    public string level1_hit_animation_name;// 子弹动画名字
    public Vector3 level1_delta_xyz; // 伤害判定 初试偏移量 中心点为角色中心点
    public int level1_hit_delay; // 第一次hit评定延时 单位 帧数
    public float level1_move_x;//攻击时产生的x位移值，该值为释放是 一次性产生

    //--- 以下是第二阶段的配置信息
    public int level2_timeout;//第二段连招 超时帧数，在第一阶段完成后(或被打断后) 开始计算
    public float level2_damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    public Vector3 level2_hit_rect; // 伤害 判定框大小 ，，判定框中心为角色正中心
    public string level2_animation_name;// 技能角色动画名称
    public int level2_cancel;//可取消帧数，该数据是在技能释放后开始计算的帧数
    public string level2_hit_animation_name;// 子弹动画名字
    public Vector3 level2_delta_xyz; // 伤害判定 初试偏移量 中心点为角色中心点
    public int level2_hit_delay; // 第一次hit评定延时 单位 帧数
    public float level2_move_x;//攻击时产生的x位移值，该值为释放是 一次性产生

    //--- 以下是第三阶段的配置信息
    public int level3_timeout;//第三段连招 超时帧数，在第一阶段完成后(或被打断后) 开始计算
    public float level3_damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    public Vector3 level3_hit_rect; // 伤害 判定框大小 ，，判定框中心为角色正中心
    public string level3_animation_name;// 技能角色动画名称
    public int level3_cancel;//可取消帧数，该数据是在技能释放后开始计算的帧数
    public string level3_hit_animation_name;// 子弹动画名字
    public Vector3 level3_delta_xyz; // 伤害判定 初试偏移量 中心点为角色中心点
    public int level3_hit_delay; // 第一次hit评定延时 单位 帧数
    public float level3_move_x;//攻击时产生的x位移值，该值为释放是 一次性产生



    public static AttackState6_Data ins = null;
    void Awake()
    {
        ins = this;
    }
}
