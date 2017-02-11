
using UnityEngine;
using System.Collections;
using GameBox.Service.GiantLightFramework;
using GameBox.Framework;
using GameBox.Service.ByteStorage;
using System;
using System.Collections.Generic;
using System.Threading;
using GameBox.Service.GiantLightServer;



//todo 不依赖于RpcMgr 和其 Scene 派发的依赖
// 改为GiantLightServer 和 GiantLightClient 处理网络


/// <summary>
/// 游戏本身层次服装
/// RpcMgr是对GameBox的封装
/// </summary>
public class RpcClient
{
    public static RpcClient ins
    {
        get
        {
            return RpcClient.GetInstance();
        }
    }

    private static RpcClient _ins = null;

    public static RpcClient GetInstance()
    {
        if (_ins == null)
        {
            _ins = new RpcClient();
        }
        return _ins;
    }



    /// <summary>
    /// 发起一个rpc请求
    /// </summary>
    /// <param name="service"></param>
    /// <param name="method"></param>
    /// <param name="kv_json">这样简单的json  "name:css,id:100,"</param>
    /// <param name="cb">回调参数是string 服务器响应数据 为空串表示错误 可进一步判定是超时还是服务器判定的错误等</param>
    public void SendRequest(string service, string method, string kv_json, VoidFuncString cb, bool timeout = false)
    {
        var t = new rpc.EnterRoomMsg();//protobuf
        t.peer_name = kv_json;
        ///   service = "xygame.services." + service;

        var wrapper = new Func<inner.ResponseWrapper, bool>((inner.ResponseWrapper resp) =>
        {//RPC 回调
            if (resp.ok)
            {// 处理成功
                cb(resp.json);
            }
            else
            {
                if (timeout && resp.timeout)//请求需要监听超时
                {
                    cb("timeout");
                }
                else
                {//响应超时 和服务器返回处理失败
                    cb("");
                }
            }

            return true;
        });

        //发送RPC请求
        inner.RpcMgr.ins.SendRequest(service, method, ByteConverter.ProtoBufToBytes(t), wrapper);
    }


    public void SendResponse(uint id, string kv_json)
    {

        var t = new rpc.EnterRoomMsg();//protobuf
        t.peer_name = kv_json;

        //发送RPC响应
        inner.RpcMgr.ins.SendResponse(id, ByteConverter.ProtoBufToBytes(t));
    }
    private IGiantLightServer server;
}

