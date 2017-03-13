
/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Timers;


public class ClientTestApp : GAObject
{
    public static ClientTestApp ins
    {
        get
        {
            return ClientTestApp.GetInstance();
        }
    }

    private static ClientTestApp _ins = null;

    public static ClientTestApp GetInstance()
    {
        if (_ins == null)
        {
            _ins = new ClientTestApp();
            _ins.Init();
            _ins.OnEnter();
        }
        return _ins;
    }




    public override bool Init()
    {

        base.Init();

        return true;
    }


    public override void OnEnter()
    {
        base.OnEnter();

        for (int i = 0; i < 1000000; i++)
        {
            TimerQueue.ins.AddTimer(10.0f, () =>
            {
                Debug.Log("1111111");

            });

        }

    }
    public override void OnDispose()
    {


    }

    public override void OnEvent(int type, object userData)
    {

    }

    public override void UpdateMS()
    {
        EventDispatcher.ins.Update();


    }



}
