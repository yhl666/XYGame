/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
 


public class NewBehaviourScript : MonoBehaviour
{
    AppBase app;

    void Awake()
    {
        Utils.SetTargetFPS(0xffffff);
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
