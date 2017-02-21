/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


public class Buffer : Model
{
    public Entity target;// target entity
    public Entity owner; // who launch this buffer

    public int tick = 0;
    public int MAX_TICK = 80;

    public virtual int GetId() { return 0; }  //buffe 唯一id

    /// <summary>
    /// then override this remember call base.UpdateMS()
    /// </summary>
    public override void UpdateMS()
    {
        ++tick;
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
        return (tick > MAX_TICK);
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
        target.x_auto -= 1 * target.flipX;
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

/// <summary>
///  lua buffer wrapper
///  config this to  write your own buffer in lua
/// </summary>
public class Buffer_LuaInterface : Buffer
{



}

