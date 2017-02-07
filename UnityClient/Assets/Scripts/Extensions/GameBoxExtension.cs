
/*


 

using System.Collections;
using GameBox.Service.GiantLightFramework;
using GameBox.Framework;
using GameBox.Service.ByteStorage;
using System;
using System.Collections.Generic;
using UnityEngine;

using GameBox.Service.GiantLightFramework.Extension;



// GameBox的 一个 RPC扩展

// 每个RPC请求(id自动增长)对应一个 匿名函数用于处理服务器响应
//可以向RpcMgr注册服务 为服务端 提供rpc 本地化调用
// 添加超时处理 
 





//
// ----------------------------------------------------------------------------------
//                                            example
// ----------------------------------------------------------------------------------


/// <summary>
/// 该服务类型是rpc.test3  提供了 func方法
/// 注意提供服务的方法 必须是public
/// </summary>
public class RpcTest3 : RpcService
{
    public override string GetServiceName()
    {
        return "rpc.test3";
    }
    public void func(uint id, byte[] content)
    {
        Debug.Log("rpc.func");
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
        RpcMgr.ins.RegisterService(new RpcTest3());  //注册该服务
        var t = new mtanks.Test();//protobuf

        //发送RPC请求
        RpcMgr.ins.SendRequest("rpc.test1", "test1", ByteConverter.ProtoBufToBytes(t), new Func<ResponseWrapper, bool>((ResponseWrapper resp) =>
        {//RPC 回调
            if (resp.code.ok)
            {// 处理成功
                Debug.Log("resp " + resp.id);
                var tt = ByteConverter.BytesToProtoBuf<mtanks.Test>(resp.content, 0, resp.content.Length);
                Debug.Log(tt.id);
            }
            else if (resp.code.code == ResponseCode.ID_TIMEOUT)
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



namespace GameBox.Service.GiantLightFramework.Extension
{

    /// <summary>
    /// 该场景接管了 GiantLightScene，想用该扩展，请把场景继承自 GiantLightSceneExtension
    /// </summary>
    public class GiantLightSceneExtension : GiantLightScene
    {
        public override void Enter(IGiantGame game)
        {
            base.Enter(game);
            RpcMgr.ins.OnChangeScene(this);

        }


        public override void Update(float delta)
        {
            base.Update(delta);
            RpcMgr.ins.Tick();
        }


        public override void Exit(IGiantGame game)
        {
            base.Exit(game);

        }


        public override sealed bool PushResponse(uint id, byte[] content)
        {
            return RpcMgr.ins.PushResponse(id, content);
        }
        public override sealed bool PushRequest(uint id, string service, string method, byte[] content)
        {
            return RpcMgr.ins.PushRequest(id, service, method, content);
        }
    }

    /// <summary>
    /// RPC 请求
    /// </summary>
    public class RequestWrapper
    {
        public uint id = 0;
        public string method;
        public string service;
        public byte[] content = null;

        private const uint TIMEOUT_TICK = 60;//超时的帧数
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
    /// RPC 响应代码
    /// </summary>
    public class ResponseCode
    {
        public const uint ID_OK = 0;
        public const uint ID_TIMEOUT = 1;
        public uint code = ID_OK;
        public bool ok = true;
    }
    /// <summary>
    /// RPC 响应
    /// </summary>
    public class ResponseWrapper
    {
        public uint id;
        public byte[] content;
        RequestWrapper request;
        public ResponseCode code = new ResponseCode();
        public ResponseWrapper(RequestWrapper request, byte[] content)
        {
            if (content == null)
            {
                code.code = ResponseCode.ID_TIMEOUT;
                code.ok = false;
            }
            this.request = request;
            this.id = request.id;
            this.content = content;
        }
        public uint ID_STATE = 0;
    }

    /// <summary>
    /// RPC 服务，提供给服务端
    /// </summary>
    public class RpcService : object
    {
        public virtual string GetServiceName() { return ""; }

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
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool PushRequest(uint id, string service, string method, byte[] content)
        {
            if (this.services.Contains(service))
            {
                RpcService rpc = this.services[service] as RpcService;

                Type type = rpc.GetType();
                object[] _params = new object[2];
                _params[0] = id;
                _params[1] = content;
                var func = type.GetMethod(method);
                if (func == null)
                {
                    Debug.LogWarning("Unknown method " + method);
                }
                else
                {
                    func.Invoke(rpc, _params);
                }
            }
            else
            {
                Debug.LogWarning("Unknown service " + service);
            }
            return true;
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
};
*/