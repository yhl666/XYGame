using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Skill61_1:SkillBase
{
    Counter counter=Counter.Create(400);
    public override void OnEnter()
    {
        this.PauseAll();
        Target.isAttacking = true;
        this.Enable = true;
        Target.attackingAnimationName = "6130_0";
        shoot();
        counter.Reset();
    }
    private void shoot()
    {
        BulletConfigInfo info = BulletConfigInfo.Create();

        BufferSpin buffer = BufferMgr.CreateHelper<BufferSpin>(Target);
        buffer.SetLastTime(1.5f);
        info.AddBuffer(buffer);

        info.launch_delta_xy.x = 0;

        info.launch_delta_xy.y = -0.2f;
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
    public override void OnSpineCompolete()
    {
        this.OnExit();
    }
    public override void UpdateMS()
    {
        counter.Tick();
    }
    public override void OnPush()
    {
        if (Target.isStand == false) return;
        if (Target.isAttacking) return;
        if (this.Enable) return;
        //if (counter.Tick()==true)
        //{
        //    return;
        //}
        this.OnEnter();
 
    }
    public override void UpdateMSIdle()
    {
        counter.Tick();
    }
    public override void OnExit()
    {
        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        this.ResumeAll();
 
    }

    public override void OnInterrupted(object what)
    {
        base.OnInterrupted(what);
    }
}

/// <summary>
///可被打断 角色施法 出一个伤害移动的伤害范围， 再次按下触发 一颗雷从天而降 造成大量伤害
/// </summary>
public class Skill61_3 : SkillBase
{
    public override void OnEnter()
    {
        //tick.Reset();
        //tick1.Reset();
        Target.machine.PauseAllStack();
        this.stack.parent.stack.Resume();
        is_shifa = true;

        Target.isAttacking = true;
        this.Enable = true;
        //开始施法
        Target.attackingAnimationName = "6230_0";

        b_shifa = null;

       

        BulletConfigInfo info = BulletConfigInfo.Create();
        info.plistAnimation = "hd/arousal_skill_bullet/arousal_skill_bullet_6300026/arousal_skill_bullet_6300026.plist";
        info.distance_atk = 2.0f;
        info.distance = 0;
        info.number = 999;
        info.speed = 1.0f;
        info.number = 999;
        info.lastTime =5;
        info.isHitDestory = false;
        BufferSpeedSlow buffer = BufferMgr.CreateHelper<BufferSpeedSlow>(Target);
        buffer.id = 0xef1;
        buffer.percent = 99;
        info.AddBuffer(buffer);
        info._OnMoveFunc = (BulletConfig bulletConfig, Vector2 vector) =>
            {
                return GetNearHeroVector();
            }
     
    
        ;
        var b = BulletMgr.Create(this.Target, "BulletConfig", info)
            ;

        b.x = GetNearHeroVector().x;
        b.y = GetNearHeroVector().y;


    }
    public override void UpdateMS()
    {
        
    }

    public override void OnSpineCompolete()
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
        if (Target.isStand == false) return;

        
            this.OnEnter();
        

    }
   

    private Vector2 GetNearHeroVector()
    {
        float minDistance = 9999.0f;
        Vector2 vector2=new Vector2();
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

    private bool is_shifa = true;

    Bullet b_shifa = null;

}

public class Skill61_2 : SkillBase
{
    Counter counter = Counter.Create(400);
    public override void OnEnter()
    {
        this.PauseAll();
        Target.isAttacking = true;
        this.Enable = true;
        Target.attackingAnimationName = "6210";
        forward = Target.flipX;
        
    }
    private void shoot()
    {
        BulletConfigInfo info = BulletConfigInfo.Create();

        info.AddBuffer("BufferHitBack");
        info.launch_delta_xy.x = 0;
        info.launch_delta_xy.y = 0;
        info.frameDelay = 4;
        info.distance_atk = 2.0f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.damage_ratio = 1.5f;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        //info.plistAnimation = "hd/magic_weapons/bullet/bul_5000141/bul_5000141.plist";
        /// info.rotate = 30.0f;
        info.distance = 5;
        info.speed = 0.2f;
        info.lastTime = 10;
        info.scale_x = 2f;
        info.scale_y = 2f;
        bullet= BulletMgr.Create(this.Target, "BulletConfig", info);
        //bullet.x = Target.x;
        //bullet.y = Target.y;
    }
    public override void OnSpineCompolete()
    {
        this.OnExit();
    }
    public override void UpdateMS()
    {
        if (tick1Counter.Tick()!=true&&isReleased==false)
        {
            isReleased = true;
            shoot();    
        }
        
    }
    public override void OnPush()
    {
        if (Target.isStand == false) return;
        if (Target.isAttacking) return;
        if (this.Enable) return;
        //if (counter.Tick() == true)
        //{
        //    return;
        //}
        this.OnEnter();

    }
    public override void OnInterrupted(AttackInfo info)
    {

        if (tick1Counter.Tick()!=true)
        {
            this.OnExit();
        }

    }
    public override void UpdateMSIdle()
    {
        counter.Tick();
    }
    public override void OnExit()
    {
        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        this.ResumeAll();
        counter.Reset();
        tick1Counter.Reset();
        isReleased = false;
    }
    Counter tick1Counter=Counter.Create(12);
    private float forward;
    private Bullet bullet;
    private bool isReleased = false;
}