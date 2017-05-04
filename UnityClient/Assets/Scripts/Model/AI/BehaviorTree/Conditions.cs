/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com
  Behavior Tree  's Conditions
 * 行为树游戏逻辑部分
 */
using UnityEngine;
using System.Collections;

namespace BehaviorTree.Condition
{
    //--------------------------------------------------游戏逻辑实际的 Condition  
    public class TargetHasNotInAtkRange : ConditionBase
    {//目标不在否在攻击范围内
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return false;
            if (host.target == null) return false;
            if (host.atk_range > host.target.ClaculateDistance(host))
            {//范围内 
                return false;
            }
            return true;
        }
    }
    public class IsCDMax : ConditionBase
    {//CD是否结束
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return false;
            if (host.target == null) return false;
            if (host.cd.IsMax())
            {
                host.cd.Reset();
                return true;
            }
            return false;
        }
    }
    public class NotTargetOrDie : ConditionBase
    {//没有目标或者死亡
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return true;
            if (host.target == null) return true;
            if (host.target.isDie) return true;
            return false;
        }
    }
}