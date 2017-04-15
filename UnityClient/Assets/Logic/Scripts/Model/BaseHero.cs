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
        this.prefabsName = "Prefabs/Hero/Hero6";
        ani_run = "run";
        ani_stand = "stand";
        scale = 0.8f;

        return true;
    }
    public override void OnEnter()
    {
        ViewMgr.Create<ViewEntity>(this);

    }
    public bool IsRobot = false;

    public override void OnEvent(int type, object userData)
    {
        base.OnEvent(type, userData);
    }

    public static BaseHero CreateRobot(int no = -1)
    {
        BaseHero hero = new BaseHero();
        hero.IsRobot = true;
        hero.Init();
        hero.prefabsName = "Prefabs/Hero/Hero" + UnityEngine.Random.Range(1, 7);
        if (no == -1)
        {
            hero.no = UnityEngine.Random.Range(512, 1024);
        }
        hero.name = "机器人" + hero.no;
        hero.x = UnityEngine.Random.Range(0f, 24f);
        hero.y = UnityEngine.Random.Range(0f, 2.3f);

        HeroMgr.ins.Add(hero);
        return hero;
    }
    public override void UpdateMS()
    {
        base.UpdateMS();
        if (this == HeroMgr.ins.self) return;
        if (this.IsRobot)
        {
            if (UnityEngine.Random.Range(0, 300) == 5)
            {
                eventDispatcher.PostEvent(Events.ID_LOGIC_NEW_POSITION, new Vector2(UnityEngine.Random.Range(0f, 24f), UnityEngine.Random.Range(0f, 2.3f)));
            }
            return;
        }
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
