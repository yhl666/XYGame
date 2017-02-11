using UnityEngine;
using System.Collections;

public class Hero : Entity
{
    public override bool Init()
    {
        base.Init();

        return true;
    }

    public override void UpdateMS()
    {

        base.UpdateMS();
    }
}




public class BattleHero : Hero
{

    public override void InitStateMachine()
    {
        base.InitStateMachine();

        //init state machine

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
            s.PushSingleState(StateBase.Create<SkillState>(this));
        }
        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<HurtState>(this));
        }
    }
    public override bool Init()
    {
        base.Init();

        // config
        this.skin = "#1";
        this.prefabsName = "Prefabs/Hero2";

        ani_hurt = "hurt";
        ani_jumpTwice = "doubleJump";
        ani_jump = "jump";
        ani_fall = "fall";
        ani_run = "run";
        ani_stand = "stand";

        bulleClassName_atk1 = "BulletConfig"; //"Bullet2_0";//普通攻击 1段  的子弹名字
        bulleClassName_s1 = "Bullet2_1"; // 1 号技能 子弹名字

        bullet_atk1_info = BulletConfigInfo.Create();



        ViewMgr.Create<ViewEntity>(this);

        //   EnemyMgr.Create<Enemy>();

        this.eventDispatcher.AddEventListener(this, Events.ID_LAUNCH_SKILL1);
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
    }
    public override void UpdateMS()
    {
        //process  input status
        if (atk)
        {
            eventDispatcher.PostEvent(Events.ID_BTN_ATTACK);
            atk = false;
        }
        if (stand)
        {
            eventDispatcher.PostEvent(Events.ID_STAND);
            stand = false;
        }
        if (left)
        {
            eventDispatcher.PostEvent(Events.ID_BTN_LEFT);
            left = false;
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

        if (s1)
        {
            eventDispatcher.PostEvent(Events.ID_LAUNCH_SKILL1);
            s1 = false;
        }


        base.UpdateMS();

        if (team == 1)
        {//test
            eventDispatcher.PostEvent(Events.ID_BTN_ATTACK);
        }

    }

}