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
        if (obj == null) return;
        this.InitWithData(obj);
    }
    public virtual void InitWithData(object obj)
    {

    }
    protected GameObject view = null;// bind view
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
        this.view.SetActive(true);
        this.Enable = true;
        ArrayList heros = HeroMgr.ins.GetHeros();
        foreach (Hero hero in heros)
        {
            if (this.y < hero.GetRealY()) continue;

            if (hero.ClaculateDistance(x, y) < distance)
            {
                this.Enable = false;
                tick.Reset();
                int delta= (int)((float) hero.hp  *  hp_percent/100.0f);
                hero.current_hp +=delta;
                Debug.LogError("Terrain 血包 回血 " + delta);
                this.view.SetActive(false);
                return;
            }
        }
    }

    public override void InitWithData(object obj)
    {
        TerrainObjectHpPackData data = obj as TerrainObjectHpPackData;
        if (data != null)
        {
            this.view = data.gameObject;
            this.hp_percent = data.hp_percent;
            this.x = data.gameObject.transform.position.x;
            this.y = data.gameObject.transform.position.y;
            this.name = "TerrainObjectHpPack";
            this.distance = data.distance;
            this.tick.SetMax((int)(data.cd_time * (float)Utils.fps));
            Debug.Log("TerrainObjectHpPackData  OK ");
            this.view.SetActive(false);
        }
        else
        {
            Debug.LogError("Error of TerrainObjectHpPack data");
        }
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
    public Vector2 next_point;
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
       ///     if (this.y < hero.GetRealY()-0.5) continue;

            if (hero.ClaculateDistance(x, y) < distance)
            {//传送点范围内
              //强制位移 到顶点
                hero.x = next_point.x;
                hero.y = next_point.y;
            }
        }
    }

    public override void InitWithData(object obj)
    {
        TerrainObjectTransformData data = obj as TerrainObjectTransformData;
        this.next_point = new Vector2(data.next_point.transform.position.x, data.next_point.transform.position.y);

        this.x = data.gameObject.transform.position.x;
        this.y = data.gameObject.transform.position.y;

    }
    public override bool Init()
    {
        base.Init();
        this.Enable = true;
        this.has_ui = false;
        return true;
    }
}