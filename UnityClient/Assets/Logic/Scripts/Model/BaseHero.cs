/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

/// <summary>
/// 主界面的模型
/// 接受位移事件
/// </summary>
public class BaseHero : Hero
{
    public override void InitStateMachine()
    {
        //init state machine

        {
            StateStack s = StateStack.Create();
            this.machine.AddParallelState(s);
            s.PushSingleState(StateBase.Create<RunXYState>(this));

        }
    }

    public override bool Init()
    {
        base.Init();

        // config
        this.skin = "#1";
        this.prefabsName = "Prefabs/Hero2";
        ani_run = "run";
        ani_stand = "stand";

        ViewMgr.Create<ViewEntity>(this);

        return true;
    }

    public override void OnEvent(int type, object userData)
    {
        base.OnEvent(type, userData);

        if (type == Events.ID_LAUNCH_SKILL1)
        {
            //  this.AddBuffer(bufferMgr.Create<Buffer4>());

            //   Bullet b = BulletMgr.Create<Bullet2_1>(this);

        }
    }


    public override void UpdateMS()
    {
        base.UpdateMS();
        if (this == HeroMgr.ins.self) return;

        if (tick.Tick()) return;

        Debug.Log("timeout hero base");
        this.SetInValid();
    }
    public void ResetTick()
    {
        tick.Reset();
    }

    Counter tick = Counter.Create(DATA.HERO_ALIVE_TICK);// 30 秒
}
