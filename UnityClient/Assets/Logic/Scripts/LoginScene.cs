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

public class LoginScene : GAObject
{
    public override void OnEnter()
    {
        Utils.SetTargetFPS(60);
        AppMgr.ins.OnEnter();
        AppMgr.ins.LoadApp<LoginApp>();
    }

    public override void Update()
    {
        AppMgr.ins.Update();
        AppMgr.ins.UpdateMS();
    }

    public override void OnExit()
    {
        AppMgr.ins.OnExit();
    }
}

