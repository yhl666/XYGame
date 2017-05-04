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
            target.atk = true;
            return true;
        }
    }



}