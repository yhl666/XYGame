/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

//状态

public class StateBase : GAObject
{
    protected ArrayList conflict_states = new ArrayList();


    public static StateBase Create<T>(Entity target) where T : new()
    {
        StateBase ret = new T() as StateBase;
        ret.Target = target;
        ret.Init();
        return ret;
    }

    public Entity Target = null; // reference
    public StateStack stack = null;//reference
    public virtual string GetName() { return "StateBase"; }
    public virtual string GetAnimationName() { return ""; }
    public bool Enable = false;
    public void SetDisable()
    {
        this.Enable = false;
    }
    public void SetEnable()
    {
        this.Enable = true;

    }
    public void Pause()
    {
        this.stack.Pause();
    }
    public void Resume()
    {
        this.stack.Resume();
    }
    public virtual StateBase GetState<T>() where T : new()
    {
        return null;
    }
    public virtual void UpdateMSIdle()
    {

    }
    /// <summary>
    /// 被打断事件
    /// </summary>
    public void OnInterrupted(GAObject what)
    {

    }

    public virtual void OnPause()
    {

    }
    public virtual void OnResume()
    {

    }
    /*
    public bool pause = false;
    public void Pause()
    {
        pause = true;
    }
    public void Resume()
    {
        pause = false;
    }*/
    //  public string animationName = "null";

}


public class JumpState : StateBase
{
    private float jump_speed = DATA.DEFAULT_JUMP_SPEED;
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(JumpState))
        {
            return this;
        }
        return null;
    }
    public override string GetName() { return "JumpState"; }
    public override string GetAnimationName() { return "jump"; }
    public override void UpdateMS()
    {
        if (jump_speed <= 0.0f)
        {
            Target.isJumping = false;
            if (Target.isStand)
            {
                jump_speed = DATA.DEFAULT_JUMP_SPEED;
                this.Enable = false;
            }
        }
        else
        {
            //接入重力

            jump_speed -= 9.8f / 40.0f * 0.05f;

            this.Target.y += jump_speed;
            Target.isJumping = true;
        }
    }
    public override void OnEvent(string type, object userData)
    {
        if (type == "jump")
        {
            this.OnEvent(Events.ID_BTN_JUMP, userData);
        }
    }

    public override void OnEvent(int type, object userData)
    {
        if (Target.isHurt) return;

        if (this.Enable == true && Target.isJumpTwice == false)
        {
            this.stack.PushSingleState(StateBase.Create<JumpTwiceState>(Target));
            this.Enable = false;
            Target.isJumping = false;
            return;
        }

        this.Enable = true;
        jump_speed = DATA.DEFAULT_JUMP_SPEED;
        //  this.OnEnter();

    }

    public override void OnEnter()
    {
        this.Enable = false;
        this.stack.AddLocalEventListener("jump");
        this.stack.AddLocalEventListener(Events.ID_BTN_JUMP);
        this.stack.id = StateStack.ID_JUMP;

    }
    public override void OnPause()
    {
        this.Enable = false;
        Target.isJumping = false;
    }
    public override void OnExit()
    {

    }
    public JumpState()
    {

    }
}

public class JumpTwiceState : StateBase
{
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(JumpTwiceState))
        {
            return this;
        }
        return null;
    }
    private float jump_speed = DATA.DEFAULT_JUMP_SPEED;
    public override string GetName() { return "JumpTwiceState"; }
    public override void UpdateMS()
    {
        if (jump_speed <= 0.0f)
        {
            Target.isJumping = false;

            if (Target.isStand)
            {
                this.Enable = false;
                jump_speed = DATA.DEFAULT_JUMP_SPEED;
                this.stack.PopSingleState();
            }
            else
            {
                Target.isJumpTwice = false;
            }
            return;
        }
        else
        {
            //接入重力
            jump_speed -= 9.8f / 40.0f * DATA.GRAVITY_RATIO;

            this.Target.y += jump_speed;
            Target.isJumpTwice = true;
        }

    }
    public override void OnPause()
    {

    }
    public override void OnEvent(string type, object userData)
    {

    }
    public override void OnEnter()
    {
        this.Enable = true;
        this.stack.id = StateStack.ID_JUMP;

    }
    public override void OnExit()
    {

    }
    public JumpTwiceState()
    {

    }
}

public class FallState : StateBase
{
    private float fall_speed = 0.0f;
    private int tick = 0;
    public float FallDistance = 0.0f;
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(FallState))
        {
            return this;
        }
        return null;
    }
    public override string GetName() { return "FallState"; }
    public override void UpdateMS()
    {
        Target.isFalling = false;
        if (Target.isJumping || Target.isJumpTwice)
        {
            tick = 0;
            fall_speed = 0.0f;
            return;
        }
        if (Target.isStand && Target.isRunning)
        {
            tick = 0;
            fall_speed = 0.0f;
            return;
        }
        if (Target.isStand == false)
        {
            //重力效果
            fall_speed = 9.8f / 40.0f * tick * DATA.GRAVITY_RATIO;

            tick++;

            this.Target.y -= fall_speed;
            FallDistance += fall_speed; // 下落位移
            if (Target.y <= 0.0f)
            {
                Target.y = 0.0f; // 修正位置
                fall_speed = 0.0f;
                tick = 0;
                FallDistance = 0.0f;
            }
            Target.isFalling = true;
        }

    }
    /*  public override void OnEvent(string type, object userData)
      {
          this.Enable = true;

      }
      public override void OnEvent(int type, object userData)
      {
          this.Enable = true;

      }*/
    public override void OnEnter()
    {
        this.Enable = true;
    }
    public override void OnPause()
    {

    }
    public override void OnExit()
    {

    }
    public FallState()
    {

    }
}


public class RunState : StateBase
{
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(RunState))
        {
            return this;
        }
        return null;
    }
    public override string GetName() { return "RunState"; }
    public RunState()
    {

    }
    public override void UpdateMS()
    {
        if (moveable == false) return;
        if (enable_atk == false)
        {
            if (Target.isAttacking && Target.isStand)
            {
                return;
            }
        }
        if (Target.isHurt) return;
        this.Target.x -= (Target.speed * this.Target.flipX);

        Target.isRunning = true;

    }
    public override void OnEvent(string type, object userData)
    {

    }

    public override void OnEvent(int type, object userData)
    {
        if (Target.isHurt) return;

        if (type == Events.ID_STAND)
        {
            this.Enable = false;
            Target.isRunning = false;
            if (Target.isFalling == true) return;
            if (Target.isJumping == true) return;

            return;
        }

        else if (type == Events.ID_BTN_LEFT)
        {
            Target.flipX = 1.0f; this.Enable = true;
        }
        else if (type == Events.ID_BTN_RIGHT)
        {
            Target.flipX = -1.0f; this.Enable = true;
        }

    }
    /// <summary>
    /// 关闭移动但可以 调整方向
    /// </summary>
    public void DisableMove()
    {
        moveable = false;
    }
    //开启移动
    public void EnableMove()
    {
        moveable = true;
    }
    /// <summary>
    /// 开启Attack状态下可移动
    /// </summary>
    public void EnableWhenAttack()
    {
        enable_atk = true;
    }
    public void DisableWhenAttak()
    {
        enable_atk = false;
    }
    public bool moveable = true;
    private bool enable_atk = false;
    public override void OnEnter()
    {
        this.stack.AddLocalEventListener("run_right");
        this.stack.AddLocalEventListener("run_left");
        this.stack.AddLocalEventListener("stand");

        this.stack.AddLocalEventListener(Events.ID_BTN_LEFT);
        this.stack.AddLocalEventListener(Events.ID_BTN_RIGHT);
        this.stack.AddLocalEventListener(Events.ID_STAND);

        this.stack.id = StateStack.ID_RUN;

    }
    public override void OnPause()
    {

    }
    public override void OnExit()
    {

    }

}

public class RunXZState : StateBase
{
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(RunXZState))
        {
            return this;
        }
        return null;
    }
    public override string GetName() { return "RunXZState"; }
    public RunXZState()
    {

    }

    public override void UpdateMS()
    {
        Target.isRunning = false;
        if (moveable == false) return;
        if (enable_atk == false)
        {
            if (Target.isAttacking && Target.isStand)
            {
                return;
            }
        }
        if (Target.isHurt) return;

        if (Target.dir == -1)
        {
            return;
        }

        //  if (Target.isFalling == true) return;
        ///    if (Target.isJumping == true) return;

        Target.isRunning = true;

        if (Target.dir > 90 && Target.dir < 270)
        { //left
            Target.flipX = 1.0f;
        }
        else
        {//right
            Target.flipX = -1.0f;
        }

        {
            ///   float  degree = Utils.GetDegree(new Vector2(Target.x, Target.y), new Vector2(x, y));
            //   float distance = Target.ClaculateDistance(x, y);
            //modify x and z  . y is jumping
            //  Debug.Log(" distance : " + distance);
            /*    if (distance < 0.06f || (x == Target.x && y == Target.z))
                {
                    this.Enable = false;
                    Target.isStand = true;
                    Target.isRunning = false;

                    return;
                }
                else
                {
                    Target.isStand = false;
                    Target.isRunning = true;
                }
                */
            float speed = Target.speed;

            float dd = Target.dir * DATA.ONE_DEGREE;//一度的弧度

            float z_delta = Mathf.Sin(dd);
            float x_delta = Mathf.Cos(dd);
            Target.z = Target.z + speed * z_delta;
            Target.x = Target.x + speed * x_delta;

        }

    }
    public override void OnEvent(string type, object userData)
    {

    }

    public override void OnEvent(int type, object userData)
    {

    }
    /// <summary>
    /// 关闭移动但可以 调整方向
    /// </summary>
    public void DisableMove()
    {
        moveable = false;
    }
    //开启移动
    public void EnableMove()
    {
        moveable = true;
    }
    /// <summary>
    /// 开启Attack状态下可移动
    /// </summary>
    public void EnableWhenAttack()
    {
        enable_atk = true;
    }
    public void DisableWhenAttak()
    {
        enable_atk = false;
    }
    public bool moveable = true;
    private bool enable_atk = false;
    public override void OnEnter()
    {
        this.stack.AddLocalEventListener("run_right");
        this.stack.AddLocalEventListener("run_left");
        this.stack.AddLocalEventListener("stand");

        this.stack.AddLocalEventListener(Events.ID_BTN_LEFT);
        this.stack.AddLocalEventListener(Events.ID_BTN_RIGHT);
        this.stack.AddLocalEventListener(Events.ID_STAND);

        this.stack.id = StateStack.ID_RUN;
        this.Enable = true;
    }
    public override void OnPause()
    {

    }
    public override void OnExit()
    {

    }

}


public class StandState : StateBase
{
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(StandState))
        {
            return this;
        }
        return null;
    }
    public override string GetName() { return "StandState"; }
    public StandState()
    {
    }
    public override void UpdateMS()
    {
        if (Target.y <= 0.0f)
        {//or other terrain ground
            Target.y = 0.0f;
            Target.isStand = true;
        }
        else
        {
            Target.isStand = false;
        }
    }
    public override void OnPause()
    {

    }
    public override void OnResume()
    {

    }
    public override void OnEnter()
    {

        this.Enable = true;

    }
    public override void OnEvent(string type, object userData)
    {

    }
    public override void OnExit()
    {

    }
}

public class HurtState : StateBase
{
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(HurtState))
        {
            return this;
        }
        return null;
    }
    public override string GetName() { return "HurtState"; }
    public HurtState()
    {

    }
    public override void UpdateMS()
    {

        tick++;
        if (tick > MAX_TICK)
        {
            this.Enable = false;
            tick = 0;
            Target.isHurt = false;
        }
    }
    public override void OnPause()
    {
        this.Enable = false;
        tick = 0;
        Target.isHurt = false;
    }
    public override void OnResume()
    {

    }
    public override void OnEnter()
    {

        //  this.Enable = true;
        this.stack.AddLocalEventListener(Events.ID_HURT);

    }
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_HURT)
        {
            if (Target.isHurt)
            {
                tick = 0;
            }
            else
            {
                this.Enable = true;
                Target.isHurt = true;
                tick = 0;
            }
        }
    }
    public override void OnExit()
    {

    }
    private int tick = 0;
    private int MAX_TICK = 10; // 0.5s
}


public class DieState : StateBase
{
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(DieState))
        {
            return this;
        }
        return null;
    }
    public override string GetName() { return "DieState"; }
    public DieState()
    {

    }
    public override void UpdateMS()
    {

        if (Target.current_hp <= 0)
        {
            this.Enable = false;
            Target.isDie = true;
            Target.bufferMgr.ClearClearAble();//清除所有Buffer
            EventDispatcher.ins.PostEvent(Events.ID_DIE, Target);
            if (Target != HeroMgr.ins.self as Entity && (Target as Enemy) == null)
            {
                EventDispatcher.ins.PostEvent(Events.ID_PUBLIC_PUSH_MSG, "玩家 " + Target.name + "死亡");
            }
        }
    }
    public override void OnPause()
    {

    }
    public override void OnResume()
    {

    }
    public override void OnEnter()
    {

        this.Enable = true;
        this.stack.AddLocalEventListener("SpineComplete");
    }
    public override void OnEvent(string type, object userData)
    {
        if (type == "SpineComplete")
        {
            if (this.Enable == false && Target.isDie)
            {
                EventDispatcher.ins.PostEvent(Events.ID_DIE, Target);
            }
        }
    }
    public override void OnExit()
    {

    }

}


public class DieStateBuilding : DieState
{
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(DieState))
        {
            return this;
        }
        return null;
    }
    public override string GetName() { return "DieState"; }
    public DieStateBuilding()
    {

    }
    public override void UpdateMS()
    {
        if (Target.current_hp <= 0)
        {
            this.Enable = false;
            Target.isDie = true;
            Target.bufferMgr.ClearClearAble();//清除所有Buffer
            EventDispatcher.ins.PostEvent(Events.ID_DIE, Target);
        }
    }
    public override void OnPause()
    {

    }
    public override void OnResume()
    {

    }
    public override void OnEnter()
    {
        this.Enable = true;
    }
    public override void OnEvent(string type, object userData)
    {
    }
    public override void OnExit()
    {

    }

}

public class AttackState_1 : StateBase
{//普通 攻击 连招1

    private int cd_attack = 0;
    Counter tick_cancel = Counter.Create(AttackState6_Data.ins.level1_cancel);//可取消tick
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(AttackState_1))
        {
            return this;
        }
        return null;
    }
    public override void OnPause()
    {
        if (this.Enable)
        {
            Target.isAttacking = false;
        }
        this.Enable = false;
    }
    public override void OnResume()
    {

    }
    public override string GetAnimationName() { return Target.ani_atk1; }

    public AttackState_1()
    {
        //   Debug.Log(" 连招  11111 待命");

    }
    public override void UpdateMS()
    {
        //    Target.isAttacking = true;
        cd_attack--;
        tick_cancel.Tick();
    }
    public override void UpdateMSIdle()
    {
        cd_attack--;
    }
    public override void OnEvent(string type, object userData)
    {
        if (type == "attack")
        {
            ///  this.Target.AddBuffer<BufferFlashMove>();
            this.OnEvent(Events.ID_BTN_ATTACK, userData);
        }
        else if ("SpineComplete" == type)
        {
            this.OnEvent(Events.ID_SPINE_COMPLETE, userData);
        }
    }

    public override void OnEvent(int type, object userData)
    {
        if (Target.isHurt) return;
        if (type == Events.ID_BTN_ATTACK && this.Enable == false)
        {
            ///  this.Target.AddBuffer<BufferFlashMove>();
            if (cd_attack > 0) return;
            tick_cancel.Reset();
            tick_cancel.SetMax(AttackState6_Data.ins.level1_cancel);
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENTITY_BEFORE_ATTACK, this.Target);
            this.Enable = true;
            Target.isAttacking = true;
            Target.attackingAnimationName = this.GetAnimationName();
            //    BulletMgr.Create<Bullet2_0>(this.Target);
            cd_attack = AttackState6_Data.ins.level1_cd;//2s cd

            BulletMgr.Create(this.Target, Target.bulleClassName_atk1, Target.bullet_atk1_info);
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENEITY_AFTER_ATTACK, this.Target);

        }
        else if ((this.Enable == true && Events.ID_SPINE_COMPLETE == type) || (tick_cancel.IsMax() && this.Enable == true && type == Events.ID_BTN_ATTACK))
        {
            Target.isAttacking = false;
            this.Enable = false;
        //    Debug.Log(" 连招  1111 完成  ");

            if (Target.atk_level > 1)
            {//下段招数
                this.stack.PushSingleState(StateBase.Create<AttackState_2>(Target));
            }

        }
        else if (this.Enable == true && Events.ID_BATTLE_PUSH_ONINTERRUPT_ATTACKSTATE == type)
        {//请求打断
            SkillBase skill = userData as SkillBase;
            if (skill.Target != this.Target) return;
            Target.isAttacking = false;
            this.Enable = false;
        }
    }

    public override void OnEnter()
    {
        this.stack.AddLocalEventListener("attack");
        this.stack.AddLocalEventListener("SpineComplete");

        this.stack.AddLocalEventListener(Events.ID_SPINE_COMPLETE);

        this.stack.AddLocalEventListener(Events.ID_BTN_ATTACK);
        this.stack.AddEventListener(Events.ID_BATTLE_PUSH_ONINTERRUPT_ATTACKSTATE);

        this.stack.id = StateStack.ID_ATTACK;

    }
    public override void OnExit()
    {

    }

}


public class AttackState_2 : StateBase
{//普通 攻击 连招2
    Counter tick_cancel = Counter.Create(AttackState6_Data.ins.level2_cancel);//可取消tick
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(AttackState_2))
        {
            return this;
        }
        return null;
    }

    public override string GetAnimationName() { return Target.ani_atk2; }

    bool checkForTimeOut = true;
    int tick = 0;
    private bool isLaunch = false;
    public AttackState_2()
    {
      //  Debug.Log(" 连招  222 待命");
    }
    public override void UpdateMS()
    {
        tick_cancel.Tick();
        if (checkForTimeOut == false) return;
        tick++;
        if (tick > AttackState6_Data.ins.level2_timeout)
        {//time out
            tick = 0;
            this.Enable = false;
            this.stack.PopSingleState();
           // Debug.Log(" 连招  222 超时");

        }

    }
    public override void OnEvent(string type, object userData)
    {
        if (type == "attack")
        {
            this.OnEvent(Events.ID_BTN_ATTACK, userData);
        }
        else if ("SpineComplete" == type)
        {
            this.OnEvent(Events.ID_SPINE_COMPLETE, userData);
        }
    }

    public override void OnEvent(int type, object userData)
    {
        if (Target.isHurt) return;

        if (type == Events.ID_BTN_ATTACK && this.Enable==false)
        {
            if (isLaunch) return;
            isLaunch = true;
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENTITY_BEFORE_ATTACK, this.Target);

            Target.attackingAnimationName = this.GetAnimationName();
            ///   this.checkForTimeOut = false;
            this.Enable = true;
            Target.isAttacking = true;
            tick_cancel.Reset();
            tick_cancel.SetMax(AttackState6_Data.ins.level2_cancel);
            BulletMgr.Create(this.Target, Target.bulleClassName_atk2, Target.bullet_atk2_info);

            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENEITY_AFTER_ATTACK, this.Target);

        }
        else if ((this.Enable == true && Events.ID_SPINE_COMPLETE == type) || (tick_cancel.IsMax() && this.Enable == true && type == Events.ID_BTN_ATTACK))
        {
          ///  Debug.Log("连招   222 完成");

            Target.isAttacking = false;
            this.Enable = false;
            this.stack.PopSingleState();
            if (Target.atk_level > 2)
            {//下段招数

                this.stack.PushSingleState(StateBase.Create<AttackState_3>(Target));
            }
        }
        else if (this.Enable == true && Events.ID_BATTLE_PUSH_ONINTERRUPT_ATTACKSTATE == type)
        {//请求打断
            SkillBase skill = userData as SkillBase;
            if (skill.Target != this.Target) return;
            Target.isAttacking = false;
            this.Enable = false;
            this.stack.PopSingleState();
        }
    }

    public override void OnPause()
    {
        if (this.Enable)
        {
            this.stack.PopSingleState();
            Target.isAttacking = false;
        }

        this.Enable = false;
    }
    public override void OnResume()
    {

    }
    public override void OnEnter()
    {
        this.Enable = false;
        tick_cancel.Reset();
        this.stack.id = StateStack.ID_ATTACK;
    }
    public override void OnExit()
    {

    }

}



public class AttackState_3 : StateBase
{//普通 攻击 连招3
    Counter tick_cancel = Counter.Create(AttackState6_Data.ins.level3_cancel);//可取消tick
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(AttackState_3))
        {
            return this;
        }
        return null;
    }
    public override string GetAnimationName() { return Target.ani_atk3; }

    bool checkForTimeOut = true;
    int tick = 0;
    public AttackState_3()
    {
     ///   Debug.Log("连招   3333 待命");

    }
    public override void UpdateMS()
    {
        tick_cancel.Tick();
        if (this.checkForTimeOut == false) return;
        tick++;
        if (tick > AttackState6_Data.ins.level3_timeout)
        {//time out
            tick = 0;
            this.Enable = false;
            this.stack.PopSingleState();
         ///   Debug.Log("连招   333333 超时");
        }

    }
    public override void OnEvent(string type, object userData)
    {
        if (type == "attack")
        {
            this.OnEvent(Events.ID_BTN_ATTACK, userData);
        }
        else if ("SpineComplete" == type)
        {
            this.OnEvent(Events.ID_SPINE_COMPLETE, userData);
        }
    }
    public override void OnEvent(int type, object userData)
    {
        if (Target.isHurt) return;
        if ((this.Enable == true && Events.ID_SPINE_COMPLETE == type) || (tick_cancel.IsMax() && this.Enable == true && type == Events.ID_BTN_ATTACK))
        {
            Target.isAttacking = false;
            this.Enable = false;
         ///   Debug.Log("连招   33333 完成");
            this.stack.PopSingleState();

        }
        else if (type == Events.ID_BTN_ATTACK && checkForTimeOut && this.Enable == false)
        {
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENTITY_BEFORE_ATTACK, this.Target);

            Target.attackingAnimationName = this.GetAnimationName();
            this.Enable = true;
            Target.isAttacking = true;
            this.checkForTimeOut = false;
            tick_cancel.Reset();
            tick_cancel.SetMax(AttackState6_Data.ins.level3_cancel);
            BulletMgr.Create(this.Target, Target.bulleClassName_atk3, Target.bullet_atk3_info);
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENEITY_AFTER_ATTACK, this.Target);

        }

        else if (this.Enable == true && Events.ID_BATTLE_PUSH_ONINTERRUPT_ATTACKSTATE == type)
        {//请求打断
            SkillBase skill = userData as SkillBase;
            if (skill.Target != this.Target) return;
            Target.isAttacking = false;
            this.Enable = false;
            this.stack.PopSingleState();
        }
    }
    public override void OnPause()
    {
        if (this.Enable)
        {
            this.stack.PopSingleState();
            Target.isAttacking = false;
        }

        this.Enable = false;
    }
    public override void OnResume()
    {

    }
    public override void OnEnter()
    {
        this.Enable = false;
        tick_cancel.Reset();
    }
    public override void OnExit()
    {

    }

}

public class SkillState111 : StateBase
{
    // 释放技能   share state isAttacking
    //读取配置信息 初始化各个SkillBase 和Stack
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(SkillState))
        {
            return this;
        }
        return null;
    }
    public override string GetAnimationName() { return "2110"; }
    /*  public SkillState()
      {

      }*/
    public override void UpdateMS()
    {

    }
    public override void OnEvent(string type, object userData)
    {
        if (type == "skill")
        {
            this.OnEvent(Events.ID_BTN_ATTACK, userData);
        }
        else if ("SpineComplete" == type)
        {
            this.OnEvent(Events.ID_SPINE_COMPLETE, userData);
        }
    }

    public override void OnEvent(int type, object userData)
    {
        if (Target.isHurt) return;

        if (type == Events.ID_LAUNCH_SKILL1 && this.Enable == false)
        {
            this.Enable = true;
            Target.isAttacking = true;
            Target.attackingAnimationName = this.GetAnimationName();
            BulletMgr.Create(this.Target, Target.bulleClassName_s1);

        }
        else if (this.Enable == true && Events.ID_SPINE_COMPLETE == type)
        {
            Target.isAttacking = false;
            this.Enable = false;
        }

    }

    public override void OnEnter()
    {
        // add event 
        this.stack.AddLocalEventListener(Events.ID_LAUNCH_SKILL1);
        this.stack.AddLocalEventListener(Events.ID_SPINE_COMPLETE);
        this.stack.AddLocalEventListener("SpineComplete");

    }
    public override void OnExit()
    {

    }

    ArrayList skill_stacks = new ArrayList();

}


public class SkillState : StateBase
{
    // 释放技能   share state isAttacking
    //读取配置信息 初始化各个SkillBase 和Stack
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(SkillState))
        {
            return this;
        }
        return null;
    }
    public override string GetAnimationName() { return ""; }
    public SkillState()
    {

    }
    public override void UpdateMS()
    {
        foreach (SkillStack s in skill_stacks)
        {
            s.UpdateMS();
        }
    }
    public override void OnEvent(string type, object userData)
    {
        if (type == "skill")
        {
            this.OnEvent(Events.ID_BTN_ATTACK, userData);
        }
        else if ("SpineComplete" == type)
        {
            this.OnEvent(Events.ID_SPINE_COMPLETE, userData);
        }
    }

    public void PushLevelUp(int idx)
    {
        if (idx > skill_stacks.Count) return;
        (skill_stacks[idx - 1] as SkillStack).PushLevelUp();
    }
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_LAUNCH_SKILL1 && this.Enable == true)
        {
            int idx = (int)userData;
            if (idx > skill_stacks.Count) return;

            (skill_stacks[idx - 1] as SkillStack).OnPush();

        }
        else if (this.Enable == true && Events.ID_SPINE_COMPLETE == type)
        {
            if (Target.isAttacking)
            {
                foreach (SkillStack s in skill_stacks)
                {
                    s.OnSpineComplete();
                }
            }
        }
        //else if (type==Events.ID_SKILL_LEVEL_UP&&this.Enable==true)
        //{
        //    //Debug.LogError("收到升级消息");
        //    //int idx = (int)userData;
        //    //
            
            
        //}
    }
    public void PushOnInterrupted(SkillBase who)
    {

        foreach (SkillStack s in skill_stacks)
        {
            if (s.ProcessOnInterrupted(who))
            {//只允许打断一个技能
                return;
            }
        }
    }
    public void PushOnInterruptedForce(SkillBase who)
    {
        bool has_one_false = false; // 存在强制打断失败否
        foreach (SkillStack s in skill_stacks)
        {
            if (s.ProcessOnInterruptedForce(who) == false)
            {
                has_one_false = true;
            }
        }
    }
    /// <summary>
    /// 请求切换技能组
    /// </summary>
    /// <param name="who"></param>
    public void PushChangeSkillGroup(SkillBase who)
    {
        foreach (SkillStack s in skill_stacks)
        {
            s.ProcessChangeSkillGroupOut(who);
        }
        if (skill_stacks == skill_stacks1)
        {
            skill_stacks = skill_stacks2;
        }
        else if (skill_stacks == skill_stacks2)
        {
            skill_stacks = skill_stacks1;
        }
        else
        {
            Debug.LogError("Unknow skill group");
        }
        foreach (SkillStack s in skill_stacks)
        {
            s.ProcessChangeSkillGroupIn(who);
        }
    }
    public override void OnEnter()
    {
        this.stack.AddLocalEventListener(Events.ID_LAUNCH_SKILL1);
        this.stack.AddLocalEventListener(Events.ID_SPINE_COMPLETE);
        this.stack.AddLocalEventListener(Events.ID_SKILL_LEVEL_UP);
        this.stack.AddLocalEventListener("SpineComplete");

        ///---------------------------------技能组 2
        string type = Target.type;
        ArrayList skills = ConfigTables.Hero.GetSkillsList(type);

        for (int i = 0; i < skills.Count / 2; i++)
        {
            SkillStack s = SkillStack.Create();
            s.host = this.Target;
            s.parent = this;
            s.PushSingleSkill(GAObject.Create<SkillBase>(skills[i] as string));
            this.skill_stacks2.Add(s);
        }


        ///---------------------------------技能组 1
        for (int i = skills.Count / 2; i < skills.Count; i++)
        {
            SkillStack s = SkillStack.Create();
            s.host = this.Target;
            s.parent = this;
            s.PushSingleSkill(GAObject.Create<SkillBase>(skills[i] as string));
            this.skill_stacks1.Add(s);
        }

        this.Enable = true;
        skill_stacks = skill_stacks2; // 默认技能组2
    }
    public override void OnExit()
    {

    }
    /// <summary>
    /// 查找技能
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="all">是否包含全部技能组，默认只在当前技能组查找</param>
    /// <returns></returns>
    public T GetSkill<T>(bool all = false) where T : SkillBase, new()
    {
        if (all)
        {
            SkillBase t = Find(ref skill_stacks1, typeof(T).ToString());
            if (t == null)
            {
                return Find(ref skill_stacks2, typeof(T).ToString()) as T;
            }
        }
        return Find(ref skill_stacks, typeof(T).ToString()) as T;
    }

    public bool AreLevelUpAbleByIndex(int idx)
    {
        if( (skill_stacks[idx - 1] as SkillStack).TopSkill().level<3)
            return true;
        return false;
    }
    private SkillBase Find(ref ArrayList list, string name)
    {
        foreach (SkillStack stack in list)
        {
            if (stack.TopSkill().GetName() == name)
            {
                return stack.TopSkill();
            }
        }
        return null;
    }
    
    ArrayList skill_stacks1 = new ArrayList();
    ArrayList skill_stacks2 = new ArrayList();
    ArrayList skill_stacks = null;

}


/// <summary>
/// 主场景点击操作 状态
/// </summary>
public class RunXYState : StateBase
{
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(RunXYState))
        {
            return this;
        }
        return null;
    }
    public override string GetName() { return "RunXYState"; }
    public RunXYState()
    {

    }
    public override void UpdateMS()
    {

        float distance = Target.ClaculateDistance(x, y);

        //  Debug.Log(" distance : " + distance);
        if (distance < 0.06f || (x == Target.x && y == Target.y))
        {
            this.Enable = false;
            Target.isStand = true;
            Target.isRunning = false;

            return;
        }
        else
        {
            Target.isStand = false;
            Target.isRunning = true;
        }

        float speed = Target.speed;

        float dd = degree * DATA.ONE_DEGREE;//一度的弧度

        float y_delta = Mathf.Sin(dd);
        float x_delta = Mathf.Cos(dd);
        Target.y = Target.y + speed * y_delta;
        Target.x = Target.x + speed * x_delta;
    }


    public override void OnEvent(int type, object userData)
    {

        this.Enable = true;
        Vector2 pos = (Vector2)userData;

        this.x = pos.x;
        this.y = pos.y;
        if (x > Target.x)
        {//面向右边
            Target.flipX = -1.0f;
        }
        else
        {//面向左边
            Target.flipX = 1.0f;
        }

        if (this.y >= 2.4f) this.y = 2.4f;

        degree = Utils.GetDegree(new Vector2(Target.x, Target.y), new Vector2(x, y));

    }
    public override void OnEnter()
    {
        this.stack.AddLocalEventListener(Events.ID_LOGIC_NEW_POSITION);
        Target.isRunning = false;
        Target.isStand = true;

    }
    public override void OnExit()
    {

    }
    float x;
    float y;
    float degree;
}
public class LuaInterfaceState : StateBase
{
    // lua 超类 状态接口，每个接口函数 配置对应的函数
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(LuaInterfaceState))
        {
            return this;
        }
        return null;
    }

    public int tick = 0;

}