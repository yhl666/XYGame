using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Skill61_1 : SkillBase
{
    Counter cd = Counter.Create(Skill61_1_Data_V1.ins.cd);
    Counter tick_cancel = Counter.Create(Skill61_1_Data_V1.ins.cancel);
    Counter launchCounter=Counter.Create(Skill61_1_Data_V1.ins.bulletLaunchDealy);
    public override void OnEnter()
    {
        cd.Reset();
        this.PauseAll();
        Target.isAttacking = true;
        this.Enable = true;
        Target.attackingAnimationName = Skill61_1_Data_V1.ins.animation_name;

 
    }

    private void shoot()
    {
        BulletConfigInfo info=BulletConfigInfo.Create();
        {
            BufferHitBack hitBack = BufferMgr.CreateHelper<BufferHitBack>(this.Target);
            hitBack.position = this.Target.pos;
            info.AddBuffer(hitBack);

            info.damage_ratio = Skill61_1_Data_V1.ins.damage_ratio;
            info.collider_size = Skill61_1_Data_V1.ins.hit_rect;


            info.launch_delta_xyz.x = Skill61_1_Data_V1.ins.PositionVector2.x;

            info.launch_delta_xyz.y = Skill61_1_Data_V1.ins.PositionVector2.y;
            info.frameDelay = 4;
            info.distance_atk = 2.0f;
            info.number = 0xfff;
            info.isHitDestory = false;
            info.oneHitTimes = 1;
            //  info.rotate = -120.0f;
            info.plistAnimation = "hd/magic_weapons/bullet/bul_5000141/bul_5000141.plist";
            /// info.rotate = 30.0f;
            info.distance = 0;
            info.lastTime = 15;
            info.scale_x = Skill61_1_Data_V1.ins.scale_x;
            info.scale_y = Skill61_1_Data_V1.ins.scale_y;
        }

        BulletMgr.Create(this.Target, "BulletConfig", info);
       
        
    }

    private void shoot2()
    {
        BulletConfigInfo info2 = BulletConfigInfo.Create();
        BufferHitBack hitBack = BufferMgr.CreateHelper<BufferHitBack>(this.Target);
        hitBack.position = this.Target.pos;
        info2.AddBuffer(hitBack);



          BufferSpin spin = BufferMgr.CreateHelper<BufferSpin>(this.Target);
          spin.SetLastTime(Skill61_1_Data_V2.ins.spinTime);
          info2.AddBuffer(spin);
          {

              info2.damage_ratio = Skill61_1_Data_V2.ins.damage_ratio;
              info2.collider_size = Skill61_1_Data_V2.ins.hit_rect;


              info2.launch_delta_xyz.x = Skill61_1_Data_V2.ins.PositionVector2.x;

              info2.launch_delta_xyz.y = Skill61_1_Data_V2.ins.PositionVector2.y;
              info2.frameDelay = 4;
              info2.distance_atk = 2.0f;
              info2.number = 0xfff;
              info2.isHitDestory = false;
              info2.oneHitTimes = 1;
              //  info.rotate = -120.0f;
              info2.plistAnimation = "hd/magic_weapons/bullet/bul_5000141/bul_5000141.plist";
              /// info.rotate = 30.0f;
              info2.distance = 0;
              info2.lastTime = 15;
              info2.scale_x = Skill61_1_Data_V2.ins.scale_x;
              info2.scale_y = Skill61_1_Data_V2.ins.scale_y;
          }
          BulletMgr.Create(this.Target, "BulletConfig", info2);
    }

    private void shoot3()
    {
        BulletConfigInfo info3 = BulletConfigInfo.Create();
        BufferSpin spin = BufferMgr.CreateHelper<BufferSpin>(this.Target);
        BufferHitBack hitBack = BufferMgr.CreateHelper<BufferHitBack>(this.Target);
        hitBack.position = this.Target.pos;
        info3.AddBuffer(hitBack);
        spin.SetLastTime(Skill61_1_Data_V3.ins.spinTime);

        {
            
            info3.AddBuffer(spin);
            info3.damage_ratio = Skill61_1_Data_V3.ins.damage_ratio;
            info3.collider_size = Skill61_1_Data_V3.ins.hit_rect;


            info3.launch_delta_xyz.x = Skill61_1_Data_V3.ins.PositionVector2.x;

            info3.launch_delta_xyz.y = Skill61_1_Data_V3.ins.PositionVector2.y;
            info3.frameDelay = 4;
            info3.distance_atk = 2.0f;
            info3.number = 0xfff;
            info3.isHitDestory = false;
            info3.oneHitTimes = 1;
            //  info.rotate = -120.0f;
            info3.plistAnimation = "hd/magic_weapons/bullet/bul_5000141/bul_5000141.plist";
            /// info.rotate = 30.0f;
            info3.distance = 0;
            info3.lastTime = 15;
            info3.scale_x = Skill61_1_Data_V3.ins.scale_x;
            info3.scale_y = Skill61_1_Data_V3.ins.scale_y;
        }

        BulletMgr.Create(this.Target, "BulletConfig", info3);
    }
    public void LevelUp()
    {
        if (level<3)
        {
            level++;
        }
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
            switch (level)
            {
                case 1:
                    shoot();
                    break;
                case 2:
                    shoot2();
                  
                    break;
                case 3:
                    shoot3();
                    break;
            }
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
        ConfigBulletInfo();
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

    public void ConfigBulletInfo()
    {
       

       
    }

    private int level = 2;
   


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
        Target.attackingAnimationName = Skill61_2_Data_V1.ins.animation_name;
        forward = Target.flipX;
    }

    private void shoot()
    {
        BulletConfigInfo info = BulletConfigInfo.Create();
        {
            info.launch_delta_xyz.x = 0;
            info.launch_delta_xyz.y = 0;
            info.frameDelay = 4;
            info.distance_atk = 2.0f;
            info.number = 0xfff;
            info.isHitDestory = Skill61_2_Data_V1.ins.immediateDisappear;
            info.damage_ratio = Skill61_2_Data_V1.ins.damage_ratio;
            info.collider_size = Skill61_2_Data_V1.ins.hit_rect;
            info.validTimes = 99999;
            info.oneHitTimes = 1;
            //  info.rotate = -120.0f;
            //info.plistAnimation = "hd/magic_weapons/bullet/bul_5000141/bul_5000141.plist";
            /// info.rotate = 30.0f;
            info.distance = Skill61_2_Data_V1.ins.distance;
            info.speed = Skill61_2_Data_V1.ins.speed;
            info.lastTime = 10;
            info.scale_x = 2f;
            info.scale_y = 2f;

        }
        BulletMgr.Create(this.Target, "BulletConfig", info);

      


        //bullet.x = Target.x;
        //bullet.y = Target.y;
    }

    private void shoot2()
    {
         BulletConfigInfo info2 = BulletConfigInfo.Create();
        {
            info2.launch_delta_xyz.x = 0;
            info2.launch_delta_xyz.y = 0;
            info2.frameDelay = 4;
            info2.distance_atk = 2.0f;
            info2.number = 0xfff;
            //Debug.Log("is hit history " + Skill61_2_Data_V2.ins.immediateDisappear);
            info2.isHitDestory = Skill61_2_Data_V2.ins.immediateDisappear;
            info2.damage_ratio = Skill61_2_Data_V2.ins.damage_ratio;
            info2.collider_size = Skill61_2_Data_V2.ins.hit_rect;
            info2.validTimes = 99999;
            info2.oneHitTimes = 1;
            //  info.rotate = -120.0f;
            //info.plistAnimation = "hd/magic_weapons/bullet/bul_5000141/bul_5000141.plist";
            /// info.rotate = 30.0f;
            info2.distance = Skill61_2_Data_V2.ins.distance;
            info2.speed = Skill61_2_Data_V2.ins.speed;
            info2.lastTime = 10;
            info2.scale_x = 2f;
            info2.scale_y = 2f;

        }
        BulletMgr.Create(this.Target, "BulletConfig", info2);
    }

    private void shoot3()
    {
        BulletConfigInfo info3 = BulletConfigInfo.Create();
        {
            info3.launch_delta_xyz.x = 0;
            info3.launch_delta_xyz.y = 0;
            info3.frameDelay = 4;
            info3.distance_atk = 2.0f;
            info3.number = 0xfff;
            info3.isHitDestory = Skill61_2_Data_V3.ins.immediateDisappear;
            info3.damage_ratio = Skill61_2_Data_V3.ins.damage_ratio;
            info3.collider_size = Skill61_2_Data_V3.ins.hit_rect;
            info3.validTimes = 99999;
            info3.oneHitTimes = 0xffffff;
            //  info.rotate = -120.0f;
            //info.plistAnimation = "hd/magic_weapons/bullet/bul_5000141/bul_5000141.plist";
            /// info.rotate = 30.0f;
            info3.distance = Skill61_2_Data_V1.ins.distance;
            info3.speed = Skill61_2_Data_V1.ins.speed;
            info3.lastTime = 10;
            info3.scale_x = Skill61_1_Data_V3.ins.scale_x;
            info3.scale_y = Skill61_1_Data_V3.ins.scale_y;
        }
        BulletMgr.Create(this.Target, "BulletConfig", info3);
    }
    public void ConfigBulletInfo()
    {
        


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
            switch (level)
            {
                case 1:
                    shoot();
                    break;
                case 2:
                    shoot2();
                    break;
                case 3:
                    shoot3();
                    break;
            }
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
        ConfigBulletInfo();
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

    Counter cd = Counter.Create(Skill61_2_Data_V1.ins.cd);
    Counter tick1Counter = Counter.Create(Skill61_2_Data_V1.ins.bulletLaunchDealy);
    Counter cancelCounter=Counter.Create(Skill61_2_Data_V1.ins.cancel);


    private float forward;
    private Bullet bullet;
    private bool isReleased = false;
    private int level = 2;


  
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

    private void shoot1()
    {
        


        ArrayList list = GetEnemyByOrder();

        Debug.Log(list.Count);
        for (int i = 0; i < 3&&i<list.Count; i++)
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
            BufferSpeedSlow buffer = BufferMgr.CreateHelper<BufferSpeedSlow>(this.Target);
            buffer.percent = Skill61_3_Data.ins.slowPrecent;
            //buffer.SetLastTime(Skill61_3_Data.ins.lastTime);
            info.AddBuffer(buffer);
            var b = BulletMgr.Create(this.Target, "BulletConfig", info);
            Enemy enemy=list[i] as Enemy;
            b.x = enemy.x;
            b.z = enemy.z;
            b.y = enemy.y;
        }

        
    }
    private void shoot2()
    {



        ArrayList list = GetEnemyByOrder();
        Debug.Log(list.Count);
        for (int i = 0; i < 10&&i<list.Count; i++)
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
            BufferSpeedSlow buffer = BufferMgr.CreateHelper<BufferSpeedSlow>(this.Target);
            buffer.percent = Skill61_3_Data.ins.slowPrecent;
            buffer.SetLastTime(Skill61_3_Data.ins.lastTime);
            info.AddBuffer(buffer);

            var b = BulletMgr.Create(this.Target, "BulletConfig", info);
            Enemy enemy = list[i] as Enemy;
            b.x = enemy.x;
            b.z = enemy.z;
            b.y = enemy.y;
        }


    }
    private void shoot3()
    {
        


        ArrayList list = GetEnemyByOrder();
        Debug.Log(list.Count);
        for (int i = 0; i < 10&&i<list.Count; i++)
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
            BufferSpeedSlow buffer = BufferMgr.CreateHelper<BufferSpeedSlow>(this.Target);
            buffer.percent = Skill61_3_Data.ins.slowPrecent;
            buffer.SetLastTime(Skill61_3_Data.ins.lastTime);
            info.AddBuffer(buffer);
            var b = BulletMgr.Create(this.Target, "BulletConfig", info);
            Enemy enemy = list[i] as Enemy;
            b.x = enemy.x;
            b.z = enemy.z;
            b.y = enemy.y;
        }


    }
    public override void UpdateMS()
    {
        cd.Tick();
        cancelCounter.Tick();
        dealyCounter.Tick();
        if (dealyCounter.GetCurrent()==dealyCounter.GetMax())
        {
            switch (level)
            {
                case 1:
                    shoot1();
                    break;
                case 2:
                    shoot2();
                    break;
                case 3:
                    shoot3();
                    break;
            }
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

    private ArrayList GetEnemyByOrder()
    {
        ArrayList enemyList = EnemyMgr.ins.GetEnemys();
        for (int i = 0; i < enemyList.Count-1; i++)
        {
            for (int j = 0; j < enemyList.Count - 1-i; j++)
            {
                if (distance(enemyList[j] as Enemy)>distance(enemyList[j+1] as Enemy))
                {
                    Enemy temp = enemyList[j + 1] as Enemy;
                    enemyList[j + 1] = enemyList[j];
                    enemyList[j] = temp;
                }
            }
        }
        return enemyList;
    }

    public override string GetName()
    {
        return "Skill61_3";
    }

    private double distance(Enemy enemy)
    {
        return
            Math.Sqrt((enemy.x - this.Target.x) * (enemy.x - this.Target.x) +
                      (enemy.y - this.Target.y) * (enemy.y - this.Target.y) +
                      (enemy.z - this.Target.z) * (enemy.z - this.Target.z));
    }
    private bool is_shifa = true;

    Bullet b_shifa = null;
    private int level = 2;
}