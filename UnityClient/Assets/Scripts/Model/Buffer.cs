/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class Buffer : Model
{

    //----for ui
    public bool show_ui = false;
    public string brief = "";
    public string name = "";
    public string icon = "hd/interface/items/502154.png";


    //--------for view
    public string plist = "";
    public bool has_view = false;
    public float scale = 0.7f;
    public Counter GetCounter()
    {
        return tick;
    }

    //--for logic
    public Entity target;// target entity
    public Entity owner; // who launch this buffer
    public BufferMgr mgr = null;//reference
    public int tick1 = 0;
    public int MAX_TICK = 80;
    public int id = 0;
    public bool isOnlyOne = false;//buffer是否是唯一
    public virtual int GetId() { return id; }  //buffer 唯一id



    public virtual string GetName() { return "Buffer"; }
    /// <summary>
    /// then override this remember call base.UpdateMS()
    /// </summary>
    public override void UpdateMS()
    {
        ++tick1;
        if (this.IsComplete())
        {
            this.SetInValid();
        }
    }
    public override void OnEnter()
    {
        base.OnEnter();
    }

    /// <summary>
    /// 当前buffer 是否结束
    /// 
    /// 默认时间为结束 标示,可重载 为其他方式，比如某条件下才是完成
    /// </summary>
    /// <returns></returns>
    public virtual bool IsComplete()
    {
        return (tick1 > MAX_TICK);
    }
    /// <summary>
    /// 如果冲突 返回 true ，添加buffer 事件
    /// 用于处理buffer 之间的冲突
    /// </summary>
    /// <param name="what"></param>
    /// <returns></returns>
    public bool IsConflict(Buffer what)
    {// 读取 buffer 冲突 配置表
        bool ret = ConfigTables.BufferConflict.IsConflict(this, what);
        /*  if(ret)
          {
              Debug.LogError(this.GetName() + "  Conflict   " + what.GetName());
          }*/
        return ret;
    }
    /// <summary>
    /// 当Buffer为唯一时 一样的buffer添加后 触发合并事件
    /// </summary>
    /// <param name="other"></param>
    public virtual void OnMerge(Buffer other)
    {

    }
    public override void OnExit()
    {
        base.OnExit();
    }
    public override void OnDispose()
    {
        base.OnDispose();
    }

    public override bool Init()
    {
        base.Init();

        return true;
    }

    public void SetLastTime(float s)
    {
        this.MAX_TICK = (int)(s * 40.0f);
    }

    protected Counter tick = Counter.Create();
}



public class Buffer1 : Buffer
{//持续伤害buffer

    public override void UpdateMS()
    {
        base.UpdateMS();
        if (this.IsInValid()) return;

        if (IsComplete() == false)
        {
            target.hp -= 10;
        }
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



public class Buffer2 : Buffer
{//  无敌 2s，buffer

    public override void UpdateMS()
    {
        base.UpdateMS();
        if (this.IsInValid()) return;

        if (IsComplete() == false)
        {
            target.attackAble = false;
        }

    }
    public override void OnEnter()
    {
        base.OnEnter();
        if (target.attackAble == false)
        {// has other same buffer
            this.SetInValid();
        }
        this.MAX_TICK = 80;
    }


    public override void OnExit()
    {
        base.OnExit();
        target.attackAble = true;
    }
}





public class Buffer3 : Buffer
{// 眩晕  2s，buffer

    public override void UpdateMS()
    {
        base.UpdateMS();
        if (this.IsInValid()) return;

        if (IsComplete() == false)
        {

        }

    }
    public override void OnEnter()
    {
        base.OnEnter();
        if (target.attackAble == false)
        {// has other same buffer
            this.SetInValid();
        }
        /*   target.eventDispatcher.PostEvent(GAEvent.ID_DISABLE_RUN);
           target.eventDispatcher.PostEvent(GAEvent.ID_DISABLE_JUMP);
           target.eventDispatcher.PostEvent(GAEvent.ID_DISABLE_ATTACK);
           */
        this.MAX_TICK = 80;
    }


    public override void OnExit()
    {
        base.OnExit();
        /*  target.eventDispatcher.PostEvent(GAEvent.ID_ENABLE_RUN);
          target.eventDispatcher.PostEvent(GAEvent.ID_ENABLE_JUMP);
          target.eventDispatcher.PostEvent(GAEvent.ID_ENABLE_ATTACK);
          */
    }
}





public class Buffer2_1 : Buffer
{//持续回血

    public override void UpdateMS()
    {
        base.UpdateMS();
        if (this.IsInValid()) return;

        if (IsComplete() == false)
        {
            target.current_hp += 1;

        }
    }
    public override void OnEnter()
    {
        base.OnEnter();
        this.SetLastTime(5);
        ///   target.x_auto -= 1 * target.flipX;
    }


    public override void OnExit()
    {
        base.OnExit();
    }

    public override bool Init()
    {
        base.Init();

        //  ViewMgr.Create<ViewBuffer2_1>(this);
        this.target = this.owner;
        return true;
    }
}



public class BufferFlashMove : Buffer
{//闪现一段距离

    public override void OnEnter()
    {
        base.OnEnter();
        //移动 1 米
        target.x_auto -= 1 * target.flipX;
        this.SetInValid();
    }
    public override bool Init()
    {
        base.Init();

        //  ViewMgr.Create<ViewBuffer2_1>(this);
        this.target = this.owner;
        return true;
    }
}



//装备buffer 屠龙效果 测试

//一定几率触发Buffer 攻击力+10 暴击率+10% 最多叠加5次 持续10 s
public class BufferEquipTest1 : Buffer
{

    public override void OnEnter()
    {
        base.OnEnter();



    }
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_BATTLE_ENTITY_BEFORE_ATTACK)
        {
            if (userData as Entity != this.owner) return;
            //一定几率触发屠龙效果
            if (this._level >= 5) return;

            if (random.Next(0, 100) < 50)
            {
                tick.Reset();
                this.show_ui = true;
                //触发屠龙效果
                this._level += 1;
                int add_damage = 50;
                this.owner.damage += add_damage;

                int add_crits = 10;
                this.owner.crits_ratio += add_crits;
                Debug.Log("触发了屠龙效果" + this._level + " 伤害增加" + add_damage + "  暴击率增加" + add_crits);
                this.brief = "屠龙x" + this._level;

            }

        }
    }
    public override void UpdateMS()
    {
        if (tick.Tick())
        {
            return;
        }
        this.ResetBuffer();
    }

    public void ResetBuffer()
    {
        this.show_ui = false;
        if (this._level <= 0) return;

        this.owner.damage -= this._level * 50;
        this.owner.crits_ratio -= this._level * 10;

        this._level = 0;
        Debug.Log("屠龙效果 清除");
    }
    public override bool Init()
    {
        base.Init();
        this.icon = "hd/interface/items/503063.png";

        EventDispatcher.ins.AddEventListener(this, Events.ID_BATTLE_ENTITY_BEFORE_ATTACK);
        this.target = this.owner;
        tick.SetMax(1600);//40 s
        return true;
    }

    private System.Random random = new System.Random(0);
    private int _level = 0;
}





//装备buffer 护体效果 测试

// 持续4秒 吸收100点伤害 免疫击退
public class BufferEquipTest2 : Buffer
{

    public override void OnEnter()
    {
        base.OnEnter();

    }
    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_BATTLE_ENTITY_BEFORE_TAKEATTACKED)
        {
            AttackInfo info = userData as AttackInfo;


            if (info.target != this.owner) return;
            //一定几率触发

            if (random.Next(0, 100) < 30)
            {
                tick.Reset();
                this.left_hp = 25;
                this.show_ui = true;
                Debug.Log("触发了护体效果吸收" + left_hp + "伤害  Hero剩余血量" + owner.current_hp);

                //触发
            }


            if (left_hp > 0)
            {
                int damage = info.damage;

                this.left_hp -= info.damage;
                info.damage = 0;
                if (this.left_hp < 0)
                {
                    info.damage = -left_hp;
                    this.ResetBuffer();
                }
                foreach (string buf in info.buffers_string)
                {
                    if (buf == "BufferHitBack")
                    {
                        Debug.Log("移除击退效果");
                        info.buffers_string.Remove(buf);
                        break;
                    }
                }
                //  if (this.left_hp <= 0) left_hp = 0;
                //     info.damage = -left_hp;
                if (info.is_crits)
                {
                    Debug.Log("护体效果吸收了 " + (damage - info.damage) + " 点暴击伤害 剩余血量" + owner.current_hp);
                }
                else
                {
                    Debug.Log("护体效果吸收了 " + (damage - info.damage) + " 点伤害 剩余血量" + owner.current_hp);

                }

            }



        }
    }
    public override void UpdateMS()
    {
        if (tick.Tick())
        {
            return;
        }
        if (left_hp > 0)
        {
            this.ResetBuffer();
        }
    }
    private void ResetBuffer()
    {
        this.left_hp = 0;
        Debug.Log("护体效果结束");
        this.show_ui = false;
    }

    public override bool Init()
    {
        base.Init();
        this.icon = "hd/interface/items/503119.png";

        EventDispatcher.ins.AddEventListener(this, Events.ID_BATTLE_ENTITY_BEFORE_TAKEATTACKED);
        this.target = this.owner;
        this.brief = "护体";
        tick.SetMax(160);// 4 s
        return true;
    }

    private int left_hp = 0;//护罩剩余血量
    private System.Random random = new System.Random(0);
}




/// <summary>
/// 减速
/// </summary>
public class BufferSpeedSlow : Buffer
{
    public override string GetName()
    {
        return "BufferSpeedSlow";
    }
    public float percent = 50.0f;//减速百分比
    public int time = 120;//默认减速3s
    private float speed_slow = 0.0f;

    private bool nonsense = true;
    public override void OnEnter()
    {
        //去重
        {
            Buffer other = mgr.GetBuffer(this.GetId());
            if (other != null && other != this && other.GetId() == this.GetId())
            {
                other.GetCounter().Reset();
                this.SetInValid();
                return;
            }
        }

        nonsense = false;
        base.OnEnter();
        tick.SetMax(time);
        speed_slow = percent / 100.0f * this.owner.speed;
        this.target.speed -= speed_slow;
        Debug.Log("减速度");

    }
    public override void UpdateMS()
    {
        if (nonsense) return;
        if (tick.Tick())
        {
            return;
        }
        this.SetInValid();
    }
    public override bool Init()
    {
        base.Init();
        return true;
    }
    public override void OnExit()
    {
        if (nonsense) return;
        this.target.speed += speed_slow;
        Debug.Log("恢复速度");

        base.OnExit();
    }

}



/// <summary>
/// 击退
/// </summary>
public class BufferHitBack : Buffer
{
    public override string GetName()
    {
        return "BufferHitBack";
    }
    public int time = 15;//默认3s
    public int dir = 1;
    public bool nonsense = true;
    public override void OnEnter()
    {
        //去重
        {
            Buffer other = mgr.GetBuffer(this.GetId());
            if (other != null && other != this && other.GetId() == this.GetId())
            {
                ///   other.GetCounter().Reset();
                this.SetInValid();
                return;
            }
        }

        nonsense = false;
        base.OnEnter();
        tick.SetMax(time);
        target.machine.GetState<RunState>().Pause();
        Debug.Log("击退开始");

    }
    public override void UpdateMS()
    {
        if (nonsense) return;

        if (tick.Tick())
        {
            target.x_auto -= dir * 0.07f;
            return;
        }
        this.SetInValid();
    }
    public override bool Init()
    {
        base.Init();
        this.id = 0xffef1;
        if (target.flipX > 0)
        {
            dir = -1;
        }
        return true;
    }
    public override void OnExit()
    {
        if (nonsense) return;
        Debug.Log("击退结束");
        target.machine.GetState<RunState>().Resume();
        base.OnExit();
    }

}



/// <summary>
/// 击飞
/// </summary>
public class BufferHitFly : Buffer
{
    public override string GetName()
    {
        return "BufferHitFly";
    }
    public int time = 15;//默认3s
    public float height = 2.0f;
    public float current_height = 0.0f;
    public float speed = 7.0f;
    public override void OnEnter()
    {
        this.isOnlyOne = true;

        base.OnEnter();
        tick.SetMax(5);
        target.machine.PauseAllStack();
        target.machine.GetState<HurtState>().Resume();
        Debug.Log("击飞开始");

    }
    public override void UpdateMS()
    {
        if (current_height > height)
        {
            this.SetInValid();
            return;
        }

      //  target.y += 0.05f;
      //  current_height += 0.05f;

 


        if (jump_speed <= 0.0f)
        {
            if(tick.Tick())
            {

                return;
            }
            this.SetInValid();
        }
        else
        {
            //接入重力

            jump_speed -= 9.8f / 40.0f * 0.05f;
         ///   current_height += jump_speed;
            this.target.y += jump_speed;
        }







    }
    private float jump_speed = DATA.DEFAULT_JUMP_SPEED*1.0f;

    public override bool Init()
    {
        base.Init();

        return true;
    }
    public override void OnExit()
    {

        target.machine.ResumeAllStack();
        base.OnExit();
        Debug.Log("击飞结束");

    }

}





/// <summary>
///立刻复活Buffer
/// </summary>
public class BufferRevive : Buffer
{
    public override string GetName()
    {
        return "BufferRevive";
    }

    public int point_index = 0;
    public override void OnEnter()
    {
        target.current_hp = target.hp;
        target.isDie = false;
        target.machine.Resume();

        ArrayList points = AppMgr.GetCurrentApp<BattleApp>().GetCurrentWorldMap().GetCustomObjects<TerrainObjectRevivePoint>();

        TerrainObjectRevivePoint p = points[point_index - 1] as TerrainObjectRevivePoint;

        target.x = p.x;
        target.SetRealY(p.y);

        EventDispatcher.ins.PostEvent(Events.ID_REVIVE, target);
        this.SetInValid();
        ///     Debug.Log("复活 " + p.x + "   " + p.y);
    }
    public override void UpdateMS()
    {

    }
    public override bool Init()
    {
        base.Init();
        return true;
    }

    public override void OnEvent(int type, object userData)
    {

    }
    public override void OnExit()
    {
        base.OnExit();
    }

}



/// <summary>
/// 免疫控制技能 buffer
/// </summary>
public class BufferNegativeUnbeatable : Buffer
{
    public override string GetName()
    {
        return "BufferNegativeUnbeatable";
    }
    public override void OnEnter()
    {
        base.OnEnter();

    }
    public override bool Init()
    {
        return base.Init();
    }
    public override void UpdateMS()
    {
        base.UpdateMS();
    }


}


/// <summary>
/// 眩晕
/// </summary>
public class BufferSpin : Buffer
{
    float time = 3.0f;//持续时间

    public override string GetName()
    {
        return "BufferSpin";
    }
    public override void OnEnter()
    {
        EventDispatcher.ins.AddEventListener(this, Events.ID_BEFORE_ONEENTITY_UPDATEMS);

        target.eventDispatcher.PostEvent("SpineComplete");
        this.SetLastTime(time);
        target.machine.PauseAllStack();

        if (target as Hero == HeroMgr.ins.self)
        {
            PublicData.ins.inputAble = false;
        }
        target.machine.GetState<RunState>().SetDisable();
        target.machine.GetState<FallState>().Resume();
        target.machine.GetState<StandState>().Resume();



    }
    public override void OnMerge(Buffer other)
    {

    }
    public override void OnExit()
    {
        target.machine.ResumeAllStack();
        if (target as Hero == HeroMgr.ins.self)
        {
            PublicData.ins.inputAble = true;
        }
        EventDispatcher.ins.RemoveEventListener(this, Events.ID_BEFORE_ONEENTITY_UPDATEMS);
    }
    public override bool Init()
    {
        base.Init();
        isOnlyOne = true;
        has_view = true;
        plist = "hd/buff/buff_200564/buff_200564.plist";
        plist = "88";

        return true;
    }
    public override void OnDispose()
    {

    }
    public override void UpdateMS()
    {
        base.UpdateMS();

        /*
        target.isAttacking = false;
   
       
    ///    target.isStand = false;
        target.isRunning = false;
        target.isJumpTwice = false;
        target.isDie = false;*/
        target.isJumping = false;
        target.isHurt = false;
        target.isRunning = false;
    }

    public override void OnEvent(int type, object userData)
    {
        if (this.IsInValid()) return;
        if (type == Events.ID_BEFORE_ONEENTITY_UPDATEMS)
        {
            if (userData as Entity != this.target) return;
            //reset all cmd
            target.atk = false;
            target.left = false;
            target.right = false;
            target.jump = false;
            target.s1 = 0;
        }
    }


}


/// <summary>
/// 可配置 buffer
/// </summary>
public class BufferConfig : Buffer
{
    public HashTable kvs = HashTable.Create();
    public string Name = "BufferConfig";
    public override string GetName()
    {
        return Name;
    }
    public override void OnEnter()
    {
        if (_OnEnter != null)
        {
            _OnEnter(this);
        }
    }
    public override bool Init()
    {
        base.Init();
        if (_OnInit != null)
        {
            _OnInit(this);
        }
        return true;
    }
    public override void UpdateMS()
    {
        if (_OnUpdateMS != null)
        {
            _OnUpdateMS(this);
        }
    }
    public VoidFuncN<Buffer> _OnUpdateMS = null;
    public VoidFuncN<Buffer> _OnEnter = null;
    public VoidFuncN<Buffer> _OnExit = null;
    public VoidFuncN<Buffer> _OnInit = null;

}


/// <summary>
///  lua buffer wrapper
///  config this to  write your own buffer in lua
/// </summary>
public class Buffer_LuaInterface : Buffer
{



}

