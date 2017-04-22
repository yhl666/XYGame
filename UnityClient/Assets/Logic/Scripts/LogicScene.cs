/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LogicScene : GAObject
{
    public override void OnEnter()
    {
        Utils.SetTargetFPS(0xffffff);
        AppMgr.ins.OnEnter();
        AppMgr.ins.LoadApp<TownApp>();
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

