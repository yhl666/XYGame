/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class Building : Entity
{
    protected Entity target = null;
    public float atk_distance = 3.0f;//g攻击范围
    protected Counter cd = Counter.Create(80);

    public override void InitInfo()
    {
        this.prefabsName = "";
        scale = 0.8f;
    }
    // override 
    public override bool Init()
    {
        base.Init();


        //init state machine

        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<AttackState_1>(this));
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
        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<DieStateBuilding>(this));
        }
        this.current_hp = 3000;
        this.hp = 5000;
        ViewMgr.Create<ViewBuilding>(this);
        this.speed *= 0.5f;

        this.eventDispatcher.AddEventListener(this, Events.ID_LAUNCH_SKILL1);
        EventDispatcher.ins.AddEventListener(this, Events.ID_DIE);
        return true;
    }

    public override void OnEvent(int type, object userData)
    {
     
        if (type == Events.ID_LAUNCH_SKILL1)
        {
            //  this.AddBuffer(bufferMgr.Create<Buffer4>());

            //   Bullet b = BulletMgr.Create<Bullet2_1>(this);

        }
        else if (Events.ID_DIE == type && userData as Building  == this)
        {
            this.SetInValid();
        }
    }

    public override void UpdateMS()
    {
        cd.Tick();
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
        if (stand)
        {
            eventDispatcher.PostEvent(Events.ID_STAND);
            stand = false;
        }
        base.UpdateMS();
    }
    public void SearchNearestTarget()
    {
        ArrayList heros = HeroMgr.ins.GetHeros();
        float minDis = float.MaxValue;

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
}
public class Tower : Building
{
    public override void InitInfo()
    {
        this.prefabsName = "Prefabs/Customs/500017";
        this.skin = "default";
        ani_stand = "rest";
        ani_atk1 = "rest";
        attackingAnimationName = ani_atk1;

        bulleClassName_atk1 = "BulletConfig";//普通攻击 1段  的子弹名字
        bulleClassName_s1 = "Bullet221_0"; // 1 号技能 子弹名字
        this.bullet_atk1_info = BulletConfigInfo.Create();
        bullet_atk1_info.plistAnimation = "";
        bullet_atk1_info.distance = 0.2f;
        bullet_atk1_info.distance_atk = 1f;
        //   this.speed *= 0.001f; ;
        ///  bullet_atk1_info.AddBuffer("BufferHitBack");
        //     bullet_atk1_info.AddBuffer("BufferSpin");

        this.atk_range = 1.0f;
        scale = 0.8f;
    }

}