using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class EntityBaseConfigInfo : MonoBehaviour
{
    public int level;//等级
    public float moveSpeed;//基础移动速度
    public int hp;//基础血量
    public int mp;//基础蓝量
    public int exp_next_level;//升级所需经验
    public float scale;//动画大小
    public float crits_ratio = 10.0f;//暴击率//0~100 %
    public float crits_damage = 200.0f;//暴击伤害 表示200% 倍数

    public int damage = 10;//攻击力
    //public int defends = 0;//防御力
}

