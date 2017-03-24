
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



    GameObject obj_a = null;
    GameObject obj_b = null;

    public override bool Init()
    {

        base.Init();

        return true;
    }


    public override void OnEnter()
    {
        base.OnEnter();

        for (int i = 0; i < 100; i++)
        {
            TimerQueue.ins.AddTimer(10.0f, () =>
            {
                Debug.Log("1111111");

            });

        }
        obj_a = GameObject.Instantiate(GameObject.Find("GameObject"));
        obj_b = GameObject.Instantiate(GameObject.Find("GameObject"));


    
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

        BoundsImpl a = BoundsImpl.Create(new Vector2(obj_a.transform.position.x, obj_a.transform.position.y), new Vector2(1, 1));
        BoundsImpl b = BoundsImpl.Create(new Vector2(obj_b.transform.position.x, obj_b.transform.position.y), new Vector2(1, 1));

        if (a.Intersects(b))
        {
            Debug.Log("cast");
        }
    }



}
