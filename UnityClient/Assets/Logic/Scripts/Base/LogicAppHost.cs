/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using GameBox.Service.GiantLightFramework;
using GameBox.Framework;
using GameBox.Service.ByteStorage;
using GameBox.Service.GiantLightServer;
using System;

public class LogicAppHost : MonoBehaviour
{
    LogicScene scene = null;
    void Awake()
    {

    }
    void Start()
    {
     
        if (RpcClient.ins.IsConnected() == false && RpcClient.ins.IsReady()==false)
        {
            this.enabled = false;
            new ServiceTask("com.giant.service.giantlightserver").Start().Continue(task1 =>
            {
                RpcClient.ins.LazyInit();
                scene = new LogicScene();
                scene.Init();
                scene.OnEnter();
                this.enabled = true;
                RpcClient.ins.ReConnect();
                return null;
            });
        }
        else
        {
            RpcClient.ins.LazyInit();
            scene = new LogicScene();
            scene.Init();
            scene.OnEnter();
        }
    }

    void Update()
    {
        if (RpcClient.ins.IsConnected() == false)
        {
            if (has_intime == false)
            {
                has_intime = true;
                StartCoroutine(TryReConnect());
            }
        }
        if (scene != null)
        {
            scene.Update();
        }
    }
    void OnDestroy()
    {
        if (scene != null)
        {
            scene.OnExit();
            scene.Dispose();
        }
    }

    IEnumerator TryReConnect()
    {
        yield return new WaitForSeconds(3);
        RpcClient.ins.ReConnect();
        has_intime = false;
    }
    bool has_intime = false;
}

