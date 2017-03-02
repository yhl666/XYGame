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
    public Counter GetCounter()
    {
        return tick;
    }

    //--for logic
    public Entity target;// target entity
    public Entity owner; // who launch this buffer

    public int tick1 = 0;
    public int MAX_TICK = 80;

    public virtual int GetId() { return 0; }  //buffe 唯一id

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

// 持续4秒 吸收100点伤害
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
                Debug.Log("触发了护体效果吸收" + left_hp+"伤害  Hero剩余血量" + owner.current_hp);
          
                //触发
            }


            if (left_hp > 0)
            {
                int damage = info.damage;

                this.left_hp -= info.damage;
                info.damage =0;
                if (this.left_hp < 0)
                {
                    info.damage = -left_hp;
                    this.ResetBuffer();
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
///  lua buffer wrapper
///  config this to  write your own buffer in lua
/// </summary>
public class Buffer_LuaInterface : Buffer
{



}

