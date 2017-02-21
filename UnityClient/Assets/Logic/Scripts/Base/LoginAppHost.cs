/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
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
