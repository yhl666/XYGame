﻿/*
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
        //  this.OnEnter();

    }


    public override void OnEnter()
    {
        this.Enable = false;
        this.stack.AddLocalEventListener("jump");
        this.stack.AddLocalEventListener(Events.ID_BTN_JUMP);
        this.stack.id = StateStack.ID_JUMP;

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
        if (Target.isAttacking && Target.isStand) return;
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
    public void DisableMove()
    {
        moveable = false;
    }
    public void EnableMove()
    {
        moveable = true;
    }

    public bool moveable = true;

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
        }

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
            if (this.Enable == false)
            {
                EventDispatcher.ins.PostEvent(Events.ID_DIE, Target);
            }
        }

    }
    public override void OnExit()
    {

    }

}

public class AttackState_1 : StateBase
{//普通 攻击 连招1

    private int cd_attack = 0;
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(AttackState_1))
        {
            return this;
        }
        return null;
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

            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENTITY_BEFORE_ATTACK, this.Target);

            this.Enable = true;
            Target.isAttacking = true;
            Target.attackingAnimationName = this.GetAnimationName();
            //    BulletMgr.Create<Bullet2_0>(this.Target);
            cd_attack = 40;//2s cd

            BulletMgr.Create(this.Target, Target.bulleClassName_atk1, Target.bullet_atk1_info);
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENEITY_AFTER_ATTACK, this.Target);

        }
        else if (this.Enable == true && Events.ID_SPINE_COMPLETE == type)
        {
            Target.isAttacking = false;
            this.Enable = false;
            // Debug.Log(" 连招  1111 完成");
            if (Target.atk_level > 1)
            {//下段招数
                this.stack.PushSingleState(StateBase.Create<AttackState_2>(Target));
            }

        }




    }



    public override void OnEnter()
    {
        this.stack.AddLocalEventListener("attack");
        this.stack.AddLocalEventListener("SpineComplete");

        this.stack.AddLocalEventListener(Events.ID_SPINE_COMPLETE);

        this.stack.AddLocalEventListener(Events.ID_BTN_ATTACK);

        this.stack.id = StateStack.ID_ATTACK;

    }
    public override void OnExit()
    {

    }

}


public class AttackState_2 : StateBase
{//普通 攻击 连招2
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
        if (checkForTimeOut == false) return;
        tick++;
        if (tick > 60)
        {//time out
            tick = 0;
            this.Enable = false;
            this.stack.PopSingleState();
            //    Debug.Log(" 连招  222 超时");

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

        if (type == Events.ID_BTN_ATTACK)
        {
            if (isLaunch) return;
            isLaunch = true;
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENTITY_BEFORE_ATTACK, this.Target);

            Target.attackingAnimationName = this.GetAnimationName();
            ///   this.checkForTimeOut = false;
            this.Enable = true;
            Target.isAttacking = true;


            BulletMgr.Create(this.Target, Target.bulleClassName_atk2, Target.bullet_atk2_info);

            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENEITY_AFTER_ATTACK, this.Target);


        }
        else if (this.Enable == true && Events.ID_SPINE_COMPLETE == type)
        {
            //  Debug.Log("连招   222 完成");

            Target.isAttacking = false;
            this.Enable = false;
            this.stack.PopSingleState();
            if (Target.atk_level > 2)
            {//下段招数

                this.stack.PushSingleState(StateBase.Create<AttackState_3>(Target));
            }
        }
    }


    public override void OnEnter()
    {
        this.Enable = true;

        this.stack.id = StateStack.ID_ATTACK;
    }
    public override void OnExit()
    {

    }

}



public class AttackState_3 : StateBase
{//普通 攻击 连招3
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
        //   Debug.Log("连招   3333 待命");

    }
    public override void UpdateMS()
    {
        if (this.checkForTimeOut == false) return;
        tick++;
        if (tick > 60)
        {//time out
            tick = 0;
            this.Enable = false;
            this.stack.PopSingleState();
            //  Debug.Log("连招   333333 超时");
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

        if (type == Events.ID_BTN_ATTACK && checkForTimeOut)
        {
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENTITY_BEFORE_ATTACK, this.Target);

            Target.attackingAnimationName = this.GetAnimationName();
            this.Enable = true;
            Target.isAttacking = true;
            this.checkForTimeOut = false;


            BulletMgr.Create(this.Target, Target.bulleClassName_atk3, Target.bullet_atk3_info);
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENEITY_AFTER_ATTACK, this.Target);

        }
        else if (this.Enable == true && Events.ID_SPINE_COMPLETE == type)
        {
            Target.isAttacking = false;
            this.Enable = false;
            // Debug.Log("连招   33333 完成");
            this.stack.PopSingleState();


        }
    }


    public override void OnEnter()
    {
        this.Enable = true;
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
                    s.OnSpineCompolete();
                }
            }
        }
    }

    public override void OnEnter()
    {

        this.stack.AddLocalEventListener(Events.ID_LAUNCH_SKILL1);
        this.stack.AddLocalEventListener(Events.ID_SPINE_COMPLETE);
        this.stack.AddLocalEventListener("SpineComplete");

        {
            SkillStack s = SkillStack.Create();
            s.host = this.Target;
            s.parent = this;
            s.PushSingleSkill(new Skill6_1());
            this.skill_stacks.Add(s);
        }
        {
            SkillStack s = SkillStack.Create();
            s.host = this.Target;
            s.parent = this;
            s.PushSingleSkill(new Skill2_2());
            this.skill_stacks.Add(s);
        }



        {
            SkillStack s = SkillStack.Create();
            s.host = this.Target;
            s.parent = this;
            s.PushSingleSkill(new Skill2_3());
            this.skill_stacks.Add(s);
        }


        {
            SkillStack s = SkillStack.Create();
            s.host = this.Target;
            s.parent = this;
            s.PushSingleSkill(new Skill2_4());
            this.skill_stacks.Add(s);
        }



        this.Enable = true;

    }
    public override void OnExit()
    {

    }

    ArrayList skill_stacks = new ArrayList();


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