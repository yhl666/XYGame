using UnityEngine;
using System.Collections;
using System;
using Spine.Unity;


public class NewBehaviourScript : MonoBehaviour
{
    AppBase app;

    void Awake()
    {
     //  Application.targetFrameRate = 999999;
    }
    // Use this for initialization
    void Start()
    {

        app = AppBase.Create<BattleApp>();
        

    }

    // Update is called once per frame
    void Update()
    {
        app.Update();
    }

}
