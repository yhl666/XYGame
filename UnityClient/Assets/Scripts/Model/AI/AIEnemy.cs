﻿/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com
using FSM to peocess lite AI
 */
using UnityEngine;
using System.Collections;

namespace AIEnemy
{
    public class FSMBase : GAObject
    {
        public Enemy host = null;
        public FSMMachine machine = null;
        public override void UpdateMS()
        {

        }
        public override void OnEnter()
        {

        }
        public override void OnExit()
        {

        }
        public override bool Init()
        {
            base.Init();
            return true;
        }
        //  ------ helper function
        public void ChangeTo<T>() where T : FSMBase, new()
        {
            machine.ChangeTo<T>();
        }
    }
    /// <summary>
    /// 锁定塔状态 
    /// 该状态下只会攻击塔
    /// </summary>
    public class LockTower : FSMBase
    {
        public override void UpdateMS()
        {
            if (host.target == null)
            {
                this.ChangeTo<Free>();
            }
        }
        public override void OnEnter()
        {

        }
        public override void OnExit()
        {

        }
    }

    /// <summary>
    /// 自由状态 
    /// 目标死亡 或者 刚出生 才会在该状态下
    /// </summary>
    public class Free : FSMBase
    {
        public override void UpdateMS()
        {
            if (BuildingMgr.ins.GetBuildingsCount() > 0)
            { //存在塔
                ArrayList list = HeroMgr.ins.GetHeros();
                bool has_hero = false;
                if (list.Count > 0)
                {
                    foreach (Hero h in list)
                    {
                        if (host.target_distance > h.ClaculateDistance(host))
                        {//范围内 有玩家
                            has_hero = true;
                            break;
                        }
                    }
                }
                if (has_hero) // 仇恨范围内有玩家
                {//锁定玩家攻击
                    if (host.AI_SearchNearestTarget(list)) ;
                    {
                        this.ChangeTo<LockHero>();
                    }
                }
                else // 仇恨范围内没有玩家，直接进入半锁定塔状态
                {
                    this.ChangeTo<LockHalfTower>();
                }
            }
            else
            {//不存在塔，直接锁定最近玩家攻击
                if (host.AI_SearchNearestTarget(HeroMgr.ins.GetHeros())) ;
                {
                    this.ChangeTo<LockHero>();
                }
            }
        }
        public override void OnEnter()
        {

        }
        public override void OnExit()
        {

        }
    }

    /// <summary>
    /// 半锁定塔，
    /// 该状态 会尝试攻击塔 在第一次 攻击命中前 玩家攻击他的话 会切换到攻击玩家
    /// 如果第一次命中后  会进入锁定攻击塔 状态
    /// </summary>
    public class LockHalfTower : FSMBase
    {
        public override void UpdateMS()
        {

            if (has_find)//已找到塔 CD冷却
            {//开始攻击指令
                if (has_shoot == false)
                {
                    if (host.target == null || host == null)
                    {
                        ChangeTo<Free>();
                        return;
                    }
                    this.Shoot();
                }
            }
            else
            {
                if (host.AI_SearchNearestTarget(BuildingMgr.ins.GetBuildings()))//寻找最近的建筑作为目标
                {//找到 目标 尝试攻击
                    has_find = true;
                }
                else
                {//搜索失败 ，进入Free
                    machine.ChangeTo<Free>();
                }
            }
        }
        public override void OnEnter()
        {
            has_shoot = false;
            has_find = false;
        }
        public override void OnDispose()
        {
            EventDispatcher.ins.RemoveEventListener(this, Events.ID_BATTLE_ENTITY_BEFORE_TAKEATTACKED);
        }
        public override void OnExit()
        {
            if (info_backup != null)
            {
                host.bullet_atk1_info = info_backup;
            }
        }
        private void Shoot()
        {
            info_backup = host.bullet_atk1_info;
            //      host.atk = true;
            info_backup.AddHitTarget(host.target);
            BulletConfigInfo info = BulletConfigInfo.Create();
            info.plistAnimation = "";
            info.distance = 0.2f;
            info.distance_atk = 1f;
            info.number = 1;
            info.collider_size = new Vector3(2f, 2f, 2f);
            info.AddHitTarget(host.target);
            info._OnTakeAttack = (Bullet bullet, object userData) =>
            {
                //命中后 切换为锁定塔 攻击
               /// machine.ChangeTo<LockTower>();
            };
            has_shoot = true;
            host.bullet_atk1_info = info;
        }
        public override void OnEvent(int type, object userData)
        {
            if (this.IsInValid()) return;
            if (type == Events.ID_BATTLE_ENTITY_BEFORE_TAKEATTACKED)
            {
                AttackInfo info = userData as AttackInfo;
                if (info.target as Enemy == host && has_shoot)
                {
                    //自己被命中
                    host.target = info.ownner;
                    machine.ChangeTo<LockHero>();
                }
            }
        }
        public override bool Init()
        {
            base.Init();

            EventDispatcher.ins.AddEventListener(this, Events.ID_BATTLE_ENTITY_BEFORE_TAKEATTACKED);
            return true;
        }
        private BulletConfigInfo info_backup = null;
        private bool has_shoot = false;
        private bool has_find = false;

    }

    /// <summary>
    /// 锁定玩家状态
    /// 该状态只会攻击玩家 直到 玩家死亡
    /// </summary>
    public class LockHero : FSMBase
    {
        public override void UpdateMS()
        {
            if (host.target == null)
            {
                this.ChangeTo<Free>();
            }
        }
        public override void OnEnter()
        {

        }
        public override void OnExit()
        {

        }
    }

    /// <summary>
    /// FSM  状态机
    /// </summary>
    public class FSMMachine : GAObject
    {
        public override void UpdateMS()
        {
            if (pause) return;
            if (current_fsm != null)
            {
                current_fsm.UpdateMS();
                //    Debug.Log("FSM: " + current_fsm.GetType().ToString());
            }
        }
        public override void OnEnter()
        {

        }
        public override void OnExit()
        {

        }
        public override bool Init()
        {
            base.Init();
            current_fsm = Utils.Create<Free>();
            current_fsm.machine = this;
            current_fsm.host = host;
            current_fsm.Init();
            return true;
        }
        public void ChangeTo(FSMBase fsm)
        {
            if (current_fsm != null)
            {
                current_fsm.OnExit();
                current_fsm.LazyDispose();
            }
            fsm.host = host;
            fsm.machine = this;
            fsm.OnEnter();
            current_fsm = fsm;
        }
        public void ChangeTo<T>() where T : FSMBase, new()
        {
            FSMBase fsm = Utils.Create<T>();
            fsm.Init();
            this.ChangeTo(fsm);
        }
        public void Pause()
        {
            pause = true;
        }
        public void Resume()
        {
            pause = false;
        }
        public bool IsPause()
        {
            return pause;
        }
        private bool pause = false;
        FSMBase current_fsm = null;
        public Enemy host = null;
    }
}
