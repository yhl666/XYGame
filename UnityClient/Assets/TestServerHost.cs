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









        RpcClient.ins.SendRequest("services.login", "login", "account:1,pwd:1,", (string msg) =>
        {
            Debug.Log(msg);

        });


    }

}
