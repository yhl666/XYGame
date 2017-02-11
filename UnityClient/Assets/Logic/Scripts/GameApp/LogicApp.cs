using UnityEngine;
using System.Collections;
using System;
public class LogicApp : AppBase
{

    // Use this for initialization
    public override bool Init()
    {
        base.Init();
        return true;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        RpcClient.ins.SendRequest("services.login", "login", "name:css,pwd:123,", (string msg) =>
            {
                if (msg == "")
                {
                    Debug.Log("error");
                }
                else
                {
                    HashTable kv = Json.Decode(msg);
                    Debug.Log(" login " + kv["res"]);
                }


            });
    }
    // Update is called once per frame
    public override void Update()
    {

    }


}
