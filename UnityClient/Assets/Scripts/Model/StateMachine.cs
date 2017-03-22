/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
 

/// <summary>
/// 并行状态机，负责处理维护 每个 状态栈
// 每个 Object都有一个 state machine 
/// </summary>
public sealed class StateMachine : GAObject
{
    public static StateMachine Create(Entity host)
    {
        StateMachine ret = new StateMachine();
        ret.host = host;
        ret.Init();
        return ret;
    }
    private StateMachine()
    {

    }
    public void AddParallelState(StateStack s)
    {//并行状态栈
        states.Add(s);
        s.machine = this;
        s.host = this.host;
        s.OnEnter();
    }
    public void RemoveParallelState(StateStack s)
    {//并行状态栈
        states.Remove(s);
        s.OnExit();
        s.host = null;
        s.machine = null;
    }
    public override bool Init()
    {
        base.Init();
        return true;
    }

    public override void OnEnter()
    {
        base.OnEnter();

    }

    public override void UpdateMS()
    {
        foreach (StateStack s in states)
        {
            if (s.pause) continue;
            if (pause) continue;

            s.UpdateMS();
        }
    }

    public StateStack GetStack(string name)
    {
        foreach (StateStack s in states)
        {
            if (s.name == name) return s;

        }
        return null;
    }

    public StateStack GetStack(int id)
    {
        foreach (StateStack s in states)
        {
            if (s.id == id) return s;

        }
        return null;
    }

    public void Pause()
    {
        pause = true;
    }
    public void Resume()
    {
        pause = false;
    }
    public bool IsPause()
    {
        return pause;
    }
    public bool IsAllStackPause()
    {
        return pause_all;
    }
    private bool pause_all = false;

    public void PauseAllStack()
    {
        pause_all = true;
        foreach (StateStack s in states)
        {
            s.Pause();
        }
    }
    public void ResumeAllStack()
    {
        pause_all = false;
        foreach (StateStack s in states)
        {
            s.Resume();
        }
    }



    public StateBase GetState<T>() where T : new()
    {
        foreach (StateStack s in states)
        {

            StateBase state = s.GetState<T>();
            if (state != null) return state;
        }
        return null;
    }
    ArrayList states = new ArrayList();
    private Entity host = null;
    private bool pause = false;
}

/// <summary>
/// 状态栈（单行状态），负责维护每个状态
/// </summary>
public sealed class StateStack : GAObject
{
    public const int ID_UNKNOWN = 0;
    public const int ID_RUN = 1;
    public const int ID_JUMP = 2;
    public const int ID_ATTACK = 3;

    public static StateStack Create()
    {
        StateStack ret = new StateStack();
        ret.Init();
        return ret;
    }
    private StateStack()
    {

    }
    public void Pause()
    {
        pause = true;
        StateBase s = stacks.Peek() as StateBase;
        s.OnPause();
    }
    public void Resume()
    {
        pause = false;
        StateBase s = stacks.Peek() as StateBase;
        s.OnResume();
    }
    public void PushSingleState(StateBase s)
    {//单行状态机
        s.stack = this;
        stacks.Push(s);
        s.OnEnter();
    }
    public void PopSingleState()
    {//单行状态机
        StateBase s = stacks.Pop() as StateBase;//
        s.OnExit();
        //  s.stack = null;
    }

    public override void UpdateMS()
    {
        if (this.enable == false) return;

        StateBase s = stacks.Peek() as StateBase;
        if (s.Enable)
        {
            s.UpdateMS();
        }
        else
        {
            s.UpdateMSIdle();
        }
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
        GAObject s = stacks.Peek() as GAObject;
        s.OnEvent(what, userData);

    }

    public StateBase GetState<T>() where T : new()
    {
        StateBase s = stacks.Peek() as StateBase;
        if (s != null)
        {
            return s.GetState<T>();
        }
        return null;
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

    public StateMachine GetStateMachine()
    {
        return machine;
    }

    public string name = "";
    public int id = ID_UNKNOWN;

    public void SetEnable(bool b)
    {
        this.enable = b;
    }
    private bool enable = true;

    public StateMachine machine = null;
    Stack stacks = new Stack();
    public StateBase _current_state = null;
    public bool pause = false;
    public Entity host = null;
}

