/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using AIEnemy;

public enum AIEnemyType
{
    FSM,//  普通仇恨机制 在AIEnemy.cs 里面 该机制只会仇恨行为
    Normal,// 该类型下 只会找当前最近的玩家攻击
    //  Boss,//boss 的AI类型
}
public class Enemy : Entity
{
    //--------------------------------接入通用  AI ，自定义可继承并且override  AI接口函数
    //TODO use behavior Tree to process AI
    public Entity target = null;
    public AIEnemyType ai_type = AIEnemyType.FSM;
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


        if (ai_type == AIEnemyType.Normal)
        {//该状态下回始终查找当前最近的玩家作为攻击目标
            this.AI_SearchNewTarget();
        }
        //如果目标非法，那么寻找另外一个目标
        if (target == null || target.IsInValid())
        {
            //   this.AI_SearchNewTarget();
            target = null;
            return;
        }
        if (isHurt) return;

        // 有目标 ，先判断是否在攻击范围内
        float dis = target.ClaculateDistance(this);
        ///   Debug.Log(dis + "      " + atk_range);
        if (dis < this.atk_range)
        {
            //攻击范围内
            dir = -1;

            if (cd.IsMax())
            {
                this.AI_AttackTarget();
                cd.Reset();
            }
            else
            {
                stand = true;
            }
        }
        else
        {
            //不在攻击范围内 移动向目标
            this.AI_MoveToTarget();
        }

    }
    public void AI_SearchNewTarget()
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
    public void AI_MoveToTarget()
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
        atk = true;
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
    public override void InitStateMachine()
    {
        //init state machine
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
        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<HurtState>(this));
        }
        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<SkillState>(this));
        }
    }
    // override 
    public override bool Init()
    {
        base.Init();
        ai_fsm_machine = Utils.Create<FSMMachine>();
        ai_fsm_machine.host = this;
        ai_fsm_machine.Init();


        this.current_hp = 3000;
        this.hp = 5000;

        ViewMgr.Create<ViewEnemy>(this);
        this.speed *= 0.5f;

        this.eventDispatcher.AddEventListener(this, Events.ID_LAUNCH_SKILL1);
        EventDispatcher.ins.AddEventListener(this, Events.ID_DIE);
        this.eventDispatcher.AddEventListener(this, "SpineComplete");
        return true;
    }
    public override void OnEvent(string type, object userData)
    {
        if ("SpineComplete" == type)
        {
            this.OnEvent(Events.ID_SPINE_COMPLETE, userData);
        }
    }
    public override void OnEvent(int type, object userData)
    {


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
                if (ani_die == "")
                {
                    this.SetInValid();
                    return;
                }
            }
            if (userData as Entity == target)
            {//目标死亡
                target = null;
                bullet_atk1_info.hit_targets.Clear();
                ai_fsm_machine.ChangeTo<Free>();
            }
        }
        else if (type == Events.ID_SPINE_COMPLETE && isDie)
        {
            this.bufferMgr.Clear();
            this.SetInValid();
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
        BulletConfigInfo info = BulletConfigInfo.Create();
        info.plistAnimation = "";
        info.distance = 0.2f;
        info.distance_atk = 1f;
        info.lastTime = 10;
        info.oneHitTimes = 0xfffff;
        info.isHitDestory = true;
        info.collider_size = new Vector3(2f, 2f, 2f);
        ///info.AddBuffer("BufferEnemyMovementAfterAtk");
        info._OnTakeAttack = (Bullet bbbb, object user) =>
        {
            TimerQueue.ins.AddTimerMSI(10, () =>
            {
                this.AddBuffer("BufferEnemyMovementAfterAtk");
            });
        };
        this.bullet_atk1_info = info;
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

public class Enemy1_Strengthen : Enemy1
{//TODO 添加 闪烁特效
    public override void InitStateMachine()
    {
        //init state machine
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
    }

    public override void InitInfo()
    {
        base.InitInfo();
        this.prefabsName = "Prefabs/Enemy221Flare";
        ani_hurt = "";
        scale = 1.3f;
        this.AddBuffer<BufferBaTi>();
    }

    public override bool Init()
    {
        base.Init();

        return true;
    }

    public override void AI_UpdateMSWithAI()
    {
        base.AI_UpdateMSWithAI();
    }
    public override void UpdateMS()
    {
        base.UpdateMS();
    }
}

/// <summary>
/// 小怪2
/// </summary>
public class Enemy2 : Enemy
{
    public override void InitInfo()
    {
        this.prefabsName = "Prefabs/Enemy221";
        this.skin = "baihu4";
        ani_hurt = "hurt";
        ani_run = "walk";
        ani_stand = "rest";
        ani_atk1 = "218020_0";
        attackingAnimationName = ani_atk1;
        atk_level = 1;
        bulleClassName_atk1 = "BulletConfig";//普通攻击 1段  的子弹名字
        bulleClassName_s1 = "Bullet221_0"; // 1 号技能 子弹名字
        BulletConfigInfo info = BulletConfigInfo.Create();
        /*  info.plistAnimation = "";
          info.distance = 0.2f;
          info.distance_atk = 1f;
          info.lastTime = 10;
          info.oneHitTimes = 0xfffff;
          info.isHitDestory = true;
          info.collider_size = new Vector3(2f, 2f, 2f);
          info._OnTakeAttack = (Bullet bbbb, object user) =>
          {
              TimerQueue.ins.AddTimerMSI(10, () =>
              {
                  this.AddBuffer("BufferEnemyMovementAfterAtk");
              });
          };*/

        info.frameDelay = 3;
        info.distance_atk = 1.5f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = "hd/magic_weapons/bullet/bul_500502/bul_500502.plist";

        //  info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6222/role_6_bul_6222.plist";
        /// info.rotate = 30.0f;
        info.distance = 2f;
        ///   info.lastTime = 10;
        info.scale_x = 2f;
        info.scale_y = 2f;

        info.launch_delta_xyz.x = 0.5f;// Skill62_3_Data.ins.delta_xyz.x;// 1.5f;
        info.launch_delta_xyz.y = -0.2f;// Skill62_3_Data.ins.delta_xyz.y;// -0.2f;
        info.launch_delta_xyz.z = 0f;// Skill62_3_Data.ins.delta_xyz.z;// -0.2f;
        info.isHitDestory = true;
        info.plistAnimation = Skill62_3_Data.ins.hit_animation_name;
        info.damage_ratio = Skill62_3_Data.ins.damage_ratio;
        info.collider_size = Skill62_3_Data.ins.hit_rect;
        info.collider_type = ColliderType.Box;
        info._OnTakeAttack = (Bullet bbbb, object user) =>
        {
            TimerQueue.ins.AddTimerMSI(10, () =>
            {
                ///   this.AddBuffer("BufferEnemyMovementAfterAtk");
            });
        };
        info._OnLaunch = (Bullet bbbb, object user) =>
        {
            info.dir_2d = Utils.GetAngle(this, target);
        };

        this.bullet_atk1_info = info;
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
    public override void AI_AttackTarget()
    {
        /*  BulletConfigInfo info = BulletConfigInfo.Create();

          // info.AddBuffer("BufferHitBack");

          // info.launch_delta_xy.x = 1.5f;
          //  info.launch_delta_xy.y = -0.2f;
          info.frameDelay = 3;
          info.distance_atk = 1.5f;
          info.number = 0xfff;
          info.isHitDestory = false;
          info.oneHitTimes = 1;
          //  info.rotate = -120.0f;
              info.plistAnimation = "hd/magic_weapons/bullet/bul_500502/bul_500502.plist";

          //  info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6222/role_6_bul_6222.plist";
          /// info.rotate = 30.0f;
          info.distance = 2f;
          ///   info.lastTime = 10;
          info.scale_x = 2f;
          info.scale_y = 2f;

          info.launch_delta_xyz.x = 0.5f;// Skill62_3_Data.ins.delta_xyz.x;// 1.5f;
          info.launch_delta_xyz.y = -0.2f;// Skill62_3_Data.ins.delta_xyz.y;// -0.2f;
          info.launch_delta_xyz.z = 0f;// Skill62_3_Data.ins.delta_xyz.z;// -0.2f;
          info.isHitDestory = true;
          info.plistAnimation = Skill62_3_Data.ins.hit_animation_name;
          info.damage_ratio = Skill62_3_Data.ins.damage_ratio;
          info.collider_size = Skill62_3_Data.ins.hit_rect;
          info.collider_type = ColliderType.Box;

          BulletMgr.Create(this, "BulletConfig", info);*/


        atk = true;
    }
}

public class Enemy2_Strengthen : Enemy2
{//TODO 添加 闪烁特效
    public override void InitStateMachine()
    {
        //init state machine
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
    }

    public override void InitInfo()
    {
        base.InitInfo();
        this.prefabsName = "Prefabs/Enemy221Flare";
        ani_hurt = "";
        scale = 1.3f;
        this.AddBuffer<BufferBaTi>();
    }

}

/// <summary>
/// 小怪3
/// </summary>
public class Enemy3 : Enemy
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
        BulletConfigInfo info = BulletConfigInfo.Create();
        info.plistAnimation = "";
        info.distance = 0.2f;
        info.distance_atk = 1f;
        info.lastTime = 10;
        info.oneHitTimes = 0xfffff;
        info.isHitDestory = true;
        info.collider_size = new Vector3(2f, 2f, 2f);
        info.AddBuffer("BufferPoison");
        info._OnTakeAttack = (Bullet bbbb, object user) =>
        {
            TimerQueue.ins.AddTimerMSI(10, () =>
                {
                    this.AddBuffer("BufferEnemyMovementAfterAtk");
                });
        };
        this.bullet_atk1_info = info;
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

/// <summary>
/// BOSS
/// </summary>
public class EnemyBoss : Enemy
{
    public override void InitInfo()
    {
        this.prefabsName = "Prefabs/Hero2";
        this.skin = "#1";
        ani_hurt = "hurt";
        ani_run = "run";
        ani_die = "";
        ani_stand = "rest";
        ani_atk1 = "218010";
        attackingAnimationName = ani_atk1;
        atk_level = 1;
        bulleClassName_atk1 = "BulletConfig";//普通攻击 1段  的子弹名字
        bulleClassName_s1 = "Bullet221_0"; // 1 号技能 子弹名字
        BulletConfigInfo info = BulletConfigInfo.Create();
        info.plistAnimation = "";
        info.distance = 0.2f;
        info.distance_atk = 1f;
        info.lastTime = 10;
        info.oneHitTimes = 0xfffff;
        info.isHitDestory = true;
        info.collider_size = new Vector3(2f, 2f, 2f);
        info.AddBuffer("BufferPoison");

        this.bullet_atk1_info = info;
        this.atk_range = 999f;
        scale = 0.8f;
    }


    public override bool Init()
    {
        type = "boss";
        base.Init();
        this.skill = machine.GetState<SkillState>() as SkillState;
        this.skill1 = skill.GetSkill<SkillBoss_1>();
        this.skill2 = skill.GetSkill<SkillBoss_2>();
        this.skill3 = skill.GetSkill<SkillBoss_3>();
        ai_fsm_machine.Pause();
        hp = 3000;

        current_hp = hp;
        return true;
    }

    public override void AI_UpdateMSWithAI()
    {
        if (target == null)
        {
            this.AI_SearchNewTarget();
        }
        ai_fsm_machine.UpdateMS();
        base.AI_UpdateMSWithAI();

        if (isAttacking == false)
        {
            run_random = true;
        }
        else
        {
            run_random = false;
        }
        if (run_random)
        {
            dir = dir_random;

        }
        if (tick_run_random.Tick() == false)
        {
            dir_random = Utils.random_frameMS.Next(0, 360);
            tick_run_random.Reset();
            tick_run_random.SetMax(Utils.random_frameMS.Next(10, 80));
        }
 
    }
    public override void UpdateMS()
    {
        if (levels.Count <= 0)
        {
            if (SkillBoss_3_Data.ins.call_times.Length > has_call.Length)
            {
                Debug.LogError("暂时只支持最大召唤波数为" + has_call.Length);
            }
            foreach (int lev in SkillBoss_3_Data.ins.call_times)
            {
                levels.Add(lev);
            }
            levels.Add(0); // 末尾填充0 方便计算
        }
   
        base.UpdateMS();
    }
    Counter tick_run_random = Counter.Create(40);// 随机走动下 多久切换一次 方向
    int dir_random = -1;
    public override void AI_AttackTarget()
    {//释放技能1
//         s1 = 1;
//        return;
        /*  s1 = 3;
          return;
          if (skill1.cd.IsMax())
          {
              s1 = 1;
          }
          else if (skill1.cd.IsMax())
          {
              s1 = 2;
          }
          else if (skill1.cd.IsMax())
          {
              s1 = 3;
          }
          else
          {

          }*/



        /*  if (skill3.cd.IsMax())
          {
              s1 = 3;
          }
          else */
        /*     if (skill2.cd.IsMax())
             {
                 s1 = 2;
             }
             else if (skill1.cd.IsMax())
             {
                 s1 = 1;
             }

             if (has3 == false && current_hp < hp * 0.3f)
             {
                 s1 = 3;
                 has3 = true;
             }
             if (has2 == false && current_hp < hp * 0.5f && current_hp > hp * 0.3f)
             {
                 s1 = 3;
                 has2 = true;
             }
             if (has1 == false && current_hp < hp * 0.7f && current_hp > hp * 0.5f)
             {
                 s1 = 3;
                 has1 = true;
             }



             if (s1 == 3)
             {
                 skill1.OnInterrupted(new SkillBase());
                 skill2.OnInterrupted(new SkillBase());
             }*/




        if (skill3.cd.IsMax())
        {
            int last = (int)levels[0];
            for (int i = 1; i < levels.Count; i++)
            {
                if (has_call[i - 1] == true)
                {
                    last = (int)levels[i];
                    continue;//该学段 已经召唤
                }
                int hp_current_percent = current_hp * 100 / hp;
                if (hp_current_percent <= last && hp_current_percent > (int)levels[i])
                {//血量在该范围内 并且没有召唤
                    s1 = 3;
                    skill3.level = i; // 开始召唤
                    has_call[i - 1] = true;
                    run_random = false;
                    return;
                    break;
                }
                last = (int)levels[i];
            }
        }
        if (skill2.cd.IsMax())
        {
            s1 = 2;
            run_random = false;
        }
        else if (skill1.cd.IsMax())
        {
            s1 = 1;
            run_random = false;
        }
        else
        {//所有技能为冷却完毕

            run_random = true;
        }

    }
    bool run_random = false;
    SkillState skill = null;
    SkillBoss_1 skill1 = null;
    SkillBoss_2 skill2 = null;
    SkillBoss_3 skill3 = null;

    bool[] has_call = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
    ArrayList levels = new ArrayList(); // 末尾会填充0 方便计算
    bool has1 = false;
    bool has2 = false;
    bool has3 = false;
    public int level_call = 0;//召唤等级 
}

