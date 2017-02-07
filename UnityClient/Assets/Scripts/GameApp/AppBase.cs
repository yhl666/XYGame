using UnityEngine;
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

}
