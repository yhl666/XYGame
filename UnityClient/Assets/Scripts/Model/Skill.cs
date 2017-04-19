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
    /// <summary>
    /// who 打断请求发起者
    /// </summary>
    /// <param name="who"></param>
    public bool ProcessOnInterrupted(SkillBase who)
    {
        SkillBase s = stacks.Peek() as SkillBase;//
        if (s.Enable == false) return false;
        if (ConfigTables.SkillConflict.IsConflict(who.GetName(), s.GetName()))
        {
            bool ret = s.OnInterrupted(who);//向被打断者 发起打断请求
            if (ret)
            {//打断成功
                who.OnAcceptInterrupted(s);
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 强制去打断 可用于状态切换
    /// </summary>
    /// <param name="who"></param>
    /// <returns>允许强制打断返回true</returns>
    public bool ProcessOnInterruptedForce(SkillBase who)
    {
        SkillBase s = stacks.Peek() as SkillBase;//
        if (s.Enable == false) return true;

        bool ret = s.OnInterruptedForce(who);//向被打断者 发起打断请求
        return ret;
    }
    public void ProcessChangeSkillGroupOut(SkillBase who)
    {
        SkillBase s = stacks.Peek() as SkillBase;//

        s.OnChangeSkillGroupOut(who);// 技能组切出消息  
    }
    public void ProcessChangeSkillGroupIn(SkillBase who)
    {
        SkillBase s = stacks.Peek() as SkillBase;//

        s.OnChangeSkillGroupIn(who);//   技能组切入消息
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
    public SkillBase TopSkill()
    {
        return stacks.Peek() as SkillBase;
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
    public void OnSpineComplete()
    {
        SkillBase s = stacks.Peek() as SkillBase;
        if (s.Enable)
        {
            s.OnSpineComplete();
        }
    }

    public void OnPush()
    {
        SkillBase s = stacks.Peek() as SkillBase;

        s.OnPush();

    }

    public void PushLevelUp()
    {
        SkillBase s = stacks.Peek() as SkillBase;

        s.PushLevelUp();
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
    public void PushOnInterrupted(SkillBase who)
    {
        this.parent.PushOnInterrupted(who);
    }
    /// <summary>
    /// 强制取消所有技能
    /// </summary>
    /// <param name="who"></param>
    public void PushOnInterruptedForce(SkillBase who)
    {
        this.parent.PushOnInterruptedForce(who);
    }
    /// <summary>
    /// 请求切换技能组
    /// </summary>
    /// <param name="who"></param>
    public void PushChangeSkillGroup(SkillBase who)
    {
        this.parent.PushChangeSkillGroup(who);
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
    public SkillState parent = null;
}


public class SkillBase : Model
{
    public Entity Target = null; // reference
    public SkillStack stack = null;//reference
    public int level = 1;
    public Counter cd = Counter.Create();
    public Counter GetCd()
    {
        return cd;
    }
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

    //-----------异步 event
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
    /// <summary>
    /// 响应被其他技能打断事件
    /// 返回true表示打断成功（允许打断的情况下要处理自己的OnExit）
    /// 返回false 表示打断失败
    /// 可用于复杂逻辑下的打断请求处理
    /// </summary>
    /// <param name="what"></param>
    /// <returns></returns>
    public virtual bool OnInterrupted(SkillBase what)
    {
        return false;
    }
    public virtual bool OnInterruptedForce(SkillBase what)
    {//强制取消，可用于比如切换取消
        this.OnExit();
        return true;
    }
    /// <summary>
    /// 技能组切出 事件
    /// Enable=false 也会受到
    /// </summary>
    /// <param name="who"></param>
    public virtual void OnChangeSkillGroupOut(SkillBase who)
    {

    }
    public virtual void OnChangeSkillGroupIn(SkillBase who)
    {

    }
    /// <summary>
    /// 动画完成回调
    /// </summary>
    public virtual void OnSpineComplete()
    {

    }
    /// <summary>
    /// 技能绑定的按钮被点击后触发
    /// </summary>
    public virtual void OnPush()
    {

    }
    /// <summary>
    /// 请求升级
    /// </summary>
    public bool PushLevelUp()
    {
        this.OnLevelUp(this.level + 1);// 默认升级规则为一级一级提升
        ++this.level;
        return true;
    }
    /// <summary>
    /// 技能升级事件 
    /// </summary>
    /// <param name="target_level">this.level当前等级 ，target_level 下一段等级</param>
    public virtual void OnLevelUp(int target_level)
    {

    }
    /// <summary>
    ///  打断发起者  打断成功后的回调
    /// </summary>
    /// <param name="who"> 被打断的skill</param>
    public virtual void OnAcceptInterrupted(SkillBase who)
    {

    }
    /// <summary>
    ///技能需要打断的时候调用此接口
    ///发起打断事件
    ///
    /// 代理方式处理
    /// </summary>
    /// <param name="who"></param>
    public void PushOnInterrupted()
    {
        this.stack.PushOnInterrupted(this);
    }
    /// <summary>
    /// 请求强制打断所有技能
    /// 
    /// 代理方式处理
    /// </summary>
    public void PushOnInterruptedForce()
    {
        this.stack.PushOnInterruptedForce(this);
    }
    /// <summary>
    /// 请求 切换 技能组
    /// 
    /// 代理方式处理
    /// </summary>
    public void PushChangeSkillGroup()
    {
        this.stack.PushChangeSkillGroup(this);
    }
    /// <summary>
    /// 请求打断普通攻击
    /// 
    /// 消息方式处理
    /// </summary>
    public void PushOnInterruptAttackSate()
    {//消息形式 发起打断普通攻击
        EventDispatcher.ins.PostEvent(Events.ID_BATTLE_PUSH_ONINTERRUPT_ATTACKSTATE, this);
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

        RunXZState run = (Target.machine.GetState<RunXZState>() as RunXZState);
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

    public override void OnSpineComplete()
    {
        if (is_wait_done == false) return;

        //      Debug.Log("释放冰龙");
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
        RunXZState run = (Target.machine.GetState<RunXZState>() as RunXZState);
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
        info.launch_delta_xyz.y = Target.height;
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

    public override void OnSpineComplete()
    {
        if (is_shifa) return;

        BulletConfigInfo info = BulletConfigInfo.Create();
        info.plistAnimation = "hd/roles/role_5/bullet/role_5_bul_5246/role_5_bul_5246.plist";
        info.distance_atk = 2.0f;
        info.distance = 0;
        info.speed *= 0.0f;
        info.number = 999;
        info.lastTime = 20;
        ///   info.deltaTime = 15;
        info.launch_delta_xyz.x = b_shifa.x;
        info.launch_delta_xyz.y = b_shifa.y;
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

    public override void OnSpineComplete()
    {

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

    public override void OnSpineComplete()
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

    public override void OnSpineComplete()
    {
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

            if (tick.Tick() && Target.y > 0.0f) // 可提前触发
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

        info.launch_delta_xyz.x = 1.5f;
        info.launch_delta_xyz.y = -0.2f;
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
    public override void OnSpineComplete()
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

        info.launch_delta_xyz.x = 1.5f;
        info.launch_delta_xyz.y = -0.2f;
        info.frameDelay = 4;
        info.distance_atk = 2.0f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.damage_ratio = 1.5f;
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
    public override void OnSpineComplete()
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


/// <summary>
/// 不可打断  快速旋转左右移动 2S  内造成8 次hit伤害
/// </summary>
public class Skill6_2 : SkillBase
{
    Counter tick = Counter.Create(80);
    public override void OnEnter()
    {

        this.PauseAll();
        this.Target.machine.GetState<StandState>().Resume();
        this.Target.machine.GetState<FallState>().Resume();
        tick.Reset();
        RunXZState state = this.Target.machine.GetState<RunXZState>() as RunXZState;
        state.Resume();
        state.EnableWhenAttack();
        Target.isAttacking = true;
        this.Enable = true;
        Target.attackingAnimationName = "6100_2";
        this.Shoot();



        BufferSpeedSlow buffer = BufferMgr.CreateHelper<BufferSpeedSlow>(Target);
        buffer.id = 0xef2;
        buffer.percent = -130;
        buffer.time = Utils.fpsi * 5;
        Target.AddBuffer(buffer);
    }
    public override void UpdateMS()
    {
        if (tick.Tick())
        {
            b.x = Target.x;
            b.y = Target.GetRealY();
            if (b.IsInValid())
            {//每次Bullet持续时间为10 fps ， 为结束 继续释放bullet
                this.Shoot();
            }
            return;
        }
        this.OnExit();
    }
    private void Shoot()
    {
        BulletConfigInfo info = BulletConfigInfo.Create();

        info.AddBuffer("BufferHitBack");

        info.launch_delta_xyz.x = 1.5f;
        info.launch_delta_xyz.y = -0.2f;
        info.frameDelay = 4;
        info.distance_atk = 1.5f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.damage_ratio = 1.5f;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6122/role_6_bul_6122.plist";
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 10;
        info.scale_x = 2f;
        info.scale_y = 2f;
        b = BulletMgr.Create(this.Target, "BulletConfig", info);
    }
    public override void OnSpineComplete()
    {
        ///  this.stack.PopSingleSkill();

    }
    public override void OnExit()
    {
        b.SetInValid();
        this.Enable = false;
        Target.isAttacking = false;
        tick.Reset();
        Target.is_spine_loop = true;
        RunXZState state = this.Target.machine.GetState<RunXZState>() as RunXZState;
        state.DisableWhenAttak();
        this.ResumeAll();

        this.stack.PushSingleSkill(new Skill6_2_2());
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
    Bullet b = null;
}


/// <summary>
/// 6_2 的第二段连招  反手猛的一招 击飞很远 不可打断
/// </summary>

public class Skill6_2_2 : SkillBase
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

        info.launch_delta_xyz.x = 0.5f;
        info.launch_delta_xyz.y = 0f;
        info.frameDelay = 3;
        info.distance_atk = 2.0f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.damage_ratio = 1.5f;
        info.speed *= 1.5f;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6121/role_6_bul_6121.plist";
        /// info.rotate = 30.0f;
        info.distance = 2.0f;
        info.lastTime = 0;
        info.scale_x = 2f;
        info.scale_y = 2f;
        BulletMgr.Create(this.Target, "BulletConfig", info);

    }
    public override void OnSpineComplete()
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


/// <summary>
/// 不可打断  连砍2次
/// </summary>
public class Skill6_3 : SkillBase
{
    public override void OnEnter()
    {
        this.PauseAll();

        Target.isAttacking = true;
        this.Enable = true;
        count = 0;
        Target.attackingAnimationName = "6140_0";
        tick.Reset();
        this.Shoot();
        Target.is_spine_loop = true;

    }
    public override void UpdateMS()
    {
        if (tick.Tick())
        {
            return;
        }
        count++;
        if (count > 1) return;

        this.Shoot();

    }
    private void Shoot()
    {
        BulletConfigInfo info = BulletConfigInfo.Create();

        info.AddBuffer("BufferHitBack");

        info.launch_delta_xyz.x = 1.5f;
        info.launch_delta_xyz.y = -0.2f;
        info.frameDelay = 4;
        info.distance_atk = 1.5f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.damage_ratio = 1.5f;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6111/role_6_bul_6111.plist";
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 10;
        info.scale_x = 2f;
        info.scale_y = 2f;
        BulletMgr.Create(this.Target, "BulletConfig", info);
    }
    public override void OnSpineComplete()
    {
        ///  this.stack.PopSingleSkill();
        this.OnExit();
    }
    public override void OnExit()
    {
        this.Enable = false;
        Target.isAttacking = false;
        count = 0;
        tick.Reset();
        this.ResumeAll();

        this.stack.PushSingleSkill(new Skill6_3_2());
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
    int count = 0;
    Counter tick = Counter.Create(17);//释放第二次
}

/// <summary>
/// 6_3的第二段连招  反手猛的一招 击飞很远 不可打断
/// </summary>

public class Skill6_3_2 : SkillBase
{
    public override void OnEnter()
    {

        this.PauseAll();
        Target.isAttacking = true;
        this.Enable = true;
        Target.attackingAnimationName = "6140_1";
        this.Shoot();

    }
    public override void UpdateMS()
    {

    }
    private void Shoot()
    {

        BulletConfigInfo info = BulletConfigInfo.Create();

        info.AddBuffer("BufferHitBack");

        info.launch_delta_xyz.x = 0.5f;
        info.launch_delta_xyz.y = 0f;
        info.frameDelay = 3;
        info.distance_atk = 2.0f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.damage_ratio = 1.5f;
        info.speed *= 1.5f;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6143/role_6_bul_6143.plist";
        /// info.rotate = 30.0f;
        info.distance = 2.0f;
        info.lastTime = 0;
        info.scale_x = 1.5f;
        info.scale_y = 1.5f;
        BulletMgr.Create(this.Target, "BulletConfig", info);

    }
    public override void OnSpineComplete()
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





/// <summary>
/// 形态2 技能1
/// 人物动画6100_2，无特效，判定范围为人物边框，持续2秒，
/// 除切换取消和终结技取消外无法停止，释放期间可以移动(控制免疫)，并且移动速度增加30%，cd15s。
/// </summary>
public class Skill62_1_v1 : SkillBase
{
    public override string GetName()
    {
        return "Skill62_1";
    }
    Bullet b = null;
    //Counter cd = Counter.Create(Skill62_1_Data.ins.cd);
    Counter tick = Counter.Create(Skill62_1_Data.ins.last_time);
    Counter tick_cancel = Counter.Create(Skill62_1_Data.ins.cancel);

    public override void OnEnter()
    {
        cd.SetMax(Skill62_1_Data.ins.cd);
        cd.SetMax(400);
        tick.SetMax(Skill62_1_Data.ins.last_time);
        tick_cancel.SetMax(Skill62_1_Data.ins.cancel);

        cd.Reset();
        tick_cancel.Reset();
        this.PauseAll();
        this.Target.machine.GetState<StandState>().Resume();
        this.Target.machine.GetState<FallState>().Resume();
        tick.Reset();
        {
            RunXZState state = this.Target.machine.GetState<RunXZState>() as RunXZState;
            state.Resume();
            state.EnableWhenAttack();
        }
        {
            RunXZState state = this.Target.machine.GetState<RunXZState>() as RunXZState;
            state.Resume();
            state.EnableWhenAttack();
        }
        Target.isAttacking = true;
        this.Enable = true;
        Target.attackingAnimationName = Skill62_1_Data.ins.animation_name; // "6100_2";
        this.Shoot();


        {
            BufferSpeedSlow buffer = BufferMgr.CreateHelper<BufferSpeedSlow>(Target);
            buffer.id = 0xef2;
            buffer.percent = -Skill62_1_Data.ins.added_speed_percent;
            buffer.time = Skill62_1_Data.ins.last_time;
            Target.AddBuffer(buffer);
        }
        {
            BufferNegativeUnbeatable buffer = BufferMgr.CreateHelper<BufferNegativeUnbeatable>(Target);
            buffer.MAX_TICK = Skill62_1_Data.ins.last_time;
            Target.AddBuffer(buffer);
        }
    }
    public override void UpdateMS()
    {
        cd.Tick();
        tick_cancel.Tick();
        if (tick.Tick())
        {
            b.x = Target.x;
            b.y = Target.GetRealY();
            if (b.IsInValid())
            {//每次Bullet持续时间为10 fps ， 为结束 继续释放bullet
                this.Shoot();
            }
            return;
        }
        this.OnExit();
    }
    public override void UpdateMSIdle()
    {
        cd.Tick();
    }
    private void Shoot()
    {

        BulletConfigInfo info = BulletConfigInfo.Create();

        info.AddBuffer("BufferHitBack");

        info.launch_delta_xyz.x = 1.5f;
        info.launch_delta_xyz.y = -0.2f;
        info.frameDelay = 4;
        info.distance_atk = 1.5f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.damage_ratio = Skill62_1_Data.ins.damage_ratio;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6122/role_6_bul_6122.plist";
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 10;
        info.collider_size = Skill62_1_Data.ins.hit_rect;
        info.scale_x = 2f;
        info.scale_y = 2f;
        b = BulletMgr.Create(this.Target, "BulletConfig", info);
    }
    public override void OnSpineComplete()
    {
        ///  this.stack.PopSingleSkill();

    }
    public override void OnExit()
    {
        b.SetInValid();
        this.Enable = false;
        Target.isAttacking = false;
        tick.Reset();
        Target.is_spine_loop = true;
        RunXZState state = this.Target.machine.GetState<RunXZState>() as RunXZState;
        state.DisableWhenAttak();
        this.ResumeAll();

        /// this.stack.PushSingleSkill(new Skill6_2_2());


    }
    public override void OnAcceptInterrupted(SkillBase who)
    {
        if (this.Enable) return;
        this.OnEnter();
    }
    public override void OnPush()
    {
        if (cd.IsMax() == false) return;
        if (Target.isStand == false) return;
        if (this.Enable) return;

        if (Target.isAttacking)
        {
            this.PushOnInterruptAttackSate(); //强制打断 普通技能
        }
        if (Target.isAttacking)
        {
            this.PushOnInterrupted();
            return;
        }
        this.OnEnter();

    }
    public override bool Init()
    {
        //Debug.LogError("技能初始化执行");
        base.Init();
        cd.SetMax(Skill62_1_Data.ins.cd);
        cd.TickMax();
        return true;
    }
    public override bool OnInterrupted(SkillBase who)
    {
        if (this.Enable && tick_cancel.IsMax())
        {
            this.OnExit();
            return true;
        }
        return false;
    }

}

/// <summary>
/// 形态2 技能2
/// </summary>
public class Skill62_2_v1 : SkillBase
{
    public override string GetName()
    {
        return "Skill62_2";
    }
    Counter tick_cancel = Counter.Create(Skill62_2_Data.ins.cancel);
    //Counter cd = Counter.Create(Skill62_2_Data.ins.cd);
    Counter tick = Counter.Create(Skill62_2_Data.ins.tick_delay);
    bool has_shoot = false;
    public override void OnEnter()
    {
        tick_cancel.SetMax(Skill62_2_Data.ins.cancel);
        cd.SetMax(Skill62_2_Data.ins.cd);
        tick.SetMax(Skill62_2_Data.ins.tick_delay);
        tick_cancel.Reset();
        cd.Reset();
        this.PauseAll();
        tick.Reset();
        has_shoot = false;
        this.Target.machine.GetState<StandState>().Resume();
        this.Target.machine.GetState<FallState>().Resume();
        Target.isAttacking = true;
        this.Enable = true;
        Target.is_spine_loop = false;
        Target.attackingAnimationName = Skill62_2_Data.ins.animation_name;//"6130_1";
    }
    public override void UpdateMS()
    {
        tick_cancel.Tick();
        if (tick.Tick() == false && has_shoot == false)
        {
            this.Shoot();
            has_shoot = true;
        }
        if (cd.Tick())
        {
            return;
        }

    }
    public override void UpdateMSIdle()
    {
        cd.Tick();
    }
    private void Shoot()
    {

        BulletConfigInfo info = BulletConfigInfo.Create();

        info.AddBuffer("BufferHitFly");

        info.launch_delta_xyz.x = Skill62_2_Data.ins.delta_xyz.x;// 1.5f;
        info.launch_delta_xyz.y = Skill62_2_Data.ins.delta_xyz.y;// -0.2f;
        info.launch_delta_xyz.z = Skill62_2_Data.ins.delta_xyz.z;// -0.2f;

        info.frameDelay = 4;
        info.distance_atk = 1.5f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = Skill62_2_Data.ins.hit_animation_name;
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 10;
        info.scale_x = 2f;
        info.scale_y = 2f;
        info.damage_ratio = Skill62_2_Data.ins.damage_ratio;
        info.collider_size = Skill62_2_Data.ins.hit_rect;
        b = BulletMgr.Create(this.Target, "BulletConfig", info);
    }
    public override void OnSpineComplete()
    {
        ///  this.stack.PopSingleSkill();

        this.OnExit();
    }
    public override void OnExit()
    {
        if (b != null)
        {
            b.SetInValid();
            b = null;
        }
        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        this.ResumeAll();

        /// this.stack.PushSingleSkill(new Skill6_2_2())

    }
    public override bool Init()
    {
        base.Init();
        cd.SetMax(Skill62_2_Data.ins.cd);

        cd.TickMax();
        return true;
    }

    public override bool OnInterrupted(SkillBase who)
    {
        if (this.Enable && tick_cancel.IsMax())
        {
            this.OnExit();
            return true;
        }
        return false;
    }
    public override void OnAcceptInterrupted(SkillBase who)
    {
        if (this.Enable) return;
        this.OnEnter();
    }
    public override void OnPush()
    {
        if (cd.IsMax() == false) return;
        if (Target.isStand == false) return;
        if (this.Enable) return;
        if (Target.isAttacking)
        {
            this.PushOnInterruptAttackSate(); //强制打断 普通技能
        }
        if (Target.isAttacking)
        {
            this.PushOnInterrupted();
            return;
        }
        this.OnEnter();

    }
    Bullet b = null;
}


/// <summary>
/// 形态2 技能3
/// 起跳 在快速砍向 地面
/// </summary>
public class Skill62_3_v1 : SkillBase
{
    public override string GetName()
    {
        return "Skill62_3";
    }

    //Counter cd = Counter.Create(Skill62_3_Data.ins.cd);
    Counter tick1 = Counter.Create(Skill62_3_Data.ins.start_jump);
    Counter tick_cancel = Counter.Create(Skill62_3_Data.ins.cancel);
    bool has_shoot = false;
    public override void OnEnter()
    {
        cd.SetMax(Skill62_3_Data.ins.cd);
        tick1.SetMax(Skill62_3_Data.ins.start_jump);
        tick_cancel.SetMax(Skill62_3_Data.ins.cancel);
        tick_cancel.Reset();
        cd.Reset();
        this.PauseAll();
        tick1.Reset();
        jump_speed = Skill62_3_Data.ins.jump_speed;// DATA.DEFAULT_JUMP_SPEED * 1f;
        has_shoot = false;
        this.Target.machine.GetState<StandState>().Resume();
        this.Target.machine.GetState<FallState>().Resume();
        Target.isAttacking = true;
        this.Enable = true;
        Target.is_spine_loop = false;
        Target.attackingAnimationName = Skill62_3_Data.ins.animation_name;//// "6140_1";
    }

    private float jump_speed = Skill62_3_Data.ins.jump_speed;  // DATA.DEFAULT_JUMP_SPEED * 1f;
    public override void UpdateMS()
    {
        ///  Target.x_auto += Target.flipX * -0.05f;
        if (tick1.Tick())
        {//阶段1 起跳


            if (jump_speed <= 0.0f)
            {
                tick1.TickMax();

                BulletConfigInfo info = BulletConfigInfo.Create();

                /// info.AddBuffer("BufferHitFly");

                info.launch_delta_xyz.x = 1.5f;
                info.launch_delta_xyz.y = -0.2f;
                info.frameDelay = 3;
                info.distance_atk = 1.5f;
                info.number = 0xfff;
                info.isHitDestory = true;
                info.damage_ratio = 1.5f;
                info.oneHitTimes = 1;
                //  info.rotate = -120.0f;
                ///   info.plistAnimation = "hd/magic_weapons/bullet/bul_500502/bul_500502.plist";

                info.plistAnimation = "";
                /// info.rotate = 30.0f;
                info.distance = 0;
                info.lastTime = 10;
                info.scale_x = 2f;
                info.scale_y = 2f;

                Bullet b = BulletMgr.Create(this.Target, "BulletConfig", info);


            }
            else
            {
                //接入重力

                jump_speed -= 9.8f / 40.0f * 0.1f * Skill62_3_Data.ins.gravity_ratio;
                ///   current_height += jump_speed;
                this.Target.y += jump_speed;
            }
        }

        if (Target.y <= 0 && has_shoot == false)
        {
            this.Shoot();
            has_shoot = true;
        }

        if (cd.Tick())
        {
            return;
        }

    }
    public override void UpdateMSIdle()
    {
        cd.Tick();
    }
    private void Shoot()
    {
        BulletConfigInfo info = BulletConfigInfo.Create();

        info.AddBuffer("BufferHitFly");

        // info.launch_delta_xy.x = 1.5f;
        //  info.launch_delta_xy.y = -0.2f;
        info.frameDelay = 3;
        info.distance_atk = 1.5f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        ///   info.plistAnimation = "hd/magic_weapons/bullet/bul_500502/bul_500502.plist";

        //  info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6222/role_6_bul_6222.plist";
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 10;
        info.scale_x = 2f;
        info.scale_y = 2f;

        info.launch_delta_xyz.x = Skill62_3_Data.ins.delta_xyz.x;// 1.5f;
        info.launch_delta_xyz.y = Skill62_3_Data.ins.delta_xyz.y;// -0.2f;
        info.launch_delta_xyz.z = Skill62_3_Data.ins.delta_xyz.z;// -0.2f;

        info.plistAnimation = Skill62_3_Data.ins.hit_animation_name;
        info.damage_ratio = Skill62_3_Data.ins.damage_ratio;
        info.collider_size = Skill62_3_Data.ins.hit_rect;

        b = BulletMgr.Create(this.Target, "BulletConfig", info);
    }
    public override void OnSpineComplete()
    {
        ///  this.stack.PopSingleSkill();
        ///  this.Shoot();
        this.OnExit();
    }
    public override void OnExit()
    {

        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        this.ResumeAll();

        /// this.stack.PushSingleSkill(new Skill6_2_2())

    }
    public override void OnPush()
    {
        if (cd.IsMax() == false) return;
        if (this.Enable) return;

        ///  if (Target.isStand == false) return;
        if (Target.isAttacking)
        {
            this.PushOnInterruptAttackSate(); //强制打断 普通技能
        }
        this.OnEnter();
    }
    public override bool Init()
    {
        base.Init();
        cd.SetMax(Skill62_3_Data.ins.cd);
        cd.TickMax();
        cd.TickMax();
        return true;
    }

    public override void OnInterrupted(AttackInfo info)
    {

    }
    public override bool OnInterrupted(SkillBase who)
    {
        if (this.Enable && tick_cancel.IsMax())
        {
            this.OnExit();
            return true;
        }
        return false;
    }
    Bullet b = null;
}




/// <summary>
/// 形态2 技能1
/// 人物动画6100_2，无特效，判定范围为人物边框，持续2秒，
/// 除切换取消和终结技取消外无法停止，释放期间可以移动(控制免疫)，并且移动速度增加30%，cd15s。
/// </summary>
public class Skill62_1_v2 : SkillBase
{
    public override string GetName()
    {
        return "Skill62_1";
    }
    Bullet b = null;
    Counter tick = Counter.Create(Skill62_1_Data.ins.last_time);
    Counter tick_cancel = Counter.Create(Skill62_1_Data.ins.cancel);

    public override void OnLevelUp(int target_level)
    {

    }
    public override void OnEnter()
    {
        cd.SetMax(Skill62_1_Data.Get(level).cd);
        tick.SetMax(Skill62_1_Data.Get(level).last_time);
        tick_cancel.SetMax(Skill62_1_Data.Get(level).cancel);
        AudioMgr.ins.PostEvent(AudioEvents.Events.HERO_SKILL21,1);
        cd.Reset();
        tick_cancel.Reset();
        this.PauseAll();
        this.Target.machine.GetState<StandState>().Resume();
        this.Target.machine.GetState<FallState>().Resume();
        tick.Reset();
        {
            RunXZState state = this.Target.machine.GetState<RunXZState>() as RunXZState;
            state.Resume();
            state.EnableWhenAttack();
        }
        {
            RunXZState state = this.Target.machine.GetState<RunXZState>() as RunXZState;
            state.Resume();
            state.EnableWhenAttack();
        }
        Target.isAttacking = true;
        this.Enable = true;
        Target.attackingAnimationName = Skill62_1_Data.Get(level).animation_name; // "6100_2";
        this.Shoot();

        if (level >= 2)
        {
            BufferSpeedSlow buffer = BufferMgr.CreateHelper<BufferSpeedSlow>(Target);
            buffer.percent = -Skill62_1_Data.Get(level).added_speed_percent;
            buffer.time = Skill62_1_Data.Get(level).last_time;
            Target.AddBuffer(buffer);
        }
        {
            BufferNegativeUnbeatable buffer = BufferMgr.CreateHelper<BufferNegativeUnbeatable>(Target);
            buffer.MAX_TICK = Skill62_1_Data.Get(level).last_time;
            Target.AddBuffer(buffer);
        }
    }
    public override void UpdateMS()
    {
        cd.Tick();
        tick_cancel.Tick();
        if (tick.Tick())
        {
            b.x = Target.x;
            b.z = Target.z;
            if (b.IsInValid())
            {//每次Bullet持续时间为10 fps ， 为结束 继续释放bullet
                this.Shoot();
            }
            return;
        }
        this.OnExit();
    }
    public override void UpdateMSIdle()
    {
        cd.Tick();
    }
    private void Shoot()
    {

        BulletConfigInfo info = BulletConfigInfo.Create();

        info.launch_delta_xyz.x = 1.5f;
        info.launch_delta_xyz.y = -0.2f;
        info.frameDelay = 4;
        info.distance_atk = 1.5f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.damage_ratio = Skill62_1_Data.Get(level).damage_ratio;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6122/role_6_bul_6122.plist";
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 10;
        info.collider_size = Skill62_1_Data.Get(level).hit_rect;
        info.scale_x = 2f;
        info.scale_y = 2f;

        if (level >= 2)
        {
            info._OnTakeAttack = (Bullet b1, object userdata) =>
                {
                    Entity target = userdata as Entity;

                    BufferAttachTo buffer = BufferMgr.CreateHelper<BufferAttachTo>(Target);
                    buffer.to = Target;
                    buffer.distance = Skill62_1_Data.Get(level).attach_distance;
                    target.AddBuffer(buffer);
                    buffer.SetLastTime(Skill62_1_Data.Get(level).attach_last_time);
                };
        }
        b = BulletMgr.Create(this.Target, "BulletConfig", info);
    }
    public override void OnSpineComplete()
    {
        ///  this.stack.PopSingleSkill();

    }
    public override void OnExit()
    {
        b.SetInValid();
        this.Enable = false;
        Target.isAttacking = false;
        tick.Reset();
        Target.is_spine_loop = true;
        RunXZState state = this.Target.machine.GetState<RunXZState>() as RunXZState;
        state.DisableWhenAttak();
        this.ResumeAll();

        /// this.stack.PushSingleSkill(new Skill6_2_2());
    }

    public override bool OnInterrupted(SkillBase who)
    {
        if (this.Enable && tick_cancel.IsMax())
        {
            this.OnExit();
            return true;
        }
        return false;
    }
    public override void OnAcceptInterrupted(SkillBase who)
    {
        if (this.Enable) return;
        this.OnEnter();
    }
    public override void OnPush()
    {
        if (cd.IsMax() == false) return;
        if (Target.isStand == false && Target.isAttacking == false) return;
        if (this.Enable) return;
        if (Target.isAttacking)
        {
            this.PushOnInterruptAttackSate(); //强制打断 普通技能
        }
        if (Target.isAttacking)
        {
            this.PushOnInterrupted();
            return;
        }
        this.OnEnter();
    }
    public override bool Init()
    {
        base.Init();
        cd.SetMax(Skill62_1_Data.Get(level).cd);
        cd.TickMax();
        return true;
    }
}
/*
/// <summary>
/// 形态2 技能2
/// 
/// 人物动画6130_1 ，无特效，可以把敌人击飞至2倍角色高度，且滞空时间足够角色跳起衔接技能3，判定位置为人物剑的位置。Cd6s。
/// </summary>
public class Skill62_2 : SkillBase
{
    public override string GetName()
    {
        return "Skill62_2";
    }
    Counter tick_cancel = Counter.Create(Skill62_2_Data.ins.cancel);
    Counter cd = Counter.Create(Skill62_2_Data.ins.cd);
    Counter tick = Counter.Create(Skill62_2_Data.ins.tick_delay);
    bool has_shoot = false;
    public override void OnEnter()
    {
        tick_cancel.SetMax(Skill62_2_Data.ins.cancel);
        cd.SetMax(Skill62_2_Data.ins.cd);
        tick.SetMax(Skill62_2_Data.ins.tick_delay);
        tick_cancel.Reset();
        cd.Reset();
        this.PauseAll();
        tick.Reset();
        has_shoot = false;
        this.Target.machine.GetState<StandState>().Resume();
        this.Target.machine.GetState<FallState>().Resume();
        Target.isAttacking = true;
        this.Enable = true;
        Target.is_spine_loop = false;
        Target.attackingAnimationName = Skill62_2_Data.ins.animation_name;//"6130_1";
    }
    public override void UpdateMS()
    {
        tick_cancel.Tick();
        if (tick.Tick() == false && has_shoot == false)
        {
            this.Shoot();
            has_shoot = true;
        }
        if (cd.Tick())
        {
            return;
        }

    }
    public override void UpdateMSIdle()
    {
        cd.Tick();
    }
    private void Shoot()
    {

        BulletConfigInfo info = BulletConfigInfo.Create();

        info.AddBuffer("BufferHitFly");

        info.launch_delta_xyz.x = Skill62_2_Data.ins.delta_xyz.x;// 1.5f;
        info.launch_delta_xyz.y = Skill62_2_Data.ins.delta_xyz.y;// -0.2f;
        info.launch_delta_xyz.z = Skill62_2_Data.ins.delta_xyz.z;// -0.2f;

        info.frameDelay = 4;
        info.distance_atk = 1.5f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = Skill62_2_Data.ins.hit_animation_name;
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 10;
        info.scale_x = 2f;
        info.scale_y = 2f;
        info.damage_ratio = Skill62_2_Data.ins.damage_ratio;
        info.collider_size = Skill62_2_Data.ins.hit_rect;
        b = BulletMgr.Create(this.Target, "BulletConfig", info);
    }
    public override void OnSpineComplete()
    {
        ///  this.stack.PopSingleSkill();

        this.OnExit();
    }
    public override void OnExit()
    {
        if (b != null)
        {
            b.SetInValid();
            b = null;
        }
        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        this.ResumeAll();

        /// this.stack.PushSingleSkill(new Skill6_2_2())

    }
    public override bool Init()
    {
        base.Init();
        cd.TickMax();
        return true;
    }

    public override bool OnInterrupted(SkillBase who)
    {
        if (this.Enable && tick_cancel.IsMax())
        {
            this.OnExit();
            return true;
        }
        return false;
    }
    public override void OnAcceptInterrupted(SkillBase who)
    {
        if (this.Enable) return;
        this.OnEnter();
    }
    public override void OnPush()
    {
        if (cd.IsMax() == false) return;
        if (Target.isStand == false) return;
        if (this.Enable) return;
        if (Target.isAttacking)
        {
            this.PushOnInterruptAttackSate(); //强制打断 普通技能
        }
        if (Target.isAttacking)
        {
            this.PushOnInterrupted();
            return;
        }
        this.OnEnter();

    }
    Bullet b = null;
}
*/


/// <summary>
/// 形态2 技能2 重做版本
/// 
/// 为开启/关闭型技能，开启时，每秒损失20（可调）点生命，对范围（可调）内敌人造成每秒100点伤害，升级后，不再造成生命损失
/// </summary>
public class Skill62_2_v2 : SkillBase
{
    public override string GetName()
    {
        return "Skill62_2";
    }
    BufferSkill62_2 buf = null;
    BulletConfig bu_hide = null;
    Counter tick = Counter.Create(40);
    bool has_shoot = false;
    public override void OnEnter()
    {
        cd.SetMax(Skill62_2_Data.Get(level).cd);
        int time = (int)(Skill62_2_Data.Get(level).time * Utils.fps);
        tick.SetMax(time);
        tick.Reset();
        cd.Reset();
        buf = BufferMgr.CreateHelper<BufferSkill62_2>(Target);
        buf.time = time;
        Target.AddBuffer(buf);
        this.Enable = true;

        //特效
        BulletConfigInfo info = BulletConfigInfo.Create();
        info.launch_delta_xyz.x = Skill62_2_Data.Get(level).delta_xyz.x;// 1.5f;
        info.launch_delta_xyz.y = Skill62_2_Data.Get(level).delta_xyz.y;// -0.2f;
        info.launch_delta_xyz.z = Skill62_2_Data.Get(level).delta_xyz.z;// -0.2f;
        info.damage_type = DamageType.REAL;
        info.frameDelay = 4;
        info.distance_atk = 1.5f;
        info.number = 0;
        info.isHitDestory = false;
        info.oneHitTimes = 1;
        info.damage_real = Skill62_2_Data.Get(level).damage;
        //  info.rotate = -120.0f;
        info.plistAnimation = Skill62_2_Data.Get(level).hit_animation_name;
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 0xffff;
        info.scale_x = 2f;
        info.scale_y = 2f;
        info.collider_size = Skill62_2_Data.Get(level).hit_rect;

        info._OnUpdateMS = (Bullet bb, object userdata) =>
        {
            bb.x = Target.x + Skill62_2_Data.Get(level).delta_xyz.x;
            bb.y = Target.y + Skill62_2_Data.Get(level).delta_xyz.y;
            bb.z = Target.z + Skill62_2_Data.Get(level).delta_xyz.z;
            bu_hide.tick = 0;
        };

        bu_hide = BulletMgr.Create(this.Target, "BulletConfig", info) as BulletConfig;

    }
    public override void UpdateMS()
    {
        cd.Tick();
        if (Target.current_hp <= Skill62_2_Data.Get(level).hp_reduce)
        {
            this.OnExit();
            return;
        }
        if (tick.Tick() == false)
        {
            this.Shoot();
            tick.Reset();
        }    
    }
    public override void UpdateMSIdle()
    {
        cd.Tick();
    }
    private void Shoot()
    {
        if (level == 1)
        {
            Target.current_hp -= Skill62_2_Data.Get(level).hp_reduce;
        }
        BulletConfigInfo info = BulletConfigInfo.Create();
        info.launch_delta_xyz.x = Skill62_2_Data.Get(level).delta_xyz.x;// 1.5f;
        info.launch_delta_xyz.y = Skill62_2_Data.Get(level).delta_xyz.y;// -0.2f;
        info.launch_delta_xyz.z = Skill62_2_Data.Get(level).delta_xyz.z;// -0.2f;
        info.damage_type = DamageType.REAL;
        info.frameDelay = 4;
        info.distance_atk = 1.5f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.oneHitTimes = 1;
        info.damage_real = Skill62_2_Data.Get(level).damage;
        //  info.rotate = -120.0f;
        info.plistAnimation = "";// Skill62_2_Data.ins.hit_animation_name;
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 10;
        info.scale_x = 2f;
        info.scale_y = 2f;
        info.collider_size = Skill62_2_Data.Get(level).hit_rect;

        info._OnUpdateMS = (Bullet bb, object userdata) =>
              {
                  bb.x = Target.x + Skill62_2_Data.Get(level).delta_xyz.x;
                  bb.y = Target.y + Skill62_2_Data.Get(level).delta_xyz.y;
                  bb.z = Target.z + Skill62_2_Data.Get(level).delta_xyz.z;
              };


        Bullet b = BulletMgr.Create(this.Target, "BulletConfig", info);
    }
    public override void OnSpineComplete()
    {

    }
    public override void OnExit()
    {
        if (buf != null)
        {
            buf.SetInValid();
            buf = null;
        }
        if (bu_hide != null)
        {
            bu_hide.SetInValid();
            bu_hide = null;
        }
        this.Enable = false;
    }
    public override bool Init()
    {
        base.Init();
        cd.SetMax(Skill62_2_Data.Get(level).cd);
    ///    cd.TickMax();
        tick.Reset();
        return true;
    }

    public override bool OnInterrupted(SkillBase who)
    {
        if (Enable)
        {
            this.OnExit();
            return true;
        }
        return true;

    }
    public override void OnAcceptInterrupted(SkillBase who)
    {
        /* if (this.Enable) return;
         this.OnEnter();*/
    }
    public override void OnPush()
    {
        if (cd.IsMax() == false) return;
        this.PushOnInterrupted();
        if (this.Enable)
        {
            this.OnExit();
            return;
        }
        this.OnEnter();
    }

}


/// <summary>
/// 形态2 技能3
/// 起跳 在快速砍向 地面
/// </summary>
public class Skill62_3_v2 : SkillBase
{
    public override string GetName()
    {
        return "Skill62_3";
    }
    Counter tick1 = Counter.Create(Skill62_3_Data.ins.start_jump);
    Counter tick_cancel = Counter.Create(Skill62_3_Data.ins.cancel);
    bool has_shoot = false;
    public override void OnEnter()
    {
        AudioMgr.ins.PostEvent(AudioEvents.Events.HERO_SKILL23, 1);
        cd.SetMax(Skill62_3_Data.Get(level).cd);
        tick1.SetMax(Skill62_3_Data.Get(level).start_jump);
        tick_cancel.SetMax(Skill62_3_Data.Get(level).cancel);
        tick_cancel.Reset();
        cd.Reset();
        this.PauseAll();
        tick1.Reset();
        jump_speed = Skill62_3_Data.Get(level).jump_speed;// DATA.DEFAULT_JUMP_SPEED * 1f;
        has_shoot = false;
        this.Target.machine.GetState<StandState>().Resume();
        this.Target.machine.GetState<FallState>().Resume();
        Target.isAttacking = true;
        this.Enable = true;
        Target.is_spine_loop = true;
        Target.attackingAnimationName = Skill62_3_Data.Get(level).animation_name;//// "6140_1";
    }

    private float jump_speed = Skill62_3_Data.ins.jump_speed;  // DATA.DEFAULT_JUMP_SPEED * 1f;
    public override void UpdateMS()
    {
        ///  Target.x_auto += Target.flipX * -0.05f;
        tick_cancel.Tick();
        if (tick1.Tick())
        {//阶段1 起跳
            if (jump_speed <= 0.0f)
            {
                tick1.TickMax();

                BulletConfigInfo info = BulletConfigInfo.Create();

                /// info.AddBuffer("BufferHitFly");

                info.launch_delta_xyz.x = 1.5f;
                info.launch_delta_xyz.y = -0.2f;
                info.frameDelay = 3;
                info.distance_atk = 1.5f;
                info.number = 0xfff;
                info.isHitDestory = true;
                info.damage_ratio = 1.5f;
                info.oneHitTimes = 1;
                //  info.rotate = -120.0f;
                ///   info.plistAnimation = "hd/magic_weapons/bullet/bul_500502/bul_500502.plist";

                info.plistAnimation = "";
                /// info.rotate = 30.0f;
                info.distance = 0;
                info.lastTime = 10;
                info.scale_x = 2f;
                info.scale_y = 2f;

                BulletMgr.Create(this.Target, "BulletConfig", info);


            }
            else
            {
                //接入重力

                jump_speed -= 9.8f / 40.0f * 0.1f * Skill62_3_Data.Get(level).gravity_ratio;
                ///   current_height += jump_speed;
                this.Target.y += jump_speed;
            }
        }

        if (Target.y <= 0 && has_shoot == false)
        {
            this.Shoot();
            has_shoot = true;
        }

        if (cd.Tick())
        {
            return;
        }

    }
    public override void UpdateMSIdle()
    {
        cd.Tick();
    }
    private void Shoot()
    {
        {
            BulletConfigInfo info = BulletConfigInfo.Create();
            BufferHitBack hitBack = BufferMgr.CreateHelper<BufferHitBack>(this.Target);
            hitBack.SetAttackSource(Target);
            info.AddBuffer(hitBack);

            if (level >= 2)
            {
                BufferSpin buf = BufferMgr.CreateHelper<BufferSpin>(Target);
                buf.SetLastTime(Skill62_3_Data.Get(level).spin_time);
                info.AddBuffer(buf);
            }
            // info.launch_delta_xy.x = 1.5f;
            //  info.launch_delta_xy.y = -0.2f;
            info.frameDelay = 3;
            info.distance_atk = 1.5f;
            info.number = 0xfff;
            info.isHitDestory = false;
            info.oneHitTimes = 1;
            //  info.rotate = -120.0f;
            ///   info.plistAnimation = "hd/magic_weapons/bullet/bul_500502/bul_500502.plist";

            //  info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6222/role_6_bul_6222.plist";
            /// info.rotate = 30.0f;
            info.distance = 2f;
            ///   info.lastTime = 10;
            info.scale_x = 2f;
            info.scale_y = 2f;

            info.launch_delta_xyz.x = Skill62_3_Data.Get(level).delta_xyz.x;// 1.5f;
            info.launch_delta_xyz.y = Skill62_3_Data.Get(level).delta_xyz.y;// -0.2f;
            info.launch_delta_xyz.z = Skill62_3_Data.Get(level).delta_xyz.z;// -0.2f;

            info.plistAnimation = Skill62_3_Data.Get(level).hit_animation_name;
            info.damage_ratio = Skill62_3_Data.Get(level).damage_ratio;

            if (level == 1)
            {
                //长方
                info.collider_size = Skill62_3_Data.Get(level).hit_rect;
                info.collider_type = ColliderType.Box;
                info.number = 1;
                info.isHitDestory = true;
                info.validTimes = 1;
            }
            else if (level >= 2)
            {
                info.collider_type = ColliderType.Sector;
                info.sector_angle = Skill62_3_Data.Get(level).sector_angle;
                info.sector_radius = Skill62_3_Data.Get(level).sector_radius;
            }
            b = BulletMgr.Create(this.Target, "BulletConfig", info);
        }
        if (level >= 2)
        {
            {
                //视觉效果
                BulletConfigInfo info = BulletConfigInfo.Create();

                // info.launch_delta_xy.x = 1.5f;
                //  info.launch_delta_xy.y = -0.2f;
                info.frameDelay = 3;
                info.distance_atk = 1.5f;
                info.number = 0;
                info.isHitDestory = false;
                info.oneHitTimes = 0;
                //  info.rotate = -120.0f;
                ///   info.plistAnimation = "hd/magic_weapons/bullet/bul_500502/bul_500502.plist";

                //  info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6222/role_6_bul_6222.plist";
                /// info.rotate = 30.0f;
                info.distance = 2f;
                ///   info.lastTime = 10;
                info.scale_x = 2f;
                info.scale_y = 2f;

                info.launch_delta_xyz.x = Skill62_3_Data.Get(level).delta_xyz.x;// 1.5f;
                info.launch_delta_xyz.y = Skill62_3_Data.Get(level).delta_xyz.y;// -0.2f;
                info.launch_delta_xyz.z = Skill62_3_Data.Get(level).delta_xyz.z;// -0.2f;

                info.plistAnimation = Skill62_3_Data.Get(level).hit_animation_name;
                info.damage_ratio = 0f;
                if (Target.flipX < 0)
                {
                    info.dir_2d = Skill62_3_Data.Get(level).sector_angle / 2;
                }
                else
                {
                    info.dir_2d = 180 - Skill62_3_Data.Get(level).sector_angle / 2;
                }

                if (level == 1)
                {
                    //长方
                    info.collider_size = Skill62_3_Data.Get(level).hit_rect;
                    info.collider_type = ColliderType.Box;
                }
                else if (level >= 2)
                {
                    info.collider_type = ColliderType.Sector;
                    info.sector_angle = Skill62_3_Data.Get(level).sector_angle;
                    info.sector_radius = Skill62_3_Data.Get(level).sector_radius;
                }
                BulletMgr.Create(this.Target, "BulletConfig", info);
            }
            {
                //视觉效果
                BulletConfigInfo info = BulletConfigInfo.Create();

                // info.launch_delta_xy.x = 1.5f;
                //  info.launch_delta_xy.y = -0.2f;
                info.frameDelay = 3;
                info.distance_atk = 1.5f;
                info.number = 0;
                info.isHitDestory = false;
                info.oneHitTimes = 0;
                //  info.rotate = -120.0f;
                ///   info.plistAnimation = "hd/magic_weapons/bullet/bul_500502/bul_500502.plist";

                //  info.plistAnimation = "hd/roles/role_6/bullet/role_6_bul_6222/role_6_bul_6222.plist";
                /// info.rotate = 30.0f;
                info.distance = 2f;
                ///   info.lastTime = 10;
                info.scale_x = 2f;
                info.scale_y = 2f;

                info.launch_delta_xyz.x = Skill62_3_Data.Get(level).delta_xyz.x;// 1.5f;
                info.launch_delta_xyz.y = Skill62_3_Data.Get(level).delta_xyz.y;// -0.2f;
                info.launch_delta_xyz.z = Skill62_3_Data.Get(level).delta_xyz.z;// -0.2f;

                info.plistAnimation = Skill62_3_Data.Get(level).hit_animation_name;
                info.damage_ratio = 0f;
                if (Target.flipX < 0)
                {
                    info.dir_2d = 360 - Skill62_3_Data.Get(level).sector_angle / 2;
                }
                else
                {
                    info.dir_2d = 180 + Skill62_3_Data.Get(level).sector_angle / 2;
                }
                if (level == 1)
                {
                    //长方
                    info.collider_size = Skill62_3_Data.Get(level).hit_rect;
                    info.collider_type = ColliderType.Box;
                }
                else if (level >= 2)
                {
                    info.collider_type = ColliderType.Sector;
                    info.sector_angle = Skill62_3_Data.Get(level).sector_angle;
                    info.sector_radius = Skill62_3_Data.Get(level).sector_radius;
                }
                BulletMgr.Create(this.Target, "BulletConfig", info);
            }
        }
    }
    public override void OnSpineComplete()
    {
        ///  this.stack.PopSingleSkill();
        ///  this.Shoot();
        if (this.Enable)
            this.OnExit();
    }
    public override void OnExit()
    {
        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        this.ResumeAll();

        /// this.stack.PushSingleSkill(new Skill6_2_2())

    }
    public override void OnPush()
    {
        if (cd.IsMax() == false)
        {
            return;
        }
        if (this.Enable)
        {
            return;
        }
        if (Target.isJumping == false && Target.isJumpTwice == false)
        {
            //   return;
        }

        if (Target.isAttacking)
        {
            this.PushOnInterruptAttackSate(); //强制打断 普通技能
        }
        if (Target.isAttacking)
        {
            this.PushOnInterrupted();
            return;
        }
        this.OnEnter();
    }
    public override bool Init()
    {
        base.Init();
        cd.SetMax(Skill62_3_Data.Get(level).cd);
        cd.TickMax();
        return true;
    }

    public override bool OnInterrupted(SkillBase who)
    {
        if (this.Enable && tick_cancel.IsMax())
        {
            this.OnExit();
            return true;
        }
        return false;
    }
    public override void OnAcceptInterrupted(SkillBase who)
    {
        if (this.Enable) return;
        this.OnEnter();
    }

    Bullet b = null;
}


/// <summary>
/// 6 终结技
/// </summary>
public class Skill6_Final : SkillBase
{
    public override string GetName()
    {
        return "Skill6_Final";
    }
    Counter tick = Counter.Create(10);
    bool has_shoot = false;
    Buffer buf_god = null;
    public override void OnEnter()
    {
        cd.SetMax(Skill6_Final_Data.ins.cd);

        cd.Reset();
        this.PauseAll();
        this.level = 1;
        tick.Reset();
        has_shoot = false;
        this.Target.machine.GetState<StandState>().Resume();
        this.Target.machine.GetState<FallState>().Resume();
        Target.isAttacking = true;
        this.Enable = true;
        Target.is_spine_loop = false;
        Target.attackingAnimationName = Skill6_Final_Data.ins.level1_animation_name;// "6000";
        buf_god = Target.AddBuffer<BufferGod>();//添加无敌buffer
        TimerQueue.ins.AddTimerMSI((int)Skill6_Final_Data.ins.level1_hit_delay, () =>
        {
            this.Shoot();
        });
    }
    public override void UpdateMS()
    {
        cd.Tick();

        if (this.level == 0)
        {

        }
        else if (this.level == 1)
        {
            //第一阶段
            if (is_1_Hit == true)
            {
                //this.level = 2;
            }
        }
        else if (this.level == 2)
        {

        }
        else if (this.level == 3)
        {

        }
    }
    public override void UpdateMSIdle()
    {
        cd.Tick();
    }
    bool is_1_Hit = false;
    int level = 0;
    private void Shoot()
    {

        BulletConfigInfo info = BulletConfigInfo.Create();

        ///   info.AddBuffer("BufferHitFly");

        info.launch_delta_xyz.x = Skill6_Final_Data.ins.level1_delta_xyz.x;// 1.5f;
        info.launch_delta_xyz.y = Skill6_Final_Data.ins.level1_delta_xyz.y;//-0.2f;
        info.launch_delta_xyz.z = Skill6_Final_Data.ins.level1_delta_xyz.z;//-0.2f;

        info.frameDelay = 4;
        info.distance_atk = 1.5f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.damage_ratio = Skill6_Final_Data.ins.level1_damage_ratio;//1.5f;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = Skill6_Final_Data.ins.level1_hit_animation_name;// "hd/roles/role_6/bullet/role_6_bul_6224/role_6_bul_6224.plist";
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 10;
        info.scale_x = 2f;
        info._OnTakeAttack = (Bullet b1, object userData) =>
            {
                this.is_1_Hit = true;
                if (this.level == 1)
                {
                    Entity h = userData as Entity;
                    this.buf = h.AddBuffer<Buffer6_Final>();

                }
            };
        info.collider_size = Skill6_Final_Data.ins.level1_hit_rect;
        info.scale_y = 2f;
        b = BulletMgr.Create(this.Target, "BulletConfig", info);
    }

    private void ShootFinal()
    {

        BulletConfigInfo info = BulletConfigInfo.Create();

        ///   info.AddBuffer("BufferHitFly");

        info.launch_delta_xyz.x = Skill6_Final_Data.ins.level4_delta_xyz.x;// 1.5f;
        info.launch_delta_xyz.y = Skill6_Final_Data.ins.level4_delta_xyz.y;//-0.2f;
        info.launch_delta_xyz.z = Skill6_Final_Data.ins.level4_delta_xyz.z;//-0.2f;

        info.frameDelay = 4;
        info.distance_atk = 1.5f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.damage_ratio = Skill6_Final_Data.ins.level4_damage_ratio;//1.5f;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        info.plistAnimation = Skill6_Final_Data.ins.level4_hit_animation_name;// "hd/roles/role_6/bullet/role_6_bul_6224/role_6_bul_6224.plist";
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 10;
        info.scale_x = 2f;
        info._OnTakeAttack = (Bullet b1, object userData) =>
        {
            this.is_1_Hit = true;
            if (this.level == 1)
            {
                Entity h = userData as Entity;
                this.buf = h.AddBuffer<Buffer6_Final>();

            }
        };
        info.collider_size = Skill6_Final_Data.ins.level4_hit_rect;
        info.scale_y = 2f;
        b = BulletMgr.Create(this.Target, "BulletConfig", info);
    }
    Buffer buf = null;
    public override void OnSpineComplete()
    {
        ///  this.stack.PopSingleSkill();
        if (this.is_1_Hit == false && this.level == 1)
        {//1段 为命中
            this.OnExit();
            return;
        }
        if (this.is_1_Hit == true && this.level == 1)
        {
            // 1段命中  释放2段
            Target.attackingAnimationName = Skill6_Final_Data.ins.level2_animation_name;//"6240_0";
            this.level++;

            CameraFollow.ins.ShowHeroFinal();

        }
        else if (this.level == 2)
        {//3段
            Target.attackingAnimationName = Skill6_Final_Data.ins.level3_animation_name;// "6240_1";
            this.level++;
        }
        else if (this.level == 3)
        {//4段
            this.ShootFinal();
            Target.attackingAnimationName = Skill6_Final_Data.ins.level4_animation_name; //"6240_2";
            this.level++;
        }
        else
        {
            this.OnExit();
        }
    }
    public override void OnExit()
    {
        if (b != null)
        {
            b.SetInValid();
            b = null;
        }
        if (buf != null) buf.SetInValid();
        if (buf_god != null) buf_god.SetInValid();
        buf_god = null;
        buf = null;
        this.Enable = false;
        Target.isAttacking = false;
        Target.is_spine_loop = true;
        this.ResumeAll();
        this.level = 0;
        if (is_1_Hit)
        {
            CameraFollow.ins.HideHeroFinal();
        }
        this.is_1_Hit = false;
        /// this.stack.PushSingleSkill(new Skill6_2_2())

    }
    public override bool Init()
    {
        base.Init();
        cd.TickMax();

        return true;
    }
    public override bool OnInterrupted(SkillBase who)
    {
        if (this.Enable)
            this.OnExit();
        return true;
        return false;
    }

    public override void OnAcceptInterrupted(SkillBase who)
    {
        if (this.Enable) return;
        this.OnEnter();
    }
    public override void OnPush()
    {
        if (cd.IsMax() == false) return;
        if (Target.isStand == false) return;
        if (this.Enable) return;
        if (Target.isAttacking)
        {
            this.PushOnInterruptAttackSate(); //强制打断 普通技能
        }

        if (Target.isAttacking)
        {
            this.PushOnInterrupted();
            return;
        }
        this.OnEnter();

    }
    Bullet b = null;
}

/// <summary>
/// 切换取消技能
/// </summary>
public class SkillForceCancel : SkillBase
{
    public override string GetName()
    {
        return "SkillForceCancel";
    }

    public override void OnEnter()
    {
        Target.isAttacking = false;
        Target.AddBuffer<BufferForceCancel>();
        this.Enable = true;

    }
    public override void UpdateMS()
    {

    }
    public override void UpdateMSIdle()
    {

    }

    public override void OnExit()
    {

    }

    public override bool Init()
    {
        base.Init();
        return true;
    }

    public override void OnAcceptInterrupted(SkillBase who)
    {
        this.OnEnter();
    }
    public override void OnPush()
    {
        this.PushOnInterruptedForce();//强制打断所有技能
        this.PushChangeSkillGroup();//切换技能组
        this.PushOnInterruptAttackSate(); //强制打断 普通技能
        this.OnEnter();
        this.OnExit();
    }

}


/// <summary>
///Boss 技能1
/// </summary>
public class SkillBoss_1 : SkillBase
{
    public override string GetName()
    {
        return "SkillBoss_1";
    }
    Bullet b = null;
    Counter tick = Counter.Create(0);
    private int atk_times = 0;
    private Buffer bati = null;
    public float distance = 1f; // 冲向目标距离
    private EnemyBoss target;
    private Vector3 pos;
    bool will_run = false;
    public override void OnLevelUp(int target_level)
    {

    }
    public override void OnEnter()
    {
        cd.SetMax(SkillBoss_1_Data.ins.cd);
        cd.Reset();
        atk_times = 0;
        will_run = false;
        this.PauseAll();
        distance = SkillBoss_1_Data.ins.distance;
        b = null;
        this.Target.machine.GetState<StandState>().Resume();
        this.Target.machine.GetState<FallState>().Resume();
        tick.Reset();
        /*  {
              RunXZState state = this.Target.machine.GetState<RunXZState>() as RunXZState;
              state.Resume();
              state.EnableWhenAttack();
          }
          {
              RunXZState state = this.Target.machine.GetState<RunXZState>() as RunXZState;
              state.Resume();
              state.EnableWhenAttack();
          }*/
        target = Target as EnemyBoss;
        if (target == null)
        {
            Debug.LogError("Skill only use in EnemyBoss");
            this.OnExit();
        }
        bati = Target.AddBuffer<BufferBaTi>();
        Target.isAttacking = true;
        this.Enable = true;

        pos = target.target.pos;

        if (target.ClaculateDistance(target.target) < distance)
        { // 目标在攻击范围内
            this.Shoot();
        }
        else
        {//目标不再攻击范围内 直接发起冲锋到目标点 在评定是否在范围内 ，如果不在直接取消 ，在的话直接发起攻击

            will_run = true;
        }
    }
    public override void UpdateMS()
    {
        cd.Tick();
        if (wait_for_next_atk)
        {
            if (tick.Tick() == false)
            {
                this.Shoot();
                wait_for_next_atk = false;
            }
        }
        if (will_run && atk_times == 0)
        {//冲向目标点
            if (target.ClaculateDistance(pos) < distance)
            { // 到达目标点

                if (target.target != null && target.ClaculateDistance(target.target) < distance)
                { // 还在范围内

                    this.Shoot();
                }
                else
                {
                    this.OnExit();//TODO 未在范围内 来回移动
                }
                target.ani_force = "";
                will_run = false;
            }
            else
            {
                int dir = (int)Utils.GetAngle(target.pos, pos);

                // code copy from RunXZState
                float speed = target.speed * SkillBoss_1_Data.ins.run_speed_ratio; // X倍速度冲锋

                float dd = dir * DATA.ONE_DEGREE;//一度的弧度

                float z_delta = Mathf.Sin(dd);
                float x_delta = Mathf.Cos(dd);
                target.z = target.z + speed * z_delta;
                target.x = target.x + speed * x_delta;
                target.ani_force = "run";
                if (dir > 90 && dir < 270)
                { //left
                    target.flipX = 1.0f;
                }
                else
                {//right
                    target.flipX = -1.0f;
                }
            }

        }
    }
    public override void UpdateMSIdle()
    {
        cd.Tick();
    }
    private void Shoot()
    {
        if (atk_times >= 3)
        {
            this.OnExit();
            return;
        }
        tick.Reset();
        ++atk_times;
        BulletConfigInfo info = BulletConfigInfo.Create();

        if (atk_times == 1)
        {
            Target.attackingAnimationName = SkillBoss_1_Data.ins.animation_name_1;  // ; "2110";
            info.plistAnimation = SkillBoss_1_Data.ins.hit_animation_name_1;
        }
        else if (atk_times == 2)
        {
            Target.attackingAnimationName = SkillBoss_1_Data.ins.animation_name_2;// "2000";
            info.plistAnimation = SkillBoss_1_Data.ins.hit_animation_name_2;
        }
        else if (atk_times == 3)
        {
            Target.attackingAnimationName = SkillBoss_1_Data.ins.animation_name_3;// "2210";//"2130";
            info.plistAnimation = SkillBoss_1_Data.ins.hit_animation_name_3;
        }


        //  info.AddBuffer("BufferHitBack");

        info.launch_delta_xyz.x = 1.5f;
        info.launch_delta_xyz.y = -0.2f;
        info.frameDelay = 4;
        info.distance_atk = 1.5f;
        info.number = 0xfff;
        info.isHitDestory = false;
        info.damage_ratio = SkillBoss_1_Data.ins.damage_ratio;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        //        info.plistAnimation = "hd/enemies/enemy_311/bullet/enemy_311_bul_311011/enemy_311_bul_311011.plist";

        ///  ;"hd/roles/role_6/bullet/role_6_bul_6122/role_6_bul_6122.plist";
        /// info.rotate = 30.0f;
        info.distance = 0;
        info.lastTime = 20;
        info.collider_size = SkillBoss_1_Data.ins.hit_rect;// new Vector3(10f, 10f, 10f);
        info.launch_delta_xyz = SkillBoss_1_Data.ins.delta_xyz;// new Vector3(1f, 0f, 0f);
        info.scale_x = 3f;
        info.scale_y = 3f;
        info.AddHitTarget(target.target);
        b = BulletMgr.Create(this.Target, "BulletConfig", info);


    }
    public override void OnSpineComplete()
    {
        ///  this.stack.PopSingleSkill();
        if (will_run == false && atk_times != 0)
        {
            ///  this.Shoot();

            wait_for_next_atk = true;
            tick.Reset();
        }
    }
    bool wait_for_next_atk = false;//是否等待攻击间隔
    public override void OnExit()
    {
        if (b != null)
        {
            b.SetInValid();
            b = null;
        }
        if (bati != null)
        {
            bati.SetInValid();
            bati = null;
        }
        this.Enable = false;
        Target.isAttacking = false;
        tick.Reset();
        Target.is_spine_loop = true;
        RunXZState state = this.Target.machine.GetState<RunXZState>() as RunXZState;
        state.DisableWhenAttak();
        this.ResumeAll();

    }
    public override void OnAcceptInterrupted(SkillBase who)
    {
        if (this.Enable) return;
        this.OnEnter();
    }
    public override void OnPush()
    {
        if (cd.IsMax() == false) return;
        if (Target.isStand == false) return;
        if (this.Enable) return;

        if (Target.isAttacking)
        {
            /// this.PushOnInterruptAttackSate(); //强制打断 普通技能
        }
        if (Target.isAttacking)
        {
            ///  this.PushOnInterrupted();
            /// return;
        }
        this.OnEnter();

    }
    public override bool Init()
    {
        base.Init();
        cd.TickMax();
        EventDispatcher.ins.AddEventListener(this, Events.ID_DIE);
        return true;
    }
    public override bool OnInterrupted(SkillBase who)
    {
        if (Enable)
        {
            this.OnExit();
        }
        return true;
    }
}

/// <summary>
///Boss 技能2
/// </summary>
public class SkillBoss_2 : SkillBase
{
    public override string GetName()
    {
        return "SkillBoss_2";
    }
    ////  Bullet b = null;
    //  Counter tick = Counter.Create(120);
    Counter tick_auto = Counter.Create(40);//自动攻击间隔
    private int atk_times = 0;
    public int level_buffer = 0;
    BufferBoss2 buf = null;
    private Enemy target = null;
    int MAX_BUFFER_LEVEL = 3;
    public override void OnLevelUp(int target_level)
    {

    }
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_DIE)
        {
            if (userData as Entity == Target)
            {//死亡后清除Buffer 
                //TODO 死亡后 复用 还是创建新Buffer
                //  this.Clear();
            }
        }
    }
    public void Clear()
    {
        atk_times = 0;
        level_buffer = 0;
        if (buf != null)
        {
            buf.SetInValid();
            buf = null;
        }
    }
    public override void OnEnter()
    {
        cd.SetMax(SkillBoss_2_Data.ins.cd);
        cd.Reset();
        tick_auto.SetMax(SkillBoss_2_Data.ins.auto_atk_interval);
        atk_times = 0;
        MAX_BUFFER_LEVEL = SkillBoss_2_Data.ins.max_buf_level;
        this.PauseAll();
        this.Target.machine.GetState<StandState>().Resume();
        this.Target.machine.GetState<FallState>().Resume();
        // tick.Reset();

        target = Target as EnemyBoss;
        if (target == null)
        {
            Debug.LogError("Skill only use in EnemyBoss");
            this.OnExit();
        }
        Target.isAttacking = true;
        this.Enable = true;
        this.Shoot();

        if (level_buffer < MAX_BUFFER_LEVEL)
        {//叠加buffer
            ++level_buffer;
            if (buf == null)
            {//只保存 一段Buf 其余都是合并而来
                buf = BufferMgr.CreateHelper<BufferBoss2>(Target);
                buf = Target.AddBuffer<BufferBoss2>() as BufferBoss2;
                buf.percent = SkillBoss_2_Data.ins.per_buf_damage_percent;//10;
            }
            else
            {
                Target.AddBuffer<BufferBoss2>();
            }
        }
    }
    public override void UpdateMS()
    {
        cd.Tick();
        //  if (tick.Tick())
        {
            return;
        }
    }
    public override void UpdateMSIdle()
    {
        cd.Tick();
        if (Target != null && (Target.isStand || Target.isRunning))
        {
            if (tick_auto.Tick())
            {
                return;
            }
            if (target != null)
            {
                /// this.Shoot();
                Target.s1 = 2;
                tick_auto.Reset();
            }
        }
    }
    private void Shoot()
    {
        // tick.Reset();
        ++atk_times;

        Target.attackingAnimationName = "2140";
        Target.attackingAnimationName = SkillBoss_2_Data.ins.animation_name;
        BulletConfigInfo info = BulletConfigInfo.Create();
        info.frameDelay = 1;
        info.distance_atk = 1.5f;
        info.number = 1;
        info.oneHitTimes = 1;
        //  info.rotate = -120.0f;
        //  info.plistAnimation = "hd/magic_weapons/bullet/bul_500502/bul_500502.plist";

        //   info.plistAnimation = "hd/enemies/enemy_311/bullet/enemy_311_bul_311031/enemy_311_bul_311031.plist";
        info.plistAnimation = SkillBoss_2_Data.ins.hit_animation_name;
        /// info.rotate = 30.0f;
        info.distance = target.ClaculateDistance(target.target);
        ///   info.lastTime = 10;
        info.scale_x = SkillBoss_2_Data.ins.scale_xy;
        info.scale_y = SkillBoss_2_Data.ins.scale_xy;

        // info.launch_delta_xyz.x = 0.5f;// Skill62_3_Data.ins.delta_xyz.x;// 1.5f;
        // info.launch_delta_xyz.y = -0.2f;// Skill62_3_Data.ins.delta_xyz.y;// -0.2f;
        // info.launch_delta_xyz.z = 0f;// Skill62_3_Data.ins.delta_xyz.z;// -0.2f;
        info.launch_delta_xyz = SkillBoss_2_Data.ins.delta_xyz;
        info.isHitDestory = true;
        info.damage_ratio = SkillBoss_2_Data.ins.damage_ratio;
        info.collider_size = SkillBoss_2_Data.ins.hit_rect;
        info.AddHitTarget(target.target);
        ///    info.AddHitTarget(target.target);
        info.collider_type = ColliderType.Box;
        info._OnTakeAttack = (Bullet bbbb, object user) =>
        {
            TimerQueue.ins.AddTimerMSI(10, () =>
            {
                ///   this.AddBuffer("BufferEnemyMovementAfterAtk");
            });
        };
        info._OnLaunch = (Bullet bbbb, object user) =>
        {
            info.dir_2d = (int)Utils.GetAngle(Target.pos, target.target.pos);
        };

        BulletMgr.Create(this.Target, "BulletConfig", info);


    }
    public override void OnSpineComplete()
    {
        this.OnExit();
    }
    public override void OnExit()
    {
        //      b.SetInValid();
        this.Enable = false;
        Target.isAttacking = false;
        //  tick.Reset();
        Target.is_spine_loop = true;

        this.ResumeAll();
    }
    public override void OnAcceptInterrupted(SkillBase who)
    {
        if (this.Enable) return;
        this.OnEnter();
    }
    public override void OnPush()
    {
        if (cd.IsMax() == false) return;
        if (Target.isStand == false) return;
        if (this.Enable) return;

        if (Target.isAttacking)
        {
            /// this.PushOnInterruptAttackSate(); //强制打断 普通技能
        }
        if (Target.isAttacking)
        {
            ///  this.PushOnInterrupted();
            /// return;
        }
        this.OnEnter();

    }
    public override bool Init()
    {
        base.Init();
        cd.TickMax();

        return true;
    }
    public override bool OnInterrupted(SkillBase who)
    {
        if (Enable)
        {
            this.OnExit();
        }
        return true;
    }
}


/// <summary>
///Boss 技能3
/// </summary>
public class SkillBoss_3 : SkillBase
{
    public override string GetName()
    {
        return "SkillBoss_3";
    }
    Counter tick = Counter.Create(120);
    Buffer bati = null;
    public override void OnLevelUp(int target_level)
    {

    }
    public override void OnEnter()
    {
        tick.SetMax(SkillBoss_3_Data.ins.last_time);
        cd.SetMax(SkillBoss_3_Data.ins.cd);
        cd.Reset();
        this.PauseAll();
        this.Target.machine.GetState<StandState>().Resume();
        this.Target.machine.GetState<FallState>().Resume();
        tick.Reset();
        bati = Target.AddBuffer<BufferBaTi>();
        Target.isAttacking = true;
        this.Enable = true;
        points = AppMgr.GetCurrentApp<BattleApp>().GetCurrentWorldMap().GetCustomObjects<TerrainObjectEnemyBornPoint>();


        this.Shoot();
    }
    private ArrayList points; // 出生点
    TerrainObjectEnemyBornPoint GetBornPoint(int id)
    {
        foreach (TerrainObjectEnemyBornPoint point in points)
        {
            if (point.id == id) return point;
        }
        return null;
    }
    bool has_call = false;
    public override void UpdateMS()
    {
        cd.Tick();
        if (tick.Tick())
        {
            return;
        }

        TimerQueue.ins.AddTimerMSI(1, () =>
        {
            //开始召唤
            ArrayList lists = SkillBoss_3_Data.ins.Get(this.level); // 按照技能等级（波数）来召唤， AI会设置level 来表示波数

            foreach (SkillBoss_3_CallData data in lists)
            {
                TerrainObjectEnemyBornPoint point = GetBornPoint(data.id);
                EnemyLauncher launch = EnemyLauncher.Create(new Vector3(point.x, point.y, point.z), data.code, Utils.ConvertToFPS(data.time), AIEnemyType.Normal);
                ModelMgr.ins.Add(launch);
            }
            //      Debug.LogError("开始召唤波数" + level);
        });

        this.OnExit();
    }
    public override void UpdateMSIdle()
    {
        cd.Tick();
    }
    private void Shoot()
    {
        Target.attackingAnimationName = SkillBoss_3_Data.ins.animation_name;//"2240_0";
    }
    public override void OnSpineComplete()
    {
        ///  this.stack.PopSingleSkill();

    }
    public override void OnExit()
    {
        if (bati != null)
        {
            bati.SetInValid();
            bati = null;
        }
        this.Enable = false;
        Target.isAttacking = false;
        tick.Reset();
        RunXZState state = this.Target.machine.GetState<RunXZState>() as RunXZState;
        state.DisableWhenAttak();
        this.ResumeAll();

        /// this.stack.PushSingleSkill(new Skill6_2_2());
    }
    public override void OnAcceptInterrupted(SkillBase who)
    {
        if (this.Enable) return;
        this.OnEnter();
    }
    public override void OnPush()
    {
        if (cd.IsMax() == false) return;
        if (Target.isStand == false) return;
        if (this.Enable) return;

        if (Target.isAttacking)
        {
            /// this.PushOnInterruptAttackSate(); //强制打断 普通技能
        }
        if (Target.isAttacking)
        {
            ///  this.PushOnInterrupted();
            /// return;
        }
        this.OnEnter();

    }
    public override bool Init()
    {
        base.Init();
        cd.TickMax();
        return true;
    }
    public override bool OnInterrupted(SkillBase who)
    {
        if (Enable)
        {
            this.OnExit();
        }
        return true;
    }
}
