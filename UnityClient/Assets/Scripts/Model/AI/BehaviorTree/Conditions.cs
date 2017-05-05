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

    public class HasTower : ConditionBase
    {//存在塔
        public override bool Visit(Entity target)
        {
            if (BuildingMgr.ins.GetBuildings().Count > 0)
            {
             //   Debug.LogError("存在塔");
                return true;
            }
            return false;
        }
    }

    public class HasHitTower : ConditionBase
    {//攻击了塔
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return true;
            return host.is_hit_tower;
        }
    }
    public class HasNotHitTower : ConditionBase
    {//没有攻击塔
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return true;
            return !host.is_hit_tower;
        }
    }

    public class HasHitByHero : ConditionBase
    {//被玩家攻击
        bool ret = false;
        Entity hit_target = null;//被攻击的目标
        public override bool Visit(Entity target)
        {
            _host = target;
            if (ret)
            {
            //    Debug.LogError("被玩家攻击");
                ret = false;
                return true;
            }
            return false;
        }
        public override void OnEvent(int type, object userData)
        {
            if (this.IsInValid()) return;
            if (type == Events.ID_BATTLE_ENTITY_BEFORE_TAKEATTACKED)
            {
                AttackInfo info = userData as AttackInfo;
                Enemy host = _host as Enemy;
                if (host == null) return;
                if (info.target as Enemy != host) return;
                //自己被命中
                if (info.ownner.IsMaxTarget() == false)
                {
                   // host.target = info.ownner;
                    ret = true;
                    host.SetTag("BT_HitHero", info.ownner); // 信息 写入黑板
                }
            }
        }
        public override void OnEnter()
        {
            EventDispatcher.ins.AddEventListener(this, Events.ID_BATTLE_ENTITY_BEFORE_TAKEATTACKED);
        }
        public override void OnExit()
        {
            EventDispatcher.ins.RemoveEventListener(this, Events.ID_BATTLE_ENTITY_BEFORE_TAKEATTACKED);
        }
    }

    public class TargetIsHero : ConditionBase
    {//目标是玩家
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return false;
            if (host.target == null) return false;
            if (host.target.isDie) return false;

            return host.target.IsHero;
        }
    }
    public class TargetIsTower : ConditionBase
    {//目标是塔
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return false;
            if (host.target == null) return false;
            if (host.target.isDie) return false;
            if (host.target.IsTower)
            {
                //  Debug.LogError("目标是塔");
                return true;
            }
            return false;
        }
    }
    public class HasHeroInTargetRange : ConditionBase
    {//目标不在否在攻击范围内
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return false;
            if (host.target == null) return false;
            foreach (Entity h in HeroMgr.ins.GetHeros())
            {
                if (host.target_distance > host.ClaculateDistance(h.x, 0, h.z))
                {//范围内 
                    return true;
                }
            }
            return false;
        }
    }
    public class HasNotTarget : ConditionBase
    {//没有目标 
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return true;
            if (host.target == null) return true;
            return false;
        }
    }




}