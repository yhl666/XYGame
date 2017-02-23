/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */

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
        Application.targetFrameRate = 0xfffffff;
        AppMgr.ins.OnEnter();
        AppMgr.ins.LoadApp<TownApp>();
    }

    public override void Update(float delta)
    {
        base.Update(delta);
       /// AppMgr.ins.Update();
        AppMgr.ins.UpdateMS();
    }


    public override void Exit(IGiantGame game)
    {
        base.Exit(game);
        AppMgr.ins.OnExit();
    }



}

