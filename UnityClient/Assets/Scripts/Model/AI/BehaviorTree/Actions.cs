/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com
  Behavior Tree  's Actions
 * 行为树游戏逻辑部分
 */
using UnityEngine;
using System.Collections;

namespace BehaviorTree.Action
{
    //--------------------------------------------------游戏逻辑实际的  Action
    public class SearchNearestTarget : ActionBase
    {//寻找最近的玩家作为目标
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;

            if (host == null) return false;

            float minDis = float.MaxValue;
            Entity t = null;
            foreach (Entity h in HeroMgr.ins.GetHeros())
            {//找出一个最近的玩家 作为锁定目标
                if (h.IsMaxTarget())
                {
                    continue;
                }
                float dis = h.ClaculateDistance(host.x, host.y);
                if (dis < minDis)
                {
                    t = h;
                    minDis = dis;
                }
            }

            if (t != null)
            {
                host.target = t;
                host.is_hit_tower = false;
                return true;
            }

            return false;
        }
    }

    public class MoveToTarget : ActionBase
    {
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return false;
            host.dir = (int)Utils.GetAngle(host.pos, host.target.pos);//委托给Run状态去做
            return true;
        }
    }
    public class AttackTarget : ActionBase
    {
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return false;
            if (host.target.IsHero && host.IsHeroRandomAtk(0.03f))
            {
                // 随机化攻击  减弱攻击强度
                target.atk = true;
                return true;
            }
            return false;
        }
    }

    public class SearchNearestTower : ActionBase
    {//仇恨指向最近的塔
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;

            if (host == null) return false;

            float minDis = float.MaxValue;
            Entity t = null;
            foreach (Entity h in BuildingMgr.ins.GetBuildings())
            {//找出一个最近的玩家 作为锁定目标
                if (h.IsMaxTarget())
                {
                    continue;
                }
                float dis = h.ClaculateDistance(host.x, host.y);
                if (dis < minDis)
                {
                    t = h;
                    minDis = dis;
                }
            }

            if (t != null)
            {
                host.target = t;
                host.is_hit_tower = false;
                // Debug.LogError("目标指向最近的塔");
                return true;
            }

            return false;
        }
    }
    public class SearchNearestHero : ActionBase
    {//寻找最近的玩家作为目标
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;

            if (host == null) return false;

            float minDis = float.MaxValue;
            Entity t = null;
            foreach (Entity h in HeroMgr.ins.GetHeros())
            {//找出一个最近的玩家 作为锁定目标
                if (h.IsMaxTarget())
                {
                    continue;
                }
                float dis = h.ClaculateDistance(host.x, host.y);
                if (dis < minDis)
                {
                    t = h;
                    minDis = dis;
                }
            }

            if (t != null)
            {
                host.target = t;
                host.is_hit_tower = false;
                //  Debug.LogError("目标指向最近的玩家");
                return true;
            }

            return false;
        }
    }

    public class SetTarget : ActionBase
    {
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            var obj = host.GetTagPairOnce("BT_HitHero");
            if (obj == null)
            {
                host.target = null;
                return false;
            }
            host.target = obj.value as Entity;
            //   Debug.LogError("重新设置目标BT_HitHero");
            return true;
        }
    }
    public class NotHitTower : ConditionBase
    {//重置没有攻击塔 
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return false;
            host.is_hit_tower = false;
            return true;
        }
    }
    public class MoveRandom : ActionBase
    {
        int dir = 0;
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return false;
            if (host.target == null) return false;
            if (host.target.isDie) return false;


            if (Utils.random_frameMS.Next(0, 300) == 5)
            { // 地图范围内的随机目标点
                Terrain terrain = AppMgr.GetCurrentApp<BattleApp>().GetCurrentWorldMap().GetTerrain();
                Vector2 to = new Vector2(Utils.random_frameMS.Next((int)(terrain.limit_x_left * 1000), (int)(terrain.limit_x_right * 1000)) / 1000f, Utils.random_frameMS.Next((int)(terrain.limit_z_down * 1000), (int)(terrain.limit_z_up * 1000)) / 1000f);
                this.dir = (int)Utils.GetAngle(host.pos, to);
            }
            host.dir = this.dir;
            return true;
        }
    }

    public class MoveRandomInAtkRange : ActionBase
    {
        int dir = 0;
        public override bool Visit(Entity target)
        {
            Enemy host = target as Enemy;
            if (host == null) return false;
            if (host.target == null) return false;
            if (host.target.isDie) return false;

            if (Utils.random_frameMS.Next(1, 200) == 2 || Mathf.Abs(host.atk_range - host.target.ClaculateDistance(host)) < 0.2f)
            {//一定概率 随机方向
                dir = (int)Utils.GetAngle(host.pos, host.target.pos);
                //  Debug.LogError("随机攻击范围内运动 " + dir);
            }
            host.dir = this.dir;
            return true;
        }
    }
}