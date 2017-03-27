/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
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

        /* ------------------------------------------hero 2
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
        ///  bulleClassName_atk1 = "BulletStateMachineTest";

        bullet_atk1_info = BulletConfigInfo.Create();
        bullet_atk1_info.AddBuffer("BufferHitBack");
 
        */

        //-------------------- hero 6
        this.skin = "#1";
        this.prefabsName = "Spine/6/Hero6";

        ani_hurt = "hurt";
        ani_jumpTwice = "doubleJump";
        ani_jump = "jump";
        ani_fall = "fall";
        ani_run = "run";
        ani_stand = "rest";
        this.ani_atk1 = "6000";
        this.ani_atk2 = "6010";
        this.ani_atk3 = "6020";



        {
            bulleClassName_atk1 = "BulletConfig"; //"Bullet2_0";//普通攻击 1段  的子弹名字
            bulleClassName_s1 = "Bullet2_1"; // 1 号技能 子弹名字
            ///  bulleClassName_atk1 = "BulletStateMachineTest";

            BulletConfigInfo info = BulletConfigInfo.Create();
            bullet_atk1_info = info;
            info.AddBuffer("BufferHitBack");
            info.AddBuffer("BufferSpin");

            info.launch_delta_xy.x = 1f;
            info.launch_delta_xy.y = 0f;

            info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6001/role_6_bul_6001.plist";
            info.rotate = 30.0f;
            info.distance = 0;
            info.distance_atk = 1.0f;
            info.isHitDestory = false;
            info.number = 0xfff;
            info.oneHitTimes = 1;
            info.frameDelay = 4;
            info.scale_x = 1.5f;
            info.scale_y = 1.5f;
            info.lastTime = 15;
            info._OnLaunch = (Bullet b,object userData) =>
            {

                if (this.flipX < 0)
                {
                    this.x_auto += this.speed * 2;
                }
                else
                {
                    this.x_auto -= this.speed * 2;
                }
            };
        }


        {
            bulleClassName_atk2 = "BulletConfig"; //"Bullet2_0";//普通攻击 2段  的子弹名字
            ///  bulleClassName_atk1 = "BulletStateMachineTest";

            BulletConfigInfo info = BulletConfigInfo.Create();
            bullet_atk2_info = info;
            info.AddBuffer("BufferHitBack");

            info.launch_delta_xy.x = 1f;
            info.launch_delta_xy.y = 0f;
            info.frameDelay = 4;
            info.distance_atk = 1.0f;
            info.number = 0xfff;
            info.isHitDestory = false;
            info.oneHitTimes = 1;
            //  info.rotate = -120.0f;
            info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6011/role_6_bul_6011.plist";
            /// info.rotate = 30.0f;
            info.distance = 0;
            info.lastTime = 15;
            info.scale_x = 1.5f;
            info.scale_y = 1.5f;
            info._OnLaunch = (Bullet b, object userData) =>
                {

                    if (this.flipX < 0)
                    {
                        this.x_auto += this.speed * 2;
                    }
                    else
                    {
                        this.x_auto -= this.speed * 2;
                    }
                };
        }


        {
            bulleClassName_atk3 = "BulletConfig"; //"Bullet2_0";//普通攻击 3段  的子弹名字
            ///  bulleClassName_atk1 = "BulletStateMachineTest";

            BulletConfigInfo info = BulletConfigInfo.Create();
            bullet_atk3_info = info;
            info.AddBuffer("BufferHitBack");

            info.launch_delta_xy.x = 1f;
            info.launch_delta_xy.y = 0f;
            info.distance_atk = 1.3f;
            info.number = 0xfff;
            info.oneHitTimes = 1;
            info.scale_x = 1.5f;
            info.scale_y = 1.5f;
            info.isHitDestory = false;
            info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6021/role_6_bul_6021.plist";
            //    info.rotate = 30.0f;
            info.distance = 0;
            info.frameDelay = 4;
            info.lastTime = 15;
            info._OnLaunch = (Bullet b, object userData) =>
            {

                if (this.flipX < 0)
                {
                    this.x_auto += this.speed * 2;
                }
                else
                {
                    this.x_auto -= this.speed * 2;
                }
            };
        }










        ViewMgr.Create<ViewEntity>(this);

        this.team = 1;

        this.current_hp = 0xffffff;
        this.hp = 0xffffff;
        this.atk_level = 3;


        DAO.Equip equip = EquipMgr.ins.GetTestEquip();

        foreach (string buf in equip.buffers)
        {
            this.AddBuffer(buf);
        }
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

        if (this.isDie)
        {
            if (enable_pvp_ai)
            {
                BufferRevive b = new BufferRevive();
                b.point_index = 1;
                this.AddBuffer(b);
                return;
            }
            this.machine.Pause();

            ///   EventDispatcher.ins.PostEvent(Events.ID_DIE, this);

            return;
        }
        if (this.enable_pvp_ai)
        {
            ///   this.AI_UpdateMSWithAI();
        }

        //process  input status

        ////  if (this.machine.IsAllStackPause() == false)
        {
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

            if (s1 != 0)
            {
                eventDispatcher.PostEvent(Events.ID_LAUNCH_SKILL1, s1);
                s1 = 0;
            }
        }
        base.UpdateMS();

        if (team == 1)
        {//test
            /// eventDispatcher.PostEvent(Events.ID_BTN_ATTACK);
        }

    }


    Hero target = null;

    private int cd_atk = 0;
    public override void AI_UpdateMSWithAI()
    {
        if (false == enable_pvp_ai) return;
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
            if (h == this) continue;
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
      //  stand = true;
        //return;
        if (target.isDie)
        {
            target = null;
            stand = true;
            return;
        }
        if (cd_atk <= 0)
        {

            //   target.TakeAttack(this);
            //    BulletMgr.Create(this, this.bulleClassName_atk1, this.bullet_atk1_info);
          //  atk = true;
            s1 = 4;
            cd_atk = 80;// 2S
        }
        else
        {
            stand = true;
        }
    }



}