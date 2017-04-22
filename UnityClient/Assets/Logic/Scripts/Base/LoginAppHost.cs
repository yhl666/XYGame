/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */

/*
using UnityEngine;
using System.Collections;
using GameBox.Service.GiantLightFramework;
using GameBox.Framework;
using GameBox.Service.ByteStorage;

public class LoginAppHost : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        new ServiceTask("com.giant.service.giantlightframework").Start().Continue(task =>
        {
            this.game = task.Result as IGiantGame;

            this.game.GotoScene(new LoginScene());
            PublicData.GetInstance().game = this.game;
            return null;

        });

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IGiantGame game = null;
}
*/










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


public class LoginAppHost : MonoBehaviour
{
    LoginScene scene = null;
    void Awake()
    {

    }
    void Start()
    {
        this.enabled = false;
        new ServiceTask("com.giant.service.giantlightserver").Start().Continue(task1 =>
       {
           RpcClient.ins.LazyInit();
           scene = new LoginScene();
           scene.Init();
           scene.OnEnter();
           this.enabled = true;
           RpcClient.ins.ReConnect();
           return null;
       });
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
