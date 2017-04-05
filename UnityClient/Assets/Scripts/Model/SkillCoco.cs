using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Skill61_1 : SkillBase
{
    Counter cd = Counter.Create(Skill61_1_Data.ins.cd);
    Counter tick_cancel = Counter.Create(Skill61_1_Data.ins.cancel);
    Counter launchCounter=Counter.Create(Skill61_1_Data.ins.bulletLaunchDealy);
    public override void OnEnter()
    {
        cd.Reset();
        this.PauseAll();
        Target.isAttacking = true;
        this.Enable = true;
        Target.attackingAnimationName = Skill61_1_Data.ins.animation_name;

 
    }

    private void shoot()
    {
        BulletConfigInfo info = BulletConfigInfo.Create();

        BufferSpin buffer = BufferMgr.CreateHelper<BufferSpin>(Target);
        buffer.SetLastTime(Skill61_1_Data.ins.spinTime);
        info.AddBuffer(buffer);


        info.damage_ratio = Skill61_1_Data.ins.damage_ratio;
        info.collider_size = Skill61_1_Data.ins.hit_rect;


        info.launch_delta_xyz.x = Skill61_1_Data.ins.PositionVector2.x;

        info.launch_delta_xyz.y = Skill61_1_Data.ins.PositionVector2.y;
        info.frameDelay = 4;
        info.distance_atk = 2.0f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.damage_ratio = 1.5f;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = "hd/magic_weapons/bullet/bul_5000141/bul_5000141.plist";
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 15;
        info.scale_x = 2f;
        info.scale_y = 2f;
        BulletMgr.Create(this.Target, "BulletConfig", info);
    }

    public override void OnSpineComplete()
    {
        this.OnExit();
    }

    public override void UpdateMS()
    {
        cd.Tick();
        tick_cancel.Tick();
        launchCounter.Tick();
        if (launchCounter.GetCurrent()==launchCounter.GetMax())
        {
            shoot();
            
        }
    }

    public override void OnPush()
    {
        if (cd.IsMax() == false) return;
        if (Target.isStand == false) return;
        if (this.Enable) return;
        if (Target.isAttacking)
        {
            this.PushOnInterruptAttackSate(); //强制打断普通攻击
        }
        if (Target.isAttacking)
        {
            Debug.Log("发起打断请求");
            this.PushOnInterrupted();
            return;
        }
        this.OnEnter();
    }
    public override bool Init()
    {
        base.Init();
        cd.TickMax();
        return true;
    }
    public override void UpdateMSIdle()
    {
        cd.Tick();
    }

    public override void OnExit()
    {
        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        launchCounter.Reset();
        this.ResumeAll();
    }

    public override void OnAcceptInterrupted(SkillBase who)
    {
        if (this.Enable) return;
        Debug.Log("interrupt");
        this.OnEnter();
    }

    public override bool OnInterrupted(SkillBase what)
    {
        if (this.Enable && tick_cancel.IsMax())
        {
            this.OnExit();
            return true;
        }
        return false;
    }

    public override string GetName()
    {
        return "Skill61_1";
    }
}


public class Skill61_2 : SkillBase
{
    public override void OnEnter()
    {
        cd.Reset();
        cancelCounter.Reset();
        tick1Counter.Reset();
        this.PauseAll();
        Target.isAttacking = true;
        this.Enable = true;
        Target.attackingAnimationName = Skill61_2_Data.ins.animation_name;
        forward = Target.flipX;
    }

    private void shoot()
    {
        BulletConfigInfo info = BulletConfigInfo.Create();

        info.AddBuffer("BufferHitBack");
        info.launch_delta_xyz.x = 0;
        info.launch_delta_xyz.y = 0;
        info.frameDelay = 4;
        info.distance_atk = 2.0f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.damage_ratio = Skill61_2_Data.ins.damage_ratio;
        info.collider_size = Skill61_2_Data.ins.hit_rect;

        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        //info.plistAnimation = "hd/magic_weapons/bullet/bul_5000141/bul_5000141.plist";
        /// info.rotate = 30.0f;
        info.distance = Skill61_2_Data.ins.distance;
        info.speed = Skill61_2_Data.ins.speed;
        info.lastTime = 10;
        info.scale_x = 2f;
        info.scale_y = 2f;
        bullet = BulletMgr.Create(this.Target, "BulletConfig", info);
        //bullet.x = Target.x;
        //bullet.y = Target.y;
    }

    public override void OnSpineComplete()
    {
        this.OnExit();
    }

    public override void UpdateMS()
    {
        cd.Tick();
        cancelCounter.Tick();
        if (tick1Counter.Tick() != true && isReleased == false)
        {
            isReleased = true;
            shoot();
        }
    }

    public override void OnPush()
    {
        if (cd.IsMax() == false) return;
        if (Target.isStand == false) return;
        if (this.Enable) return;
        if (Target.isAttacking)
        {
            this.PushOnInterruptAttackSate(); //强制打断普通攻击
        }
        if (Target.isAttacking)
        {
            Debug.Log("发起打断请求");
            this.PushOnInterrupted();
            return;
        }
        this.OnEnter();
    }
    public override bool Init()
    {
        base.Init();
        cd.TickMax();
        return true;
    }

    public override bool OnInterrupted(SkillBase skillBase)
    {
        if (cancelCounter.IsMax())
        {
            this.OnExit();
            return true;
        }
       
        return false;
    }

    public override void UpdateMSIdle()
    {
        cd.Tick();
    }

    public override void OnExit()
    {
        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        this.ResumeAll();
        isReleased = false;
    }

    public override void OnAcceptInterrupted(SkillBase who)
    {
        if (this.Enable) return;
        this.OnEnter();
    }

    public override string GetName()
    {
        return "Skill61_2";
    }

    Counter cd = Counter.Create(Skill61_2_Data.ins.cd);
    Counter tick1Counter = Counter.Create(Skill61_2_Data.ins.bulletLaunchDealy);
    Counter cancelCounter=Counter.Create(Skill61_2_Data.ins.cancel);
    private float forward;
    private Bullet bullet;
    private bool isReleased = false;
}


/// <summary>
///减速的雷落下来 立即生效
/// </summary>
public class Skill61_3 : SkillBase
{
    Counter cd=Counter.Create(Skill61_3_Data.ins.cd);
    Counter cancelCounter=Counter.Create(Skill61_3_Data.ins.cancel);
    Counter dealyCounter=Counter.Create(Skill61_3_Data.ins.delayFrame);
    public override void OnEnter()
    {
        cd.Reset();
        cancelCounter.Reset();
        dealyCounter.Reset();
        Target.machine.PauseAllStack();
        this.stack.parent.stack.Resume();
        is_shifa = true;

        Target.isAttacking = true;
        this.Enable = true;
        //开始施法
        Target.attackingAnimationName = Skill61_3_Data.ins.animation_name;

        b_shifa = null;


       
    }

    private void shoot()
    {
        BulletConfigInfo info = BulletConfigInfo.Create();
        info.plistAnimation = "hd/arousal_skill_bullet/arousal_skill_bullet_6300026/arousal_skill_bullet_6300026.plist";
        info.distance_atk = 2.0f;
        info.distance = 0;
        info.number = 999;
        info.speed = 1.0f;
        info.number = 999;
        info.lastTime = 5;
        info.isHitDestory = false;
        info.collider_size = Skill61_3_Data.ins.hit_rect;
        BufferSpeedSlow buffer = BufferMgr.CreateHelper<BufferSpeedSlow>(Target);
        buffer.percent = Skill61_3_Data.ins.slowPrecent;
        buffer.SetLastTime(Skill61_3_Data.ins.lastTime);
        info.AddBuffer(buffer);
        info._OnMoveFunc = (BulletConfig bulletConfig, Vector2 vector) => { return GetNearHeroVector(); }
            ;
        var b = BulletMgr.Create(this.Target, "BulletConfig", info);

        b.x = GetNearHeroVector().x;
        b.y = GetNearHeroVector().y;
    }
    public override void UpdateMS()
    {
        cd.Tick();
        cancelCounter.Tick();
        dealyCounter.Tick();
        if (dealyCounter.GetCurrent()==dealyCounter.GetMax())
        {
            shoot();
        }

    }

    public override void UpdateMSIdle()
    {
        cd.Tick();
    }

    public override void OnSpineComplete()
    {
        this.OnExit();
    }

    public override void OnExit()
    {
        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        Target.machine.ResumeAllStack();
        //this.is_shifa = true;
    }

    public override void OnPush()
    {
        if (cd.IsMax() == false) return;
        if (Target.isStand == false) return;

        if (Target.isAttacking)
        {
            this.PushOnInterruptAttackSate(); //强制打断普通攻击
        }
        if (Target.isAttacking)
        {
            Debug.Log("发起打断请求");
            this.PushOnInterrupted();
            return;
        }
        this.OnEnter();
    }

    public override bool OnInterrupted(SkillBase what)
    {
        if (cancelCounter.IsMax())
        {
            this.OnExit();
            return true;
        }
        return false;
    }

    public override void OnAcceptInterrupted(SkillBase who)
    {
        if (this.Enable)
        {
            return;
        }
        this.OnEnter();
    }

    private Vector2 GetNearHeroVector()
    {
        float minDistance = 9999.0f;
        Vector2 vector2 = new Vector2();
        ArrayList heros = HeroMgr.ins.GetHeros();
        foreach (Hero VARIABLE in heros)
        {
            if (VARIABLE == this.Target)
            {
                continue;
            }

            if (minDistance > Math.Abs(VARIABLE.x - this.Target.x))
            {
                minDistance = Math.Abs(VARIABLE.x - this.Target.x);
                vector2.x = VARIABLE.x;
                vector2.y = VARIABLE.y;
            }
        }
        return vector2;
    }

    public override string GetName()
    {
        return "Skill61_3";
    }

    private bool is_shifa = true;

    Bullet b_shifa = null;
}