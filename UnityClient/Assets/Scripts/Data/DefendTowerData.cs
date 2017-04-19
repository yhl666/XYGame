/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

[System.Serializable]
public class DefendTowerDataBaseInfo
{
    public int hp = 5000;
    public int damage = 200;
}

[AddComponentMenu("DATA/DefentTower-防御塔")]
public class DefendTowerData : MonoBehaviour
{
    public int power_level_init = 3;//初始能量等级
    public int power_level_max = 3;// 最大能量等级
    public int cd_power_level = 40;// 充能一次 所需时间，单位:帧数
    public string animation_name;//波的动画文件名字
    public float distance = 10.0f; // 波 移动距离
    public Vector3 delta_xy; // 波初始偏移量
    public Vector3 collider_size; // 波的碰撞盒大小
    public Vector2 scale; // 动画效果 缩放比例
    public float damage_ratio;//伤害比例
    public int hp; // 血量
    public int damage;//基础伤害
    public bool faceto_left = false;//朝向左边

    public DefendTowerDataBaseInfo [] levels;
 

    public static DefendTowerData ins = null;
    void Awake()
    {
        ins = this;
    }
}
