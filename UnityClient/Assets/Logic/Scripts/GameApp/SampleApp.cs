/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;



/// <summary>
/// 和xygame 逻辑服务器交互例子
/// 尽量使用RpcClient的接口
/// 尽量不要使用RpcMgr的接口 破坏规范  
/// </summary>
public class SampleApp : AppBase
{

    public override bool Init()
    {
        base.Init();
        return true;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        //发起RPC请求
        RpcClient.ins.SendRequest("services.example", "login", "name:css,pwd:123,", (string msg) =>
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

    public override void Update()
    {
     
    }


}
/// <summary>
/// 注册服务器rpc 2种方法
/// 一种是注册自动查找的服务
/// 服务名字就是类名
/// 
/// 另外一种是RpcMgr.ins.RegistorServices
/// </summary>
namespace Services
{
    public class SampleServices : RpcService
    {

        public void method(string msg, VoidFuncString cb)
        {

            Json.Decode(msg);

            Debug.Log(msg);
            if (true)
            {//处理成功 返回json给服务端
                string ret = "ret:ok,";
                cb(ret);

            }
            else
            {//空串表示 失败，如果不调用cb 那么服务器会判定超时
                cb("");
            }
        }
    }


}