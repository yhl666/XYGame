/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
using Spine.Unity;


public class NewBehaviourScript : MonoBehaviour
{
    AppBase app;

    void Awake()
    {
     Application.targetFrameRate = 999999;
    }
    // Use this for initialization
    void Start()
    {

        AppMgr.ins.LoadApp<BattleApp>();

    }

    // Update is called once per frame
    void Update()
    {
        AppMgr.ins.Update();
    }

}
