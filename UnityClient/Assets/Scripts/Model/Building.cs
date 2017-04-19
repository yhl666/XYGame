/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class Building : Entity
{
    public override void UpdateMS()
    {
        base.UpdateMS();
    }
    public override bool Init()
    {
        return base.Init();
    }
    public override void OnExit()
    {
        base.OnExit();
    }
    public override void OnEnter()
    {
        base.OnEnter();
    }
    public virtual void AI_UpdateMSWithAI()
    {

    }
}

public class Tower : Building
{
    protected Entity target = null;
    public float atk_distance = 3.0f;//g攻击范围
    protected Counter cd = Counter.Create(80);

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
        else if (Events.ID_DIE == type && userData as Building == this)
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

    }

    public override void AI_UpdateMSWithAI()
    {
        //如果目标非法，那么寻找另外一个目标
        if (target == null || target.IsInValid())
        {
            this.AI_SearchNewTarget();
            return;
        }
        // 有目标 ，先判断是否在攻击范围内
        float dis = target.ClaculateDistance(this);
        //     Debug.Log(dis);
        if (dis < this.atk_range)
        {
            //攻击范围内
            dir = -1;
            this.AI_AttackTarget();
        }
        else
        {
            //不在攻击范围内 移动向目标
        }

    }
    public virtual void AI_SearchNewTarget()
    {
        /*  {
              ArrayList objs = EnemyMgr.ins.GetEnemys();
              float minDis = float.MaxValue;

              foreach (Enemy h in objs)
              {//找出一个最近的玩家 作为锁定目标
                  if (team == h.team) continue;
                  float dis = h.ClaculateDistance(x, y);
                  if (dis < minDis)
                  {
                      target = h;
                      minDis = dis;
                  }
              }
          }*/
        {
            ArrayList objs = HeroMgr.ins.GetHeros();
            float minDis = float.MaxValue;

            foreach (Entity h in objs)
            {//找出一个最近的玩家 作为锁定目标

                ///   if (team == h.team) continue;
                float dis = h.ClaculateDistance(x, y);
                if (dis < minDis)
                {
                    target = h;
                    minDis = dis;
                }
            }
        }
    }

    public virtual void AI_AttackTarget()
    {
        if (cd.IsMax())
        {
            atk = true;
            cd.Reset();
        }
    }


    public override void InitInfo()
    {
        this.prefabsName = "Prefabs/Customs/500017";
        this.skin = "default";
        ani_stand = "rest";
        ani_atk1 = "rest";
        attackingAnimationName = ani_atk1;

        bulleClassName_atk1 = "BulletConfig";//普通攻击 1段  的子弹名字
        bulleClassName_s1 = "Bullet221_0"; // 1 号技能 子弹名字


        BulletConfigInfo info = BulletConfigInfo.Create();
        info.plistAnimation = "hd/roles/role_2/bullet/role_2_bul_2001/role_2_bul_2001.plist";
        info.distance = 10f;
        info.distance_atk = 1f;
        ///    info.lastTime = 10;
        info.oneHitTimes = 0xfffff;
        info.isHitDestory = true;
        info.collider_size = new Vector3(2f, 2f, 2f);
        info.launch_delta_xyz = new Vector3(0f, 0f, 0f);
        ///  info.AddBuffer("BufferPoison");
        info._OnLaunch = (Bullet bb, object user) =>
        {
            BulletConfig conf = bb as BulletConfig;
            ///   info.AddHitTarget(target);
            if (this.x > target.x)
            {
                bb.flipX = -1;
            }
            else
            {
                bb.flipX = 1;
            }
            info.dir_2d = Utils.GetAngle(this, target);
        };

        this.bullet_atk1_info = info;
        this.atk_range = 8.0f;
        scale = 0.8f;
    }

}

/// <summary>
/// 充能防御塔
/// </summary>
public class DefendTower : Tower
{
    public int MAX_POWER_LEVEL = 3;
    public int power_level = 0;//当前能量等级
    public Counter tick_power = Counter.Create(40);//充能Tick
    public bool IsMaxPowerLevel()
    {
        return MAX_POWER_LEVEL <= power_level;
    }
    public override void InitInfo()
    {
        this.prefabsName = "Prefabs/Customs/500017";
        this.skin = "default";
        ani_stand = "rest";
        ani_atk1 = "rest";
        attackingAnimationName = ani_atk1;

        bulleClassName_atk1 = "BulletConfig";//普通攻击 1段  的子弹名字
        bulleClassName_s1 = "Bullet221_0"; // 1 号技能 子弹名字


        BulletConfigInfo info = BulletConfigInfo.Create();
        info.plistAnimation = "hd/roles/role_2/bullet/role_2_bul_2001/role_2_bul_2001.plist";
        info.distance = 10f;
        info.distance_atk = 1f;
        ///    info.lastTime = 10;
        info.oneHitTimes = 0xfffff;
        info.isHitDestory = true;
        info.collider_size = new Vector3(2f, 2f, 2f);
        info.launch_delta_xyz = new Vector3(0f, 0f, 0f);
        //  info.AddBuffer("BufferPoison");
        info._OnLaunch = (Bullet bb, object user) =>
        {
            BulletConfig conf = bb as BulletConfig;
            info.AddHitTarget(target);
            if (this.x > target.x)
            {
                bb.flipX = -1;
            }
            else
            {
                bb.flipX = 1;
            }
            info.dir_2d = Utils.GetAngle(this, target);
        };

        this.bullet_atk1_info = info;
        this.atk_range = 8.0f;
        scale = 0.8f;
        power_level = 0;
    }
    public override bool Init()
    {
        base.Init();
        MAX_POWER_LEVEL = DefendTowerData.ins.power_level_max;
        this.hp = DefendTowerData.ins.hp;
        this.current_hp = this.hp;

        this.damage = DefendTowerData.ins.damage;
        if (DefendTowerData.ins.faceto_left)
        {
            this.flipX = 1f;
        }
        else
        {
            this.flipX = -1f;
        }
        return true;
    }
    public override void OnEnter()
    {
        MAX_POWER_LEVEL = DefendTowerData.ins.power_level_max;
        tick_power.SetMax(DefendTowerData.ins.cd_power_level);
        power_level = DefendTowerData.ins.power_level_init;
    }
    public override void UpdateMS()
    {
        Hero self = HeroMgr.ins.self;
        if (self.level > this.level && DefendTowerData.ins.levels.Length>=this.level)
        {
            this.level++;
            this.hp = DefendTowerData.ins.levels[this.level-1].hp;
            this.current_hp = this.hp;
            this.damage = DefendTowerData.ins.levels[this.level-1].damage;
        }
        if (power_level > 0)
        {
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_SHOW_TOWER_PANEL);
        }
        else
        {
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_HIDE_TOWER_PANEL);
        }
        //充能
        if (tick_power.Tick())
        {

        }
        else
        {
            tick_power.Reset();
            if (power_level < MAX_POWER_LEVEL)
            {
                ++power_level;
                //  EventDispatcher.ins.PostEvent(Events.ID_BATTLE_SHOW_TOWER_PANEL);
            }
            else if (power_level == MAX_POWER_LEVEL)
            {
                //  EventDispatcher.ins.PostEvent(Events.ID_BATTLE_SHOW_TOWER_PANEL);
            }
        }
        base.UpdateMS();
    }
    public void Launch()
    {
        this.Shoot();
    }
    private void Shoot()
    {
        if (power_level > 0)
        {
            power_level--;
        }
        else
        {
            //  EventDispatcher.ins.PostEvent(Events.ID_BATTLE_HIDE_TOWER_PANEL);
            return;
        }
        if (power_level <= 0)
        {
            //  EventDispatcher.ins.PostEvent(Events.ID_BATTLE_HIDE_TOWER_PANEL);
        }
        BulletConfigInfo info = BulletConfigInfo.Create();

        info.frameDelay = 4;

        info.distance_atk = 1.5f;
        info.number = 0xffffff;
        info.isHitDestory = false;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = "hd/pet/pet_251/bullet/pet_251_bul_214023/pet_251_bul_214023.plist";
        info.plistAnimation = DefendTowerData.ins.animation_name;

        //  info.lastTime = 2;
        info.scale_x = 2f;
        info.scale_y = 2f;
        info.validTimes = 0xffffff;
        info.distance = DefendTowerData.ins.distance;
        info.launch_delta_xyz.x = 0f;// Skill62_3_Data.ins.delta_xyz.x;// 1.5f;
        info.launch_delta_xyz.y = 0f;// Skill62_3_Data.ins.delta_xyz.y;// -0.2f;
        info.launch_delta_xyz.z = 0f;// Skill62_3_Data.ins.delta_xyz.z;// -0.2f;
        info.launch_delta_xyz = DefendTowerData.ins.delta_xy;
        info.damage_ratio = DefendTowerData.ins.damage_ratio;
        info.collider_size = DefendTowerData.ins.collider_size;
        info.collider_type = ColliderType.Box;
        info.scale_x = DefendTowerData.ins.scale.x;
        info.scale_y = DefendTowerData.ins.scale.y;

        BulletMgr.Create(this, "BulletConfig", info);
    }
    public override void AI_UpdateMSWithAI()
    {
        //关闭AI
    }
}