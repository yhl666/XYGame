/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using FSM;

public enum AIEnemyType
{
    FSM,//  普通仇恨机制 在AIEnemy.cs 里面 该机制只会仇恨行为
    Normal,// 该类型下 只会找当前最近的玩家攻击
    BehaviorTree, // 行为树
    //  Boss,//boss 的AI类型
}
public class Enemy : Entity
{
    //--------------------------------接入通用  AI ，自定义可继承并且override  AI接口函数
    //TODO use behavior Tree to process AI
    public Entity target = null;
    public AIEnemyType ai_type = AIEnemyType.FSM;
    public float target_distance = 4f;//仇恨范围
    public Counter cd = Counter.Create(80);
    public FSMMachine ai_fsm_machine = null;
    public bool is_hit_tower = false;//是否攻击了塔
    public bool IsHeroRandomAtk(float factor = 1.0f)
    {//随机化 攻击行为，减弱 攻击强度
        int number = target.GetTargetNo();
        if (number < 2)
        {
            return true;
        }
        else if (number >= 3 && number <= 5)
        {
            if (Utils.random_frameMS.Next(1, (int)(100 * factor)) == 2)
            {
                return true;
            }
        }
        else if (number >= 5 && number <= 10)
        {
            if (Utils.random_frameMS.Next(1, (int)(200 * factor)) == 2)
            {
                return true;
            }
        }
        else
        {
            if (Utils.random_frameMS.Next(1, number * (int)(200 * factor) / 5) == 2)
            {
                return true;
            }
        }
        return false;
    }
    private void RunRandom()
    {
        if (run_random == false)
        {
            run_random = true;
            dir1 = (int)Utils.GetAngle(this.pos, target.pos);
        }
        else
        {
            if (Utils.random_frameMS.Next(1, 200) == 2)
            {//一定概率 随机方向
                dir1 = (int)Utils.GetAngle(this.pos, target.pos);
            }
            dir = dir1;
        }
    }
    private void InitBehaviorTree()
    {
        bt_root = new BehaviorTree.Parallel();

        /*  // 版本1
            bt_root = new BehaviorTree.Parallel();
 {
             var bt_target = new BehaviorTree.Sequence();
             bt_target.AddChild(new BehaviorTree.Condition.NotTargetOrDie());
             bt_target.AddChild(new BehaviorTree.Action.SearchNearestTarget());
             bt_root.AddChild(bt_target);
         }
         {
             var bt_selector = new BehaviorTree.Selector();
             var bt_sequence1 = new BehaviorTree.Sequence();
             var bt_sequence2 = new BehaviorTree.Sequence();
             bt_selector.AddChild(bt_sequence1);
             bt_selector.AddChild(bt_sequence2);

             bt_sequence1.AddChild(new BehaviorTree.Condition.TargetHasNotInAtkRange());
             bt_sequence1.AddChild(new BehaviorTree.Action.MoveToTarget());

             bt_sequence2.AddChild(new BehaviorTree.Condition.IsCDMax());
             bt_sequence2.AddChild(new BehaviorTree.Action.AttackTarget());
             bt_root.AddChild(bt_selector);
         }
         */


        //版本2


        {// 仇恨行为
            var selector1 = new BehaviorTree.Selector();
            bt_root.AddChild(selector1);

            var sequence2 = new BehaviorTree.Sequence();
            selector1.AddChild(sequence2);
            sequence2.AddChild(new BehaviorTree.Condition.NotTargetOrDie());

            // selector4
            var selector4 = new BehaviorTree.Selector();
            sequence2.AddChild(selector4);
            var sequence7 = new BehaviorTree.Sequence();
            selector4.AddChild(sequence7);
            sequence7.AddChild(new BehaviorTree.Condition.HasTower());
            sequence7.AddChild(new BehaviorTree.Action.SearchNearestTower());
            selector4.AddChild(new BehaviorTree.Action.SearchNearestHero());

            //selector3
            var selector3 = new BehaviorTree.Selector();
            selector1.AddChild(selector3);
            var sequence5 = new BehaviorTree.Sequence();
            selector3.AddChild(sequence5);

            var sequence6 = new BehaviorTree.Sequence();
            selector3.AddChild(sequence6);
            sequence6.AddChild(new BehaviorTree.Condition.TargetIsHero());
            sequence6.AddChild(new BehaviorTree.Action.NotHitTower());

            sequence5.AddChild(new BehaviorTree.Condition.TargetIsTower());
            var selector8 = new BehaviorTree.Selector();
            sequence5.AddChild(selector8);

            var sequence9 = new BehaviorTree.Sequence();
            selector8.AddChild(sequence9);
            var sequence10 = new BehaviorTree.Sequence();
            selector8.AddChild(sequence10);

            sequence9.AddChild(new BehaviorTree.Condition.HasHitTower());
            sequence9.AddChild(new BehaviorTree.Condition.HasHitByHero());
            sequence9.AddChild(new BehaviorTree.Action.SetTarget());

            sequence10.AddChild(new BehaviorTree.Condition.HasNotHitTower());
            sequence10.AddChild(new BehaviorTree.Condition.HasHeroInTargetRange());
            sequence10.AddChild(new BehaviorTree.Action.SearchNearestHero());

        }

        {// 攻击 移动行为

            var selector1 = new BehaviorTree.Selector();
            bt_root.AddChild(selector1);
            var selector2 = new BehaviorTree.Selector();
            var selector3 = new BehaviorTree.Selector();

            selector1.AddChild(selector2);
            selector1.AddChild(selector3);

            //selector2
            var sequence4 = new BehaviorTree.Sequence();
            var sequence5 = new BehaviorTree.Sequence();
            selector2.AddChild(sequence4);
            selector2.AddChild(sequence5);
            sequence4.AddChild(new BehaviorTree.Condition.HasNotTarget());
            sequence4.AddChild(new BehaviorTree.Action.MoveRandom());
            sequence5.AddChild(new BehaviorTree.Condition.TargetHasNotInAtkRange());
            sequence5.AddChild(new BehaviorTree.Action.MoveToTarget());

            // selector 3
            var sequence6 = new BehaviorTree.Sequence();
            selector3.AddChild(sequence6);
            selector3.AddChild(new BehaviorTree.Action.MoveRandomInAtkRange());
            sequence6.AddChild(new BehaviorTree.Condition.IsCDMax());
            sequence6.AddChild(new BehaviorTree.Action.AttackTarget());
        }

        this.bullet_atk1_info._OnTakeAttack += (Bullet bullet, object userData) =>
       {
           //命中后  
           if (target != null && (userData as Entity) == target)
           {
               is_hit_tower = true;
           }
       };
    }
    public virtual void AI_UpdateMSWithAI()
    {


        bt_root.Visit(this);


        return;
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
            this.AI_SearchNearestTarget(HeroMgr.ins.GetHeros(), true);
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
        ///   
        if (this.IsEnemyBoss)
        {
            if (dis < this.atk_range)
            {
                //攻击范围内
                this.AI_AttackTarget();
            }
            else
            {
                //不在攻击范围内 移动向目标
                this.AI_MoveToTarget();

            }
            return;
        }
        if (dis < this.atk_range)
        {
            if (target.IsHero == false)
            {
                dir = -1;
                stand = true;
                run_random = false;
                if (cd.IsMax())
                {
                    atk = true;
                }
            }
            else if (target.IsHero && cd.IsMax())
            {   //调用 频率是 40HZ 
                atk = this.IsHeroRandomAtk();
                if (atk == false)
                {//未触发了攻击
                    this.RunRandom();
                }
            }
            else
            {//hero and cd 未到
                this.RunRandom();
            }
            if (atk)
            {
                cd.Reset();
                run_random = false;
            }
        }
        else
        {
            run_random = false;
            this.AI_MoveToTarget();
        }
        return;
        //原始 未处理攻击行为 版本
        if (dis < this.atk_range)
        {
            //攻击范围内
            this.AI_AttackTarget();
        }
        else
        {
            //不在攻击范围内 移动向目标
            this.AI_MoveToTarget();
        }
    }
    bool run_random = false;
    int dir1 = -1;
    /*  public void AI_SearchNewTarget()
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
      }*/
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
    private BehaviorTree.NodeBase bt_root = null;

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


        this.InitBehaviorTree();
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
    public bool AI_SearchNearestTarget(ArrayList list, bool enable_target_no) // 是否开启最大仇恨数限制
    {
        enable_target_no = true;
        if (list.Count <= 0) return false;
        float minDis = float.MaxValue;
        Entity target_bk = target;

        foreach (Entity h in list)
        {//找出一个最近的玩家 作为锁定目标
            if (enable_target_no && h.IsMaxTarget())
            {
                continue;
            }
            float dis = h.ClaculateDistance(x, y);
            if (dis < minDis)
            {
                target = h;
                minDis = dis;
            }
        }
        if (target_bk == target && target == null)
        {
            return false;
        }
        return true;
    }
}
public class Enemy221 : Enemy
{
    public override void InitInfo()
    {
        this.prefabsName = "Spine/Customs/213/213";
        this.skin = "213";
        ani_hurt = "hurt";
        ani_run = "walk";
        ani_stand = "rest";
        ani_atk1 = "210000";
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
        this.prefabsName = "Spine/Customs/213/213";
        this.skin = "213";
        ani_hurt = "hurt";
        ani_run = "walk";
        ani_stand = "rest";
        ani_atk1 = "210000";
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
        info._OnTakeAttack += (Bullet bbbb, object user) =>
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

    /*  public override void AI_UpdateMSWithAI()
      {
          ai_fsm_machine.UpdateMS();
          base.AI_UpdateMSWithAI();
      }*/
    /*  public override void UpdateMS()
      {
          base.UpdateMS();
      }*/

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
        this.prefabsName = "Spine/Customs/217/217";
        this.skin = "217";
        ani_run = "walk";
        ani_stand = "rest";
        ani_atk1 = "214000";
        ani_hurt = "";
        scale = 1.0f;
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
        this.prefabsName = "Spine/Customs/424/424";
        this.skin = "default";
        ani_hurt = "hurt";
        ani_run = "walk";
        ani_stand = "rest";
        ani_atk1 = "424000";
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
        info.plistAnimation = "hd/enemies/enemy_526/bullet/enemy_526_bul_526011/enemy_526_bul_526011.plist";

        //  info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6222/role_6_bul_6222.plist";
        /// info.rotate = 30.0f;
        info.distance = 4f;
        info.speed = 0.1f * 0.8f;
        ///   info.lastTime = 10;
        info.scale_x = 2f;
        info.scale_y = 2f;

        info.launch_delta_xyz.x = 0.5f;// Skill62_3_Data.ins.delta_xyz.x;// 1.5f;
        info.launch_delta_xyz.y = -0.2f;// Skill62_3_Data.ins.delta_xyz.y;// -0.2f;
        info.launch_delta_xyz.z = 0f;// Skill62_3_Data.ins.delta_xyz.z;// -0.2f;
        info.isHitDestory = true;
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
        this.atk_range = 8f;
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
        this.prefabsName = "Spine/Customs/306/306";
        this.skin = "default";
        ani_hurt = "";
        ani_run = "walk";
        ani_stand = "rest";
        ani_atk1 = "306000";
        scale = 1.0f;
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
        this.prefabsName = "Spine/Customs/444/444";
        this.skin = "default";
        ani_hurt = "hurt";
        ani_run = "walk";
        ani_stand = "rest";
        ani_atk1 = "444000_1";
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
    public bool enable_ai = true;
    public override void InitStateMachine()
    {
        //init state machine
        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<DieStateBoss>(this));
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
    public override void InitInfo()
    {
        this.prefabsName = "Prefabs/Hero2";
        this.skin = "#1";
        ani_hurt = "hurt";
        ani_run = "run";
        ani_die = "";
        ani_stand = "rest";
        // ani_atk1 = "218010";
        //   ani_atk1 = "218010";
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
        AudioMgr.ins.PostEvent(AudioEvents.Events.BATTLE_BOSS_BG, true);
        current_hp = hp;
        return true;
    }

    public override void AI_UpdateMSWithAI()
    {
        if (enable_ai == false) return;
        if (target == null)
        {
            this.AI_SearchNearestTarget(HeroMgr.ins.GetHeros(), false);
        }
        ai_fsm_machine.UpdateMS();
        base.AI_UpdateMSWithAI();
        if (isAttacking) return;
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
        if (isAttacking) return;
        if (skill3.cd.IsMax() && levels.Count > 0 && isAttacking == false)
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
        if (skill2.cd.IsMax() && isAttacking == false)
        {
            s1 = 2;
            run_random = false;
        }
        else if (skill1.cd.IsMax() && isAttacking == false)
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

