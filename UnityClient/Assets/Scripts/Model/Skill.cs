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
        SkillBase s = new Skill_1();
        s.Target = this.host;
        this.PushSingleSkill(s);

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
    public void SetDisable()
    {
        this.Enable = false;
    }
    public void SetEnable()
    {
        this.Enable = true;

    }
    public virtual void UpdateMSIdle()
    {

    }
    /// <summary>
    /// 被打断事件
    /// </summary>
    public virtual void OnInterrupted(AttackInfo info)
    {

    }
    public virtual void OnSpineCompolete()
    {

    }
    public virtual void OnPush()
    {

    }
}









/// <summary>
///可打断 蓄力1 秒 后，释放一条冰龙 造成100点伤害
/// </summary>
public class Skill_1 : SkillBase
{
    public override void OnEnter()
    {
        tick.Reset();
        tick1.Reset();
        Target.machine.PauseAllStack();
        this.stack.parent.stack.Resume();
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
        BulletMgr.Create(this.Target, "BulletConfig", info);

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
public class Skill_2 : SkillBase
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
    Counter tick = Counter.Create(80);
    Counter tick1 = Counter.Create(5);
    Bullet b_shifa = null;

}

