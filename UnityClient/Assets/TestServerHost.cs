/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using GameBox.Service.GiantLightFramework;
using GameBox.Framework;
using GameBox.Service.ByteStorage;



public class TestServerHost : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        new ServiceTask("com.giant.service.giantlightframework").Start().Continue(task =>
        {
            this.game = task.Result as IGiantGame;
            PublicData.ins.game = this.game;
            this.game.GotoScene(new TestServerScene());

            return null;

        });

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IGiantGame game = null;
}




public class TestServerScene : GiantLightSceneExtension
{
    private bool init = false;
    public override void Enter(IGiantGame game)
    {
        if (init == true) return;
        init = true;
        base.Enter(game);





        RpcClient.ins.SendRequest("services.battle_team", "create", "no:123,", (string msg) =>
           {
               Debug.Log(msg);
           });
        RpcClient.ins.SendRequest("services.battle_team", "create", "no:123,", (string msg) =>
        {
            Debug.Log(msg);
        });

        RpcClient.ins.SendRequest("services.battle_team", "create", "no:123,", (string msg) =>
        {
            Debug.Log(msg);
        });


        RpcClient.ins.SendRequest("services.battle_team", "join", "no:2,other:5,", (string msg) =>
        {
            Debug.Log(msg);
        });
        RpcClient.ins.SendRequest("services.battle_team", "search", "no:123,", (string msg) =>
        {
            Debug.Log(msg);
        });

        RpcClient.ins.SendRequest("services.battle_team", "random", "no:123", (string msg) =>
        {
            Debug.Log(msg);
        });
    }

}
