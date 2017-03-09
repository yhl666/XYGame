/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

/// <summary>
/// 地图逻辑对象
/// 加血包  子弹等
/// </summary>
public class CustomObject : Model
{
    public float x = 0.0f;
    public float y = 0.0f;
    public string name = "";
    public bool has_ui = true;
    public bool Enable = true;

    public void LoadWithData(object obj)
    {
        this.InitWithData(obj);
    }
    public virtual void InitWithData(object obj)
    {

    }
}


/// <summary>
/// 加血包
/// </summary>
public class TerrainObjectHpPack : CustomObject
{
    public float distance = 0.5f;
    private float hp_percent = 0.0f;
    public override void UpdateMS()
    {
        if (Enable == false && tick.Tick())
        {//wait for sync
            return;
        }
        this.Enable = true;
        ArrayList heros = HeroMgr.ins.GetHeros();
        foreach (Hero hero in heros)
        {
            if (this.y < hero.GetRealY()) continue;

            if (hero.ClaculateDistance(x, y) < distance)
            {
                this.Enable = false;
                tick.Reset();
                hero.current_hp +=(int)((float) hero.hp  *  hp_percent/100.0f);
                return;
            }
        }
    }

    public override void InitWithData(object obj)
    {
        TerrainObjectHpPackData data = (TerrainObjectHpPackData)obj;
        this.hp_percent = data.hp_percent;
        this.x = data.gameObject.transform.position.x;
        this.y = data.gameObject.transform.position.y;
        this.name = "TerrainObjectHpPack";
        this.distance = data.distance;
    }

    public override bool Init()
    {
        base.Init();
        this.Enable = false;
        this.has_ui = false;
        return true;
    }
    Counter tick = Counter.Create((int)(30.0f * Utils.fps));
}




/// <summary>
/// 复活点
/// 只提供复活数据真正的逻辑在Entity里面做
/// </summary>
public class TerrainObjectRevivePoint : CustomObject
{

    public override bool Init()
    {
        base.Init();

        return true;
    }
    public override void UpdateMS()
    {
       
    }
}





/// <summary>
/// 传送门
/// 处于范围内 可直接传送到顶点
/// 
/// </summary>
public class TerrainObjectTransform : CustomObject
{
    public float distance = 0.5f;

    public override void UpdateMS()
    {
        if (Enable == false )
        {
            return;
        }
        this.Enable = true;
        ArrayList heros = HeroMgr.ins.GetHeros();
        foreach (Hero hero in heros)
        {
            if (this.y < hero.GetRealY()) continue;

            if (hero.ClaculateDistance(x, y) < distance)
            {//传送点范围内
              //强制位移 到顶点

                
            }
        }
    }


    public override bool Init()
    {
        base.Init();
        this.Enable = false;
        this.has_ui = false;
        return true;
    }
}