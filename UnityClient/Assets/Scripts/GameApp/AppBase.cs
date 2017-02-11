using UnityEngine;
using System.Collections;


public class AppBase : GAObject
{
    public WorldMap GetCurrentWorldMap()
    {
        return this.worldMap;
    }
    public virtual string GetAppName()
    {
        return "AppBase";
    }
  
    protected WorldMap worldMap = null;
}
