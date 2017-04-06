/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using AIEnemy;

public class Enemy : Entity
{
    //--------------------------------接入通用  AI ，自定义可继承并且override  AI接口函数
    //TODO use behavior Tree to process AI
    public Entity target = null;
    public float target_distance = 5f;//仇恨范围
    public Counter cd = Counter.Create(80);
    public FSMMachine ai_fsm_machine = null;
    public virtual void AI_UpdateMSWithAI()
    {
        /*//如果目标非法，那么寻找另外一个目标
        if (target == null || target.IsInValid())
        {
            this.AI_SearchNewTarget();
            return;
        }
        if (isHurt) return;

        // 有目标 ，先判断是否在攻击范围内
        float dis = target.ClaculateDistance(this);
        //     Debug.Log(dis);
        if (dis < this.atk_range)
        {
            //攻击范围内
            dir = -1;
            this.AI_AttackTarget();
        }
        else
        {
            //不在攻击范围内 移动向目标
            this.AI_MoveToTarget();
        }*/

        //如果目标非法，那么寻找另外一个目标
        if (target == null || target.IsInValid())
        {
            ///  this.AI_SearchNewTarget();
            target = null;
            return;
        }
        if (isHurt) return;

        // 有目标 ，先判断是否在攻击范围内
        float dis = target.ClaculateDistance(this);
        //     Debug.Log(dis);
        if (dis < this.atk_range)
        {
            //攻击范围内
            dir = -1;
            this.AI_AttackTarget();
        }
        else
        {
            //不在攻击范围内 移动向目标
            this.AI_MoveToTarget();
        }

    }
    public virtual void AI_SearchNewTarget()
    {
        ArrayList heros = HeroMgr.ins.GetHeros();
        float minDis = 9999.0f;

        foreach (Hero h in heros)
        {//找出一个最近的玩家 作为锁定目标
            float dis = h.ClaculateDistance(x, y);
            if (dis < minDis)
            {
                target = h;
                minDis = dis;
            }

        }
    }
    public virtual void AI_MoveToTarget()
    {
        /*  if (this.x < target.x)
          {
              //玩家在右方
              right = true;
              if (this.isInOneTerrainRight == true)
              {
                  if (this.GetRealY() < target.GetRealY())
                  {
                      this.jump = true;
                  }
              }
          }
          else
          {
              //玩家在左方
              left = true;
              if (this.isInOneTerrainRight == false)
              {
                  if (this.GetRealY() < target.GetRealY())
                  {
                      this.jump = true;
                  }
              }
          }
          */

        dir = (int)Utils.GetAngle(this.pos, target.pos);
    }
    public virtual void AI_AttackTarget()
    {
        if (cd.IsMax())
        {
            //   target.TakeAttack(this);
            ///  BulletMgr.Create(this, "Bullet444_0");
            atk = true;
            cd.Reset();
        }
        else
        {
            stand = true;
        }
    }

    public override void InitInfo()
    {
        this.prefabsName = "Prefabs/Enemy444";

        ani_hurt = "hurt";
        ani_run = "walk";
        ani_stand = "rest";
        ani_atk1 = "444000_0";
        attackingAnimationName = ani_atk1;

        bulleClassName_atk1 = "Bullet444_0";//普通攻击 1段  的子弹名字
        bulleClassName_s1 = "Bullet444_0"; // 1 号技能 子弹名字

        scale = 0.8f;
    }
    // override 
    public override bool Init()
    {
        base.Init();
        ai_fsm_machine = Utils.Create<FSMMachine>();
        ai_fsm_machine.host = this;
        ai_fsm_machine.Init();
        //init state machine
        /*
         {
             StateStack s = StateStack.Create();
             this.machine.AddParallelState(s);
             s.PushSingleState(StateBase.Create<AttackState_1>(this));
         }

         {
             StateStack s = StateStack.Create();
             this.machine.AddParallelState(s);
             s.PushSingleState(StateBase.Create<RunState>(this));

         }

         {
             StateStack s = StateStack.Create();
             this.machine.AddParallelState(s);
             s.PushSingleState(StateBase.Create<StandState>(this));
         }
         {
             StateStack s = StateStack.Create();
             this.machine.AddParallelState(s);
             s.PushSingleState(StateBase.Create<SkillState>(this));
         }
         {
             StateStack s = StateStack.Create();
             this.machine.AddParallelState(s);
             s.PushSingleState(StateBase.Create<HurtState>(this));
         }
         */

        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<DieState>(this));
        }

        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<AttackState_1>(this));
        }

        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<RunXZState>(this));

        }

        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<JumpState>(this));
        }

        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<FallState>(this));
        }

        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<StandState>(this));

        }
        /*  {
              StateStack s = StateStack.Create();
              this.machine.AddParallelState(s);
              s.PushSingleState(StateBase.Create<SkillState>(this));
          }*/
        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<HurtState>(this));
        }

        this.current_hp = 3000;
        this.hp = 5000;

        ViewMgr.Create<ViewEnemy>(this);
        this.speed *= 0.5f;

        this.eventDispatcher.AddEventListener(this, Events.ID_LAUNCH_SKILL1);
        EventDispatcher.ins.AddEventListener(this, Events.ID_DIE);
        return true;
    }

    public override void OnEvent(int type, object userData)
    {
        base.OnEvent(type, userData);

        if (type == Events.ID_LAUNCH_SKILL1)
        {
            //  this.AddBuffer(bufferMgr.Create<Buffer4>());

            //   Bullet b = BulletMgr.Create<Bullet2_1>(this);

        }
        else if (Events.ID_DIE == type)
        {
            if (userData == null) return;
            if (userData as Enemy == this)
            {
                this.machine.Pause();
                this.LazyDispose();
            }
            if (userData as Entity == target)
            {//目标死亡
                target = null;
                bullet_atk1_info.hit_targets.Clear();
                ai_fsm_machine.ChangeTo<Free>();
            }
        }
    }

    public override void UpdateMS()
    {
        cd.Tick();
        ///   this.AI_UpdateMSWithAI();
        ///   
        if (this.isDie)
        {
            this.machine.Pause(); return;
        }
        //process  input status
        if (atk)
        {
            eventDispatcher.PostEvent(Events.ID_BTN_ATTACK);
            atk = false;
        }
        if (left)
        {
            eventDispatcher.PostEvent(Events.ID_BTN_LEFT);
            left = false;
        }
        if (stand)
        {
            eventDispatcher.PostEvent(Events.ID_STAND);
            stand = false;
        }
        if (right)
        {
            eventDispatcher.PostEvent(Events.ID_BTN_RIGHT);
            right = false;
        }
        if (jump)
        {
            eventDispatcher.PostEvent(Events.ID_BTN_JUMP);
            jump = false;
        }
        if (s1 != 0)
        {
            eventDispatcher.PostEvent(Events.ID_LAUNCH_SKILL1, s1);
            s1 = 0;
        }

        base.UpdateMS();
    }

    //------------------------------------AI helper function
    public bool HasTowner()
    {
        return true;
    }
    public void SearchNewTower()
    {

    }
    /// <summary>
    /// 搜索最近的目标
    /// </summary>
    public bool AI_SearchNearestTarget(ArrayList list)
    {
        if (list.Count <= 0) return false;
        float minDis = float.MaxValue;

        foreach (Entity h in list)
        {//找出一个最近的玩家 作为锁定目标
            float dis = h.ClaculateDistance(x, y);
            if (dis < minDis)
            {
                target = h;
                minDis = dis;
            }
        }
        return true;
    }
}
public class Enemy221 : Enemy
{
    public override void InitInfo()
    {
        this.prefabsName = "Prefabs/Enemy221";
        this.skin = "baihu4";
        ani_hurt = "hurt";
        ani_run = "walk";
        ani_stand = "rest";
        ani_atk1 = "218010";
        attackingAnimationName = ani_atk1;

        bulleClassName_atk1 = "BulletConfig";//普通攻击 1段  的子弹名字
        bulleClassName_s1 = "Bullet221_0"; // 1 号技能 子弹名字
        this.bullet_atk1_info = BulletConfigInfo.Create();
        bullet_atk1_info.plistAnimation = "";
        bullet_atk1_info.distance = 0.2f;
        bullet_atk1_info.distance_atk = 1f;
        ///  bullet_atk1_info.AddHitTarget(BuildingMgr.ins.GetBuildings()[0] as Entity);
        //   this.speed *= 0.001f; ;
        ///  bullet_atk1_info.AddBuffer("BufferHitBack");
        //     bullet_atk1_info.AddBuffer("BufferSpin");

        this.atk_range = 0.3f;
        scale = 0.8f;
    }

}

/// <summary>
/// 小怪1
/// </summary>
public class Enemy1 : Enemy
{
    public override void InitInfo()
    {
        this.prefabsName = "Prefabs/Enemy221";
        this.skin = "baihu4";
        ani_hurt = "hurt";
        ani_run = "walk";
        ani_stand = "rest";
        ani_atk1 = "218010";
        attackingAnimationName = ani_atk1;
        atk_level = 1;
        bulleClassName_atk1 = "BulletConfig";//普通攻击 1段  的子弹名字
        bulleClassName_s1 = "Bullet221_0"; // 1 号技能 子弹名字
        this.bullet_atk1_info = BulletConfigInfo.Create();
        bullet_atk1_info.plistAnimation = "";
        bullet_atk1_info.distance = 0.2f;
        bullet_atk1_info.distance_atk = 1f;
        bullet_atk1_info.lastTime = 10;
        bullet_atk1_info.oneHitTimes = 0xfffff;
        bullet_atk1_info.isHitDestory = true;
        bullet_atk1_info.collider_size = new Vector3(2f, 2f, 2f);

        //   this.speed *= 0.001f; ;
        ///  bullet_atk1_info.AddBuffer("BufferHitBack");
        //     bullet_atk1_info.AddBuffer("BufferSpin");

        this.atk_range = 1.0f;
        scale = 0.8f;
    }


    public override bool Init()
    {
        base.Init();

        return true;
    }

    public override void AI_UpdateMSWithAI()
    {
        ai_fsm_machine.UpdateMS();
        base.AI_UpdateMSWithAI();
    }
    public override void UpdateMS()
    {
        base.UpdateMS();



    }
}