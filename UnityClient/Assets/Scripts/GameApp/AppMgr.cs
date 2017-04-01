/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;

public class AppMgr :  SingletonGAObject<AppMgr>
{

    //------------------------------------override 
    public override bool Init()
    {
        base.Init();

        return true;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
    public override void OnDispose()
    {
        base.OnDispose();
    }
    public override void OnEvent(int type, object userData)
    {

    }
    public override void Update()
    {
        base.Update();
        if (current_app != null) current_app.Update();
        AutoReleasePool.ins.Clear();

    }

    public override void UpdateMS()
    {
        base.UpdateMS();
        if (current_app != null) current_app.UpdateMS();
        AutoReleasePool.ins.Clear();

    }

    // ----------------controller 

    public void LoadApp<T>() where T : new()
    {
        this.Clear();
        this.Push(Create<T>());
    }
    public void ReplaceApp<T>() where T : new()
    {
        this.Pop();
        this.Push(Create<T>());
    }
    public void PushApp<T>() where T : new()
    {
        this.Push(Create<T>());
    }
    public void PopApp()
    {
        this.Pop();
    }
    public void AddParallelApp()
    {


    }
    public void RemoveParallelApp()
    {

    }
    public static AppBase Create<T>() where T : new()
    {
        AppBase ret = new T() as AppBase;
        ret.Init();
        return ret;
    }
    public static AppBase GetCurrentApp()
    {
        return current_app;
    }
    public static T GetCurrentApp<T>() where T : AppBase
    {
        return (T)current_app;
    }

    //-------------private helper function

    private void Push(AppBase app)
    {
        app.OnEnter();
        stack.Push(app);
        current_app = app;
    }
    private void Pop()
    {
        if (this.stack.Count <= 0)
        {
            current_app = null;
            return;
        }
        AppBase app = this.stack.Pop() as AppBase;
        app.OnExit();
    }
    private void Clear()
    {
        while (this.stack.Count > 0)
        {
            this.Pop();
        }
        this.stack.Clear();
        current_app = null;
    }

    private static AppBase current_app = null;
    private Stack stack = new Stack();
    private ArrayList paralles = new ArrayList();
}