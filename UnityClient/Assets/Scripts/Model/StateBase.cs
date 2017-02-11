using UnityEngine;
using System.Collections;

//状态

public class StateBase : GAObject
{

    public static StateBase Create<T>(Entity target) where T : new()
    {
        StateBase ret = new T() as StateBase;
        ret.Target = target;
        ret.Init();
        return ret;
    }

    public Entity Target = null; // reference
    public StateStack stack = null;//reference
    public virtual string GetName() { return "State"; }
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
    public virtual StateBase GetState<T>() where T : new()
    {
        return null;
    }
    public virtual void UpdateMSIdle()
    {

    }
    //  public string animationName = "null";

}


public class JumpState : StateBase
{
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(JumpState))
        {
            return this;
        }
        return null;
    }

    public override string GetAnimationName() { return "jump"; }
    public override void UpdateMS()
    {
        if (tick > 30)
        {
            Target.isJumping = false;
            if (Target.isStand)
            {
                tick = 0;
                this.Enable = false;
            }
        }
        else
        {
            this.Target.y += 0.1f;
            Target.isJumping = true;
            tick++;
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
    private int tick = 0;
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

    public override void UpdateMS()
    {

        if (tick > 30)
        {
            if (Target.isStand)
            {
                this.Enable = false;
                tick = 0;
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
            this.Target.y += 0.1f;
            Target.isJumpTwice = true;
            tick++;
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
    private int tick = 0;
}






public class FallState : StateBase
{
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(FallState))
        {
            return this;
        }
        return null;
    }

    public override void UpdateMS()
    {
        Target.isFalling = false;
        if (Target.isJumping || Target.isJumpTwice)
        {
            return;
        }
        if (Target.isStand && Target.isRunning)
        {
            return;
        }
        if (Target.isStand == false)
        {
            this.Target.y -= 0.1f;
            if (Target.y <= 0.0f) Target.y = 0.0f;
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

    public RunState()
    {

    }
    public override void UpdateMS()
    {
        if (Target.isAttacking && Target.isStand) return;
        if (Target.isHurt) return;
        this.Target.x -= (0.05f * this.Target.flipX);

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

    public override string GetAnimationName() { return "2000"; }

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
            this.Target.AddBuffer<BufferFlashMove>();
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
            this.Target.AddBuffer<BufferFlashMove>();
            if (cd_attack > 0) return;

            this.Enable = true;
            Target.isAttacking = true;
            Target.attackingAnimationName = Target.ani_atk; // this.GetAnimationName();
            //    BulletMgr.Create<Bullet2_0>(this.Target);
            cd_attack = 40;//2s cd

            BulletMgr.Create(this.Target, Target.bulleClassName_atk1, Target.bullet_atk1_info);

        }
        else if (this.Enable == true && Events.ID_SPINE_COMPLETE == type)
        {
            Target.isAttacking = false;
            this.Enable = false;
            //    Debug.Log(" 连招  1111 完成");
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

    public override string GetAnimationName() { return "2120"; }

    bool checkForTimeOut = true;
    int tick = 0;
    public AttackState_2()
    {
        Debug.Log(" 连招  222 待命");
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
            Debug.Log(" 连招  222 超时");

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
            Target.attackingAnimationName = this.GetAnimationName();
            this.checkForTimeOut = false;
            this.Enable = true;
            Target.isAttacking = true;
        }
        else if (this.Enable == true && Events.ID_SPINE_COMPLETE == type)
        {
            Debug.Log("连招   222 完成");

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


    public override string GetAnimationName() { return "2130"; }

    bool checkForTimeOut = true;
    int tick = 0;
    public AttackState_3()
    {
        Debug.Log("连招   3333 待命");

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
            Debug.Log("连招   333333 超时");
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
            Target.attackingAnimationName = this.GetAnimationName();
            this.Enable = true;
            Target.isAttacking = true;
            this.checkForTimeOut = false;

        }
        else if (this.Enable == true && Events.ID_SPINE_COMPLETE == type)
        {
            Target.isAttacking = false;
            this.Enable = false;
            Debug.Log("连招   33333 完成");
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



public class SkillState : StateBase
{
    // 释放技能   share state isAttacking

    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(SkillState))
        {
            return this;
        }
        return null;
    }
    public override string GetAnimationName() { return "2110"; }
    public SkillState()
    {

    }
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



}





/// <summary>
/// 主场景点击操作 状态
/// </summary>
public class RunXYState : StateBase
{
    public override StateBase GetState<T>()
    {
        if (typeof(T) == typeof(RunState))
        {
            return this;
        }
        return null;
    }

    public RunXYState()
    {

    }
    private bool isXok = false;
    private bool isYok = false;
    public override void UpdateMS()
    {
        float distance = Target.ClaculateDistance(x, y);
        //  Debug.Log(" distance : " + distance);
        if (distance < 0.06f)
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
        if (Mathf.Abs(y - Target.y) < 0.06f)
        {
            Target.y = y;
        }
        if (Mathf.Abs(x - Target.x) < 0.06f)
        {
            Target.x = x;
        }

        float speed = 0.05f;


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


        float dx = this.x - Target.x;
        float dy = this.y - Target.y;


        degree = Vector2.Angle(new Vector2(Target.x, Target.y), pos);
        
        degree = Utils.GetDegree(new Vector2(Target.x, Target.y), pos );

        Debug.Log(degree + " degree");

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