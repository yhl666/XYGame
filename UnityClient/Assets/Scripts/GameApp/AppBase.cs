﻿using UnityEngine;
using System.Collections;


public class AppBase : GAObject
{
    public static AppBase Create<T>() where T : new()
    {
        AppBase ret = new T() as AppBase;
        ret.Init();
        ret.OnEnter();
        return ret;
    }
    protected WorldMap worldMap = null;
    public WorldMap GetCurrentWorldMap()
    {
        return this.worldMap;
    }
    public static AppBase GetCurrentApp()
    {
        return current_app;
    }
    public static T GetCurrentApp<T>() where T : AppBase
    {
        return (T)current_app;
    }


    protected static AppBase current_app;
}
