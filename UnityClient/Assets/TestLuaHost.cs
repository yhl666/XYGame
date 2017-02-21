/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using GameBox.Service.GiantLightFramework;
using GameBox.Framework;
using GameBox.Service.ByteStorage;
using System;

using LuaInterface;
using System.IO;


public class TestLuaHost : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        new ServiceTask("com.giant.service.giantlightframework").Start().Continue(task =>
        {
            this.game = task.Result as IGiantGame;
            PublicData.ins.game = this.game;
            this.game.GotoScene(new TestLuaScene());

            return null;

        });

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IGiantGame game = null;
}

public class LuaModelInterface : Model
{

    public override void UpdateMS()
    {
        try
        {
            LuaFunction func = table.GetLuaFunction("UpdateMS");

            func.BeginPCall();
            func.Push(this);
            func.PCall();

            func.EndPCall();
        }
        catch (LuaException e)
        {
            return;
        }

    }
    public override bool Init()
    {
        base.Init();
        return true;
    }

    public void Bind(LuaTable table)
    {
        this.table = table;
    }
    LuaTable table = null;
}



public class LuaBuffer : Buffer
{
    public string name = "";

    public override void UpdateMS()
    {
        base.UpdateMS();
        try
        {
            LuaFunction func = table.GetLuaFunction("UpdateMS");

            func.BeginPCall();
            func.Push(table);
            func.PCall();

            func.EndPCall();

            this.owner.hp = (int)(double)table["hp"];
        }
        catch (LuaException e)
        {
            Debug.LogError("LuaBuffer.UpdateMS  " + e.Message);

            return;
        }
    }




    public override bool Init()
    {
        base.Init();
        this.SetLastTime(5);

        if (l == null)
        {
            l = new LuaState();
            l.AddSearchPath(Application.dataPath + "/Lua");
            l.AddSearchPath(Application.dataPath + "/ToLua/Lua");
            l.AddSearchPath(Application.dataPath + "/ToLua");

            l.Start();

            LuaBinder.Bind(l);
            BindCustom.Bind(l);

            l.DoFile("Main.lua");
        }
        this.InitWithLua();

        return true;
    }

    public void InitWithLua()
    {
        l.LuaRequire("Model.Buffer5");
 
        int top = l.LuaGetTop();

        LuaTable t = l.CheckLuaTable(top);

        LuaFunction func = t.GetLuaFunction("new"); //create a new class

        func.BeginPCall();
        func.PCall();
        this.table = l.CheckLuaTable(l.LuaGetTop()); // get the table which new create
        table["hp"] = owner.hp;
     
        func.EndPCall();
    }

    LuaState l = null;
    LuaTable table = null;
}


public class BindCustom
{

    public static void Bind(LuaState l)
    {
        l.BeginModule(null);
        Bind_RpcClient(l);
        Bind_SceneMgr(l);
       // Bind_Model(l);
     //   Bind_LuaModel(l);




        l.EndModule();
    }

    private static void Bind_RpcClient(LuaState lua)
    {
        lua.BeginModule("RpcClient");

        lua.RegFunction("SendRequest", (System.IntPtr l) =>
        {
            int top = LuaDLL.lua_gettop(l);

            string service = LuaDLL.lua_tostring(l, top - 3);
            string method = LuaDLL.lua_tostring(l, top - 2);
            string para = LuaDLL.lua_tostring(l, top - 1);

            LuaTypes t = LuaDLL.lua_type(l, top);

            LuaFunction func = null;
            if (t == LuaTypes.LUA_TFUNCTION)
            {
                func = ToLua.ToLuaFunction(l, top);
            }
            else
            {
                Debug.LogWarning("[LUA]:cb not a function"); return 0;
            }

            RpcClient.ins.SendRequest(service, method, para, (string msg) =>
            {
                func.BeginPCall();
                func.Push(msg);
                func.PCall();
                func.EndPCall();
            });


            return 0;
        });


        lua.EndModule();
    }

    private static void Bind_SceneMgr(LuaState lua)
    {


        lua.BeginModule("SceneMgr");

        lua.RegFunction("Load", (System.IntPtr l) =>
        {

            int top = LuaDLL.lua_gettop(l);

            string name = LuaDLL.lua_tostring(l, top);
            SceneMgr.Load(name);
            return 0;
        });
        lua.EndModule();

    }


    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int UpdateMS(IntPtr L)
    {
        try
        {
            ToLua.CheckArgsCount(L, 1);
            Model obj = (Model)ToLua.CheckObject(L, 1, typeof(Model));
            //  System.Type arg0 = (System.Type)ToLua.CheckObject(L, 2, typeof(System.Type));

            obj.UpdateMS();

            return 0;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }


    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int NEW<T>(IntPtr L) where T : new()
    {
        try
        {
            ToLua.CheckArgsCount(L, 0);

            T o = new T();
            ToLua.Push(L, o);
            ModelMgr.ins.Add(o as Model);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }


    private static void Bind_Model(LuaState lua)
    {
        lua.BeginClass(typeof(Model), typeof(GAObject), "Model");

        lua.RegFunction("UpdateMS", UpdateMS);
        lua.RegFunction("new", NEW<Model>);

        lua.EndClass();

    }

    private static void Bind_LuaModel(LuaState lua)
    {
        lua.BeginClass(typeof(LuaModelInterface), typeof(Model), "LuaModel");

        lua.RegFunction("bind", bind_function);
        lua.RegFunction("new", NEW<LuaModelInterface>);

        lua.EndClass();

    }


    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int bind_function(IntPtr L)
    {
        try
        {
            ToLua.CheckArgsCount(L, 2);
            int top = LuaDLL.lua_gettop(L);

            LuaModelInterface obj = (LuaModelInterface)ToLua.CheckObject(L, top - 1, typeof(LuaModelInterface));

            var t = (LuaTable)ToLua.CheckLuaTable(L, top);
            obj.Bind(t);

            //  LuaTable t= ((LuaTable)ToLua.CheckObject(L, 1, typeof(LuaTable)));
            ////   obj.UpdateMS();

            return 0;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }
}


public class TestLuaScene : GiantLightSceneExtension
{
    public static LuaState lua = null;
    private bool init = false;
    public override void Enter(IGiantGame game)
    {
        if (init == true) return;
        init = true;
        base.Enter(game);

        Application.logMessageReceived += Log;



        lua = new LuaState();
        lua.AddSearchPath(Application.dataPath + "/Lua");
        lua.AddSearchPath(Application.dataPath + "/ToLua/Lua");
        lua.AddSearchPath(Application.dataPath + "/ToLua");

        lua.Start();

        LuaBinder.Bind(lua);
        BindCustom.Bind(lua);

        lua.DoFile("Main.lua");
        {
            LuaBuffer buf = new LuaBuffer();
            buf.Init();
            ModelMgr.ins.Add(buf);
            buf.name = "111111111111111";
        }


        {
            LuaBuffer buf = new LuaBuffer();
            buf.Init();
            ModelMgr.ins.Add(buf);
            buf.name = "22222222222222222";
        }


    }


    void Log(string msg, string stackTrace, UnityEngine.LogType type)
    {
        strLog += msg;
        strLog += "\r\n";
    }
    private string strLog = "";
    public override void Update(float delta)
    {
      ///  ModelMgr.ins.UpdateMS();
    }
}
