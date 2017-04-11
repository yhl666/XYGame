using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
class HeroBaseDataByLevel
{
    public int health;
    public int attack;
    public int exp_next_level;//升级所需经验
    public float crit;//暴击
    public float move_speed;

}

class HeroLevelData:MonoBehaviour
{
    public  HeroBaseDataByLevel[] heroBaseDataByLevels;
    public static HeroLevelData ins = null;
    void Awake()
    {
        ins = this;
    }
}

