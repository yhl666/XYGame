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
/*

public class Client : IGiantLightClient
{
    public void OnConnect()
    {
        Debug.LogError("connect1");
    }

    public void OnDisconnect()
    {
        Debug.LogError("dis connect1");
    }
    public void Disconnect()
    {

    }

 

    public bool PushRequest(uint id, string service, string method, byte[] content)
    {
        return true;
    }
    public bool PushResponse(uint id, byte[] content)
    {
        return true;
    }


}




public class LoginAppHost111 : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        new ServiceTask("com.giant.service.giantlightserver").Start().Continue(task1 =>
        {
            var server = ServiceCenter.GetService<IGiantLightServer>();
            server.Connect("127.0.0.1", 8002, new Client());
       //     while (server.Connected == false)
            {
                if (server.Connected)
                {
                    Debug.LogError("1111");

                }
                else
                {
                    Debug.LogError("22222");
                }
            }
            server.Disconnect();
            return null;
        });


 

    }

    void Update()
    {

    }
    private IGiantLightServer server;
    private IGiantLightClient client;
    private IGiantGame game = null;
}
*/