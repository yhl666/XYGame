using UnityEngine;
using System.Collections;
using GameBox.Service.GiantLightFramework;
using GameBox.Framework;
using GameBox.Service.ByteStorage;
using mtanks;
using System;
using System.Collections.Generic;
using System.Threading;

  class test : GiantLightSceneExtension
{
    public override void Enter(IGiantGame game)
    {
        base.Enter(game);

        /*RpcClient.ins.SendRequest("login", "login","hahaha",new Func<ResponseWrapper, bool>((ResponseWrapper resp) =>
        {//RPC 回调
            if (resp.ok)
            {// 处理成功

                Debug.Log(resp.json);
            }
            else if (resp.timeout)
            {//响应超时
                Debug.Log("time out");
            }
            return true;
        }));
        */
    }


    public override void Update(float delta)
    {
        base.Update(delta);
    }


    public override void Exit(IGiantGame game)
    {
        base.Exit(game);
    }




}




public class TestRPC : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        new ServiceTask("com.giant.service.giantlightframework").Start().Continue(task =>
            {
                this.game = task.Result as IGiantGame;



                this.game.GotoScene(new test());


                return null;

            });

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IGiantGame game = null;
}







