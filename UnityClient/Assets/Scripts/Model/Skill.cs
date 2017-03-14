/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
/*
public class SkillStack : Model 
{
    public Entity Target=null;

    public override void UpdateMS()
    {
 
    }
    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
*/


/// <summary>
/// Skill栈（单行状态），负责维护每个Skill
/// </summary>
public sealed class SkillStack : GAObject
{

    public static SkillStack Create()
    {
        SkillStack ret = new SkillStack();
        ret.Init();
        return ret;
    }
    private SkillStack()
    {

    }

    public void PushSingleSkill(SkillBase s)
    {//单行状态机
        s.stack = this;
        s.Target = this.host;
        s.Init();
        stacks.Push(s);

    }
    public void PopSingleSkill()
    {//单行状态机
        SkillBase s = stacks.Pop() as SkillBase;//
        s.OnExit();
        //  s.stack = null;
    }

    public override void UpdateMS()
    {
        if (this.enable == false) return;

        SkillBase s = stacks.Peek() as SkillBase;
        if (s.Enable)
        {
            s.UpdateMS();
        }
        else
        {
            s.UpdateMSIdle();
        }
    }
    public void OnSpineCompolete()
    {
        SkillBase s = stacks.Peek() as SkillBase;
        if (s.Enable)
        {
            s.OnSpineCompolete();
        }
    }

    public void OnPush()
    {
        SkillBase s = stacks.Peek() as SkillBase;

        s.OnPush();

    }


    public override void OnEvent(string what, object userData)
    {
        if (this.enable == false) return;
        if (this.pause) return;
        GAObject s = stacks.Peek() as GAObject;
        s.OnEvent(what, userData);

    }

    public override void OnEvent(int what, object userData)
    {
        if (this.enable == false) return;
        if (this.pause) return;
        SkillBase s = stacks.Peek() as SkillBase;

        if ((userData as AttackInfo).target == this.host)
        {
            s.OnInterrupted(userData as AttackInfo);
        }

        s.OnEvent(what, userData);

    }

    public void AddEventListener(string what)
    {
        EventDispatcher.ins.AddEventListener(this, what);
    }
    public void AddEventListener(int what)
    {
        EventDispatcher.ins.AddEventListener(this, what);
    }


    public void AddLocalEventListener(string what)
    {
        host.eventDispatcher.AddEventListener(this, what);
    }
    public void AddLocalEventListener(int what)
    {
        host.eventDispatcher.AddEventListener(this, what);
    }


    public void SetEnable(bool b)
    {
        this.enable = b;
    }
    public override void OnEnter()
    {
        /*  SkillBase s = new Skill_1();
          s.Target = this.host;
          this.PushSingleSkill(s);
          */
        EventDispatcher.ins.AddEventListener(this, Events.ID_BATTLE_ENTITY_BEFORE_TAKEATTACKED);

    }
    private bool enable = true;

    Stack stacks = new Stack();
    public SkillBase _current_state = null;
    public bool pause = false;
    public Entity host = null;
    public StateBase parent = null;
}


public class SkillBase : Model
{

    public Entity Target = null; // reference
    public SkillStack stack = null;//reference
    public virtual string GetName() { return "SkillBase"; }
    public virtual string GetAnimationName() { return ""; }
    public bool Enable = false;

    public virtual void UpdateMSIdle()
    {

    }

    //---------helper function
    protected void PauseAll()
    {
        Target.machine.PauseAllStack();
        this.stack.parent.stack.Resume();
    }
    protected void ResumeAll()
    {
        Target.machine.ResumeAllStack();
    }
    public void SetDisable()
    {
        this.Enable = false;
    }
    public void SetEnable()
    {
        this.Enable = true;

    }

    //-----------event
    /// <summary>
    /// 被打断事件
    /// </summary>
    public virtual void OnInterrupted(AttackInfo info)
    {

    }
    public virtual void OnInterrupted(GAObject what)
    {

    }
    public virtual void OnInterrupted(object what)
    {

    }

    public virtual void OnSpineCompolete()
    {

    }
    public virtual void OnPush()
    {

    }
}





//-----------------------------------------------------------------------hero 2



/// <summary>
///可打断 蓄力1 秒 后，释放一条冰龙 造成100点伤害
/// </summary>
public class Skill2_1 : SkillBase
{
    public override void OnEnter()
    {
        tick.Reset();
        tick1.Reset();
        this.PauseAll();
        is_launch_bullet = false;
        is_wait_done = false;
        Target.isAttacking = true;
        this.Enable = true;
        //开始蓄力
        Target.attackingAnimationName = "2200_0";
        Target.is_spine_loop = false;

        RunState run = (Target.machine.GetState<RunState>() as RunState);
        run.stack.Resume();
        run.DisableMove();
    }
    public override void UpdateMS()
    {
        if (is_launch_bullet == false)
        {
            if (tick.Tick())
            {
                //蓄力中

                return;
            }
        }
        //蓄力结束
        is_wait_done = true;

        //   if (is_wait_done) return;
        ///  if (Target.attackingAnimationName == "2200_1") return;

        Target.attackingAnimationName = "2200_1";
        //    Target.is_spine_loop = true;


        if (is_launch_bullet)
        {
            if (tick1.Tick()) return;

            this.OnExit();

        }
    }

    public override void OnSpineCompolete()
    {
        if (is_wait_done == false) return;

        Debug.Log("释放冰龙");
        // this.Enable = false;
        //  Target.isAttacking = false;
        is_launch_bullet = true;
        ///   BulletMgr.Create(this.Target, Target.bulleClassName_atk1, Target.bullet_atk1_info);


        BulletConfigInfo info = BulletConfigInfo.Create();
        info.plistAnimation = "hd/roles/role_2/bullet/role_2_bul_2201/role_2_bul_2201.plist";
        info.number = 0xffff;
        info.distance = 10.0f;
        info.validTimes = 0xffff;
        info.isHitDestory = false;

        Bullet b = BulletMgr.Create(this.Target, "BulletConfig", info);
        b.y = Target.GetRealY() + 0.0f;

    }
    public override void OnExit()
    {
        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        Target.machine.ResumeAllStack();
        RunState run = (Target.machine.GetState<RunState>() as RunState);
        run.EnableMove();
    }
    public override void OnPush()
    {
        if (Target.isAttacking) return;

        if (this.Enable == false)
        {
            this.OnEnter();
        }
    }
    public override void OnInterrupted(AttackInfo info)
    {

        this.OnExit();

    }
    private bool is_launch_bullet = false;
    private bool is_wait_done = false;
    Counter tick = Counter.Create(40);
    Counter tick1 = Counter.Create(5);


}





/// <summary>
///可被打断 角色施法 出一个伤害移动的伤害范围， 再次按下触发 一颗雷从天而降 造成大量伤害
/// </summary>
public class Skill2_2 : SkillBase
{
    public override void OnEnter()
    {
        tick.Reset();
        tick1.Reset();
        Target.machine.PauseAllStack();
        this.stack.parent.stack.Resume();
        is_shifa = true;

        Target.isAttacking = true;
        this.Enable = true;
        //开始施法
        Target.attackingAnimationName = "2240_0";

        b_shifa = null;

        BulletConfigInfo info = BulletConfigInfo.Create();
        info.plistAnimation = "hd/roles/role_2/bullet/role_2_bul_2241/role_2_bul_2241.plist";
        info.distance_atk = 0;
        info.distance = 7;
        info.speed *= 0.5f;
        info.number = 0;
        info.launch_delta_xy.y = Target.height;
        b_shifa = BulletMgr.Create(this.Target, "BulletConfig", info);
        b_shifa.y = Target.height;



    }
    public override void UpdateMS()
    {
        if (is_shifa)
        {
            if (tick.Tick())
            {
                //施法中
                Target.y += 0.03f;
                return;
            }
            this.OnExit();
        }
    }

    public override void OnSpineCompolete()
    {
        if (is_shifa) return;

        Debug.Log("释放伤害");

        BulletConfigInfo info = BulletConfigInfo.Create();
        info.plistAnimation = "hd/roles/role_5/bullet/role_5_bul_5246/role_5_bul_5246.plist";
        info.distance_atk = 2.0f;
        info.distance = 0;
        info.speed *= 0.0f;
        info.number = 999;
        info.lastTime = 20;
        ///   info.deltaTime = 15;
        info.launch_delta_xy.x = b_shifa.x;
        info.launch_delta_xy.y = b_shifa.y;
        info.isHitDestory = false;
        Bullet b = BulletMgr.Create(this.Target, "BulletConfig", info);
        b.y = b_shifa.y;
        b.x = b_shifa.x;
        this.OnExit();
    }
    public override void OnExit()
    {
        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        Target.machine.ResumeAllStack();
        this.is_shifa = true;
        b_shifa.SetInValid();

    }
    public override void OnPush()
    {
        if (Target.isStand == false) return;

        if (this.Enable == false && Target.isAttacking == false)
        {
            this.OnEnter();
        }
        else if (this.Enable == true && is_shifa)
        {
            is_shifa = false;
            Target.attackingAnimationName = "2240_1";
            b_shifa.speed = 0.0f;
        }
    }
    public override void OnInterrupted(AttackInfo info)
    {

        this.OnExit();

    }

    private bool is_shifa = true;
    Counter tick = Counter.Create(100);
    Counter tick1 = Counter.Create(5);
    Bullet b_shifa = null;

}





/// <summary>
///可打断 紧箍咒 范围内的额敌人 击飞并且吸到紧箍咒上方
/// </summary>
public class Skill2_3 : SkillBase
{
    Bullet bullet = null;
    public override void OnEnter()
    {

        Target.machine.PauseAllStack();
        this.stack.parent.stack.Resume();

        Target.isAttacking = true;
        this.Enable = true;
        //开始蓄力
        Target.attackingAnimationName = "2121";



        BulletConfigInfo info = BulletConfigInfo.Create();
        info.plistAnimation = "hd/roles/role_2/bullet/role_2_bul_2121/role_2_bul_2121.plist";
        info.distance = 0.0f;
        info.distance_atk = 0;
        info.speed = 0.0f;
        info.number = 0;
        info.lastTime = 30;
        info.isHitDestory = false;
        bullet = BulletMgr.Create(this.Target, "BulletConfig", info);
        if (Target.flipX < 0)
        {
            bullet.x = Target.x + 2.0f;
        }
        else
        {
            bullet.x = Target.x - 2.0f;

        }
        bullet.y = Target.height + Target.y + 0.3f;
        delta = Target.GetRealY() + 4.0f;

        {
            ArrayList list = EnemyMgr.ins.GetEnemys();
            foreach (Entity e in list)
            {
                if (e.ClaculateDistance(bullet.x, bullet.y) < 2.0f)
                {
                    e.machine.GetState<FallState>().stack.Pause();
                    this.list.Add(e);
                }
            }
        }

        {
            ArrayList list = HeroMgr.ins.GetHeros();
            foreach (Entity e in list)
            {
                if (e.team == Target.team) continue;
                if (e.ClaculateDistance(bullet.x, bullet.y) < 2.0f)
                {
                    e.machine.GetState<FallState>().stack.Pause();
                    this.list.Add(e);
                }
            }
        }
    }
    public override void UpdateMS()
    {
        if (current > delta) return;

        bullet.scale_x -= 0.04f;
        bullet.scale_y = bullet.scale_x;

        current += 0.03f;
        foreach (Entity e in list)
        {
            e.y += 0.15f;

            if (Mathf.Abs(e.x - bullet.x) > 0.05f)
            {
                if (e.x > bullet.x)
                {
                    //敌人在右边
                    e.x -= 0.12f;
                }
                else
                {
                    e.x += 0.12f;
                }
            }
        }
    }

    public override void OnSpineCompolete()
    {

        Debug.Log("释放冰龙");

        this.OnExit();


    }
    public override void OnExit()
    {
        Target.isAttacking = false;
        current = 0.0f;
        Target.machine.ResumeAllStack();
        foreach (Entity e in list)
        {
            e.machine.GetState<FallState>().stack.Resume();
        }
        bullet.SetInValid();
        list.Clear();
        this.Enable = false;
    }
    public override void OnPush()
    {
        if (Target.isStand == false) return;
        if (Target.isAttacking) return;
        if (this.Enable) return;

        this.OnEnter();

    }
    public override void OnInterrupted(AttackInfo info)
    {

        this.OnExit();

    }
    private float delta = 0.0f;
    private float current = 0.0f;
    ArrayList list = new ArrayList();
}



/// <summary>
///可打断 回血
/// </summary>
public class Skill2_4 : SkillBase
{
    public override void OnEnter()
    {

        this.PauseAll();
        Target.isAttacking = true;
        this.Enable = true;
        //开始蓄力
        Target.attackingAnimationName = "2110";
        BulletMgr.Create<Bullet2_1>(Target);
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
        this.ResumeAll();
    }
    public override void OnPush()
    {
        if (Target.isStand == false) return;
        if (Target.isAttacking) return;
        if (this.Enable) return;

        this.OnEnter();

    }
    public override void OnInterrupted(AttackInfo info)
    {

        this.OnExit();

    }
}





/// <summary>
///可被打断  造成大量范围伤害 并且减速 80
/// </summary>
public class Skill2_5 : SkillBase
{
    public override void OnEnter()
    {
        this.PauseAll();

        Target.isAttacking = true;
        this.Enable = true;
        Target.attackingAnimationName = "2210";
    }
    public override void UpdateMS()
    {

    }

    public override void OnSpineCompolete()
    {
        Debug.Log("launch ");
        for (float i = -2.0f; i < 2.5f; i += 0.5f)
        {
            if (Mathf.Abs(i) < 0.01f) continue;
            BulletConfigInfo info = BulletConfigInfo.Create();
            info.plistAnimation = "hd/roles/role_2/bullet/role_2_bul_2211/role_2_bul_2211.plist";
            info.distance_atk = 0.5f;
            info.distance = 0;
            info.speed = 0.0f;
            info.number = 0xfff;
            info.lastTime = 16;
            info.isHitDestory = false;

            BufferSpeedSlow buffer = BufferMgr.CreateHelper<BufferSpeedSlow>(Target);
            buffer.id = 0xef1;
            buffer.percent = 80;
            info.AddBuffer(buffer);
            var b = BulletMgr.Create(this.Target, "BulletConfig", info);
            b.y = Target.GetRealY();
            b.x = Target.x + i;
        }

        this.OnExit();
    }
    public override void OnExit()
    {
        this.Enable = false;
        Target.isAttacking = false;

        this.ResumeAll();
    }
    public override void OnPush()
    {
        if (Target.isStand == false) return;
        if (Target.isAttacking) return;
        if (this.Enable) return;

        this.OnEnter();
    }
    public override void OnInterrupted(AttackInfo info)
    {

        this.OnExit();

    }

}




//-------------------------------------------------------------------------------- hero 6
/// <summary>
/// 不可打断 第一段 跳起来打  
/// </summary>
public class Skill6_1 : SkillBase
{
    public override void OnEnter()
    {

        this.PauseAll();
        Target.isAttacking = true;
        this.Enable = true;
        Target.attackingAnimationName = "6110_0";
        //起跳
        Target.machine.GetState<JumpState>().Resume();
        Target.machine.GetState<StandState>().Resume();
        Target.machine.GetState<FallState>().Resume();
        Target.jump = true;
        Target.is_spine_loop = false;
    }
    public override void UpdateMS()
    {
        if (count == 0)
        {
            //  if(tick1.Tick())
            {
                Target.x_auto += 0.1f * -Target.flipX;
                return;
            }

        }
        else if (count == 1)
        {

            if (tick.Tick() && Target.stand == false)
            {
                Target.x_auto += 0.2f * -Target.flipX;
                return;
            }

            count++;
            Target.attackingAnimationName = "6110_3";

            this.Shoot();

        }
        else if (count == 2)
        {
            Target.x_auto += 0.02f * -Target.flipX;
        }
    }
    private void Shoot()
    {

        BulletConfigInfo info = BulletConfigInfo.Create();

        info.AddBuffer("BufferHitBack");

        info.launch_delta_xy.x = 1.5f;
        info.launch_delta_xy.y = 0f;
        info.frameDelay = 4;
        info.distance_atk = 2.0f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6111/role_6_bul_6111.plist";
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 15;
        info.scale_x = 2f;
        info.scale_y = 2f;
        BulletMgr.Create(this.Target, "BulletConfig", info);

    }
    public override void OnSpineCompolete()
    {
        if (count == 0)
        {
            //进入阶段二 下跳
            ++count;

            Target.attackingAnimationName = "6110_2";
        }
        else if (count == 1)
        {

        }
        else/// if (count == 2)
        {
            this.OnExit();
            this.stack.PushSingleSkill(new Skill6_1_2());

        }

    }
    public override void OnExit()
    {
        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        count = 0;
        this.ResumeAll();

    }
    public override void OnPush()
    {
        if (Target.isStand == false) return;
        if (Target.isAttacking) return;
        if (this.Enable) return;

        this.OnEnter();

    }
    public override void OnInterrupted(AttackInfo info)
    {

    }
    bool is_move_complete = false;
    int count = 0;
    Counter tick = Counter.Create(15);//位移
    Counter tick1 = Counter.Create(15);// 起跳
}


/// <summary>
/// 6_1 的第二段连招  反手猛的一招 击飞很远 不可打断
/// </summary>

public class Skill6_1_2 : SkillBase
{
    public override void OnEnter()
    {

        this.PauseAll();
        Target.isAttacking = true;
        this.Enable = true;
        Target.attackingAnimationName = "6010";
        this.Shoot();

    }
    public override void UpdateMS()
    {

    }
    private void Shoot()
    {

        BulletConfigInfo info = BulletConfigInfo.Create();

        info.AddBuffer("BufferHitBack");

        info.launch_delta_xy.x = 1.5f;
        info.launch_delta_xy.y = -0.2f;
        info.frameDelay = 4;
        info.distance_atk = 2.0f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6112/role_6_bul_6112.plist";
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 15;
        info.scale_x = 2f;
        info.scale_y = 2f;
        BulletMgr.Create(this.Target, "BulletConfig", info);

    }
    public override void OnSpineCompolete()
    {
        this.stack.PopSingleSkill();

    }
    public override void OnExit()
    {
        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        this.ResumeAll();

    }
    public override void OnPush()
    {
        if (Target.isStand == false) return;
        if (Target.isAttacking) return;
        if (this.Enable) return;

        this.OnEnter();

    }
    public override void OnInterrupted(AttackInfo info)
    {

    }
}
