using UnityEngine;
using System.Collections;

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
        scale = 0.8f;
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
        if (this.enable_pvp_ai)
        {
            this.AI_UpdateMSWithAI();
        }

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
            /// eventDispatcher.PostEvent(Events.ID_BTN_ATTACK);
        }

    }


    Hero target = null;

    private int cd_atk = 0;
    public virtual void AI_UpdateMSWithAI()
    {
        cd_atk--;

        //如果目标非法，那么寻找另外一个目标
        if (target == null || target.IsInValid())
        {
            this.AI_SearchNewTarget();
            return;
        }
        if (isHurt) return;

        // 有目标 ，先判断是否在攻击范围内
        float dis = target.ClaculateDistance(x, y);
        if (dis < 2)
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
        if (this.x < target.x)
        {
            //玩家在右方
            right = true;
        }
        else
        {
            //玩家在左方
            left = true;
        }
    }
    public virtual void AI_AttackTarget()
    {
        if (cd_atk <= 0)
        {
            //   target.TakeAttack(this);
            BulletMgr.Create(this, this.bulleClassName_atk1,this.bullet_atk1_info);


            atk = true;
            cd_atk = 80;// 2S
        }
        else
        {
            stand = true;
        }
    }



}