/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class Skill6_Final_Data : MonoBehaviour
{
    /*
     
     8.	终结技：人物动画6000，6240_0,6240_1,6240_2，释放时背景消失，命中判定通普通攻击第一段，
     * 若第一段命中则播放后续三个动画，若第一段未命中则不会继续播放，能量条归0。
      
     */

    //终结技能分为2段 
    //common
    public int cd; // cd ，单位 帧数
    public int cancel;//可取消帧数，该数据是在技能释放后开始计算的帧数

    //---------以下是 第一阶段 的配置信息
    public float level1_damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    public Vector2 level1_hit_rect; // 伤害 判定框大小 ，，判定框中心为角色正中心
    public string level1_animation_name;// 技能角色动画名称
    public string level1_hit_animation_name;// 子弹动画名字
    public Vector2 level1_delta_xy; // 伤害判定 初试偏移量 中心点为角色中心点
    public int level1_hit_delay; // 第一次hit评定延时 单位 帧数
    //--- 以下是第二阶段（在第一阶段命中后 触发）
    public string level2_animation_name;// 技能角色动画名称

    //--- 以下是第三阶段（在第二阶段完成后 触发）
    public string level3_animation_name;// 技能角色动画名称


    //---以下是 第四阶段 （最后一击） 配置信息
    public float level4_damage_ratio; // 伤害比例 1.5 表示为基础伤害的 1.5倍
    public Vector2 level4_hit_rect; // 伤害 判定框大小 ，，判定框中心为角色正中心
    public string level4_animation_name;// 技能角色动画名称
    public string level4_hit_animation_name;// 子弹动画名字
    public Vector2 level4_delta_xy; // 伤害判定 初试偏移量 中心点为角色中心点
 


    public static Skill6_Final_Data ins = null;
    void Awake()
    {
        ins = this;
    }
}
