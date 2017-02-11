
using UnityEngine;
using System.Collections;
using GameBox.Service.GiantLightFramework;
using GameBox.Framework;
using System;
using System.Collections.Generic;

public class LogicScene : GiantLightSceneExtension
{
    public override void Enter(IGiantGame game)
    {
        base.Enter(game);
        app = AppBase.Create<SampleApp>();
        app.Init();
        app.OnEnter();
        AutoReleasePool.ins.Clear();
    }


    public override void Update(float delta)
    {
        base.Update(delta);
        app.Update();
    }


    public override void Exit(IGiantGame game)
    {
        base.Exit(game);
        app.OnExit();
        app.Dispose();
    }

    AppBase app = null;


}

