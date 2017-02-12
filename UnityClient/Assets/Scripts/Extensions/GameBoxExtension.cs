

using System.Collections;
using GameBox.Service.GiantLightFramework;
using GameBox.Framework;
using GameBox.Service.ByteStorage;
using System;
using System.Collections.Generic;
using UnityEngine;

// GameBox的 一个 RPC扩展

// 每个RPC请求(id自动增长)对应一个 匿名函数用于处理服务器响应
//可以向RpcMgr注册服务 为服务端 提供rpc 本地化调用
// 添加超时处理 

//在逻辑 部分 请使用
/*
 * 
 * RpcClient的接口使用 闭包 和简单数据类型，示例SampleApp.cs
 * RpcMgr 的接口 使用原始框架的低级封装
 * RpcClient.cs 也有说明
*/


//
// ----------------------------------------------------------------------------------
//                                            example
// ----------------------------------------------------------------------------------


/// <summary>
/// 该服务类型是rpc.test3  提供了 func方法
/// 注意提供服务的方法 必须是public
/// </summary>
public class rpctest4 : RpcService
{
    public override string GetServiceName()
    {
        return "rpc.test3";
    }
    public void func(string msg, VoidFuncString cb)
    {
        Debug.Log("rpc.func  " + msg);
        cb("res:okkk,");
    }
}

/// <summary>
/// 
/// 该场景示例了扩展    game.GotoScene(new GiantLightSceneExtensionTest());
/// </summary>
public class GiantLightSceneExtensionTest : GiantLightSceneExtension
{
    public override void Enter(IGiantGame game)
    {
        base.Enter(game);
        inner.RpcMgr.ins.RegisterService(new RpcService());  //注册该服务
        var t = new mtanks.Test();//protobuf

        //发送RPC请求
        inner.RpcMgr.ins.SendRequest("rpc.test1", "test1", ByteConverter.ProtoBufToBytes(t), new Func<inner.ResponseWrapper, bool>((inner.ResponseWrapper resp) =>
        {//RPC 回调
            if (resp.ok)
            {// 处理成功
                Debug.Log("resp " + resp.json);

            }
            else if (resp.timeout)
            {//响应超时
                Debug.Log("time out");
            }
            return true;
        }));

    }


    public override void Update(float delta)
    {
        base.Update(delta);
    }


    public override void Exit(IGiantGame game)
    {
        base.Exit(game);
    }




}



//----------------------------------------------------------------------------------
//                                            Extension
// ----------------------------------------------------------------------------------




/// <summary>
/// 该场景接管了 GiantLightScene，想用该扩展，请把场景继承自 GiantLightSceneExtension
/// </summary>
public class GiantLightSceneExtension : GiantLightScene
{
    public override void Enter(IGiantGame game)
    {
        base.Enter(game);
        inner.RpcMgr.ins.OnChangeScene(this);

    }


    public override void Update(float delta)
    {
        base.Update(delta);
        inner.RpcMgr.ins.Tick();
    }


    public override void Exit(IGiantGame game)
    {
        base.Exit(game);

    }


    public override sealed bool PushResponse(uint id, byte[] content)
    {
        return inner.RpcMgr.ins.PushResponse(id, content);
    }
    public override sealed bool PushRequest(uint id, string service, string method, byte[] content)
    {
        return inner.RpcMgr.ins.PushRequest(id, service, method, content);
    }
}




/// <summary>
/// RPC 服务，提供给服务端
/// 服务器可以控制GAObject的事件 
/// </summary>
public class RpcService : GAObject
{
    public virtual string GetServiceName() { return ""; }
    public void _Inner_Invoke__(uint id, string method, byte[] content)
    {
        if ("_Inner_Invoke__" == method) return;
        if ("GetServiceName" == method) return;


        string msg = ByteConverter.BytesToProtoBuf<rpc.EnterRoomMsg>(content, 0, content.Length).peer_name;
        VoidFuncString cb = new VoidFuncString((string kv_json) =>
        {
            var t = new rpc.EnterRoomMsg();//protobuf
            t.peer_name = kv_json;
            inner.RpcMgr.ins.SendResponse(id, ByteConverter.ProtoBufToBytes(t));
        });


        VoidFuncRpc func = null;
        if (methods.Contains(method))
        {
            func = methods[method] as VoidFuncRpc;
            Debug.Log("call func in cache");

        }
        else
        {// create wrapper and add to cache
            Type type = this.GetType();

            var method_func = type.GetMethod(method);
            if (method_func == null)
            {
                Debug.LogWarning("Unknown method " + method);
                RpcClient.ins.SendResponse(id, "");
                return;
            }
       
            func = (string msgg, VoidFuncString cbb) =>
            {
                object[] _params = new object[2];
                _params[0] = msgg as string;
                _params[1] = cbb as VoidFuncString;
                method_func.Invoke(this, _params);
            };
            methods.Add(method, func);

            Debug.Log("call func in new");
        }


        func(msg, cb);

    }



    private Hashtable methods = new Hashtable();
}



/*
 * 
 
 -+---------------------尽量不要使用以下接口
 */

namespace inner
{


    /// <summary>
    /// RPC 请求
    /// </summary>
    public class RequestWrapper
    {
        public uint id = 0;
        public string method;
        public string service;
        public byte[] content = null;

        private uint TIMEOUT_TICK = (uint)(3.0f * (1.0f / Time.deltaTime));//超时的帧数 3秒超时
        private uint _tick = 0;
        private Func<ResponseWrapper, bool> cb;
        public bool isInVoke = false;

        public RequestWrapper(string service, string method, byte[] content, Func<ResponseWrapper, bool> cb)
        {
            this.id = ++idx;
            this.method = method;
            this.service = service;
            this.content = content;
            this.cb = cb;
        }
        public bool InVoke(byte[] content)
        {
            if (this.isInVoke) return false;
            this.isInVoke = true;
            var wrapper = new ResponseWrapper(this, content);
            return cb(wrapper);

        }
        public void Tick()
        {//check for time out 
            if (this.isInVoke) return;
            ++_tick;
            if (_tick > TIMEOUT_TICK)
            {
                this.InVoke(null);
            }
        }

        private static uint idx = 0;//global request id
    }
    /// <summary>
    /// RPC 响应
    /// </summary>
    public class ResponseWrapper
    {
        public uint id;
        //   private  byte[] content;
        //    private   RequestWrapper request;
        public string json; // fast way to decode
        public bool ok = true; // is respone ok ?
        public bool timeout = false; // time out
        public ResponseWrapper(RequestWrapper request, byte[] content)
        {
            if (content == null)
            {
                timeout = true;
                ok = false;
            }
            else
            {
                this.id = request.id;
                this.json = ByteConverter.BytesToProtoBuf<rpc.EnterRoomMsg>(content, 0, content.Length).peer_name;
            }
        }
        public uint ID_STATE = 0;
    }


    /// <summary>
    /// RPC管理器
    /// @brief  接管GiantLightScene的PushRequest 和 PushResponse
    /// 所有RPC 注册 请求 都用该类 提供的接口
    /// </summary>
    public class RpcMgr
    {

        /// <summary>
        /// 注册一个服务
        /// </summary>
        /// <param name="service"></param>
        public void RegisterService(RpcService service)
        {
            if (this.services.Contains(service.GetServiceName()))
            {
                Debug.LogWarning("Service has been register :" + service.GetServiceName());
                return;
            }
            services.Add(service.GetServiceName(), service);
        }

        public void ClearCache()
        {
            this.services.Clear();
        }
        /// <summary>
        /// 本地超时心跳
        /// </summary>
        public void Tick()
        {
            foreach (RequestWrapper request in requests)
            {
                request.Tick();
            }

            //clear 
            for (int i = 0; i < requests.Count; )
            {
                if ((requests[i] as RequestWrapper).isInVoke)
                {
                    requests.Remove(requests[i]);
                }
                else
                {
                    ++i;
                }
            }
        }
        /// <summary>
        /// 接管代码， 在GiantLightScene里面转发到该函数，其他时刻请勿调用
        /// 处理客户端发起的rpc的 服务器响应
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool PushResponse(uint id, byte[] content)
        {
            foreach (RequestWrapper request in requests)
            {
                if (request.id == id)
                {
                    return request.InVoke(content);
                }
            }
            Debug.LogWarning("Unknown response id:" + id);
            return false;
        }
        /// <summary>
        /// 接管代码， 在GiantLightScene里面转发到该函数，其他时刻请勿调用
        /// 处理来自服务器的rpc调用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool PushRequest(uint id, string service, string method, byte[] content)
        {
            RpcService rpc;
            service = "Services." + service;

            if (this.services.Contains(service))
            {
                rpc = this.services[service] as RpcService;
            }
            else
            {
                Type t = Type.GetType(service);
                if (t == null)
                {
                    RpcClient.ins.SendResponse(id, "");//return error
                    Debug.LogWarning("UnKnown services:" + service);
                    return true;
                }
                Debug.Log("[Server Rpc Call]:" + service + " add to cache");
                rpc = Activator.CreateInstance(t) as RpcService;
                services.Add(service, rpc);
            }
            Debug.LogWarning("[Server Rpc Call]:" + service + "." + method);
            rpc._Inner_Invoke__(id, method, content);
            return true;
        }

        /// <summary>
        /// 发出服务器发起的rpc请求 的响应
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        public void SendResponse(uint id, byte[] content)
        {

            scene.SendResponse(id, content);
        }
        /// <summary>
        /// 发送RPC请求接口
        /// </summary>
        /// <param name="service"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <param name="cb"> 回调，参数是ResponseWrapper 该请求响应后会自动调用回调</param>
        public void SendRequest(string service, string method, byte[] content, Func<ResponseWrapper, bool> cb)
        {
            Debug.LogWarning("[Client Rpc Call]:" + service + "." + method);
            var wrapper = new RequestWrapper(service, method, content, cb);
            requests.Add(wrapper);
            scene.SendRequest(wrapper.id, service, method, content);
        }

        /// <summary>
        /// 切换场景后，切换接管的 GiantLightScene
        /// </summary>
        /// <param name="scene"></param>
        public void OnChangeScene(GiantLightScene scene)
        {
            this.scene = scene;
        }
        public void Remove(RequestWrapper wrapper)
        {
            this.requests.Remove(wrapper);
        }
        private RpcMgr()
        {
            _ins = this;
        }
        public static RpcMgr ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new RpcMgr();
                }
                return _ins;
            }
        }

        private static RpcMgr _ins = null;
        Hashtable services = new Hashtable();

        List<RequestWrapper> requests = new List<RequestWrapper>();

        private GiantLightScene scene = null;

    }
}