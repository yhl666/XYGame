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
    public float z = 0.0f;
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
            ///  if (this.y < hero.GetRealY()) continue;
            float dis = hero.ClaculateDistance(x, 0, y);
            if (dis < distance)
            {
                this.Enable = false;
                tick.Reset();
                int delta = (int)((float)hero.hp * hp_percent / 100.0f);
                hero.current_hp += delta;
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
    public override void InitWithData(object obj)
    {
        TerrainObjectRevivePointData data = obj as TerrainObjectRevivePointData;
        this.x = data.gameObject.transform.localPosition.x;
        this.y = data.gameObject.transform.localPosition.y;
        this.name = data.gameObject.name;
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
    private bool has_init = false;
    public override void UpdateMS()
    {
        if (Enable == false || has_init == false)
        {
            return;
        }
        if (tick.Tick())
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
                hero.SetRealY(next_point.y);
                tick.Reset();
                hero.AddTag("terrain_limit", AppMgr.GetCurrentApp<BattleApp>().GetCurrentWorldMap().GetTerrainPlatform().GetBlock("limit"));
                //TODO 死亡后 移除
            }
        }
    }
    public override void InitWithData(object obj)
    {
        TerrainObjectTransformData data = obj as TerrainObjectTransformData;
        if (data.next_point == null) return;

        this.next_point = new Vector2(data.next_point.transform.localPosition.x, data.next_point.transform.localPosition.y);
        this.tick.SetMax(data.cd);
        this.x = data.gameObject.transform.localPosition.x;
        this.y = data.gameObject.transform.localPosition.y;
        this.has_init = true;
    }
    public override bool Init()
    {
        base.Init();
        this.Enable = true;
        this.has_ui = false;
        return true;
    }
    Counter tick = Counter.Create(40); //cd for transform
}


/// <summary>
/// Enemy 启动器
/// </summary>
class EnemyLauncher : Model
{
    public OneEnemyBornData data = null;

    public static EnemyLauncher Create(OneEnemyBornData data)
    {
        EnemyLauncher ret = new EnemyLauncher();
        ret.Init();
        ret.data = data;
        return ret;
    }
    public override void UpdateMS()
    {
        if (tick.Tick())
        {
            return;
        }
        Enemy enemy = EnemyMgr.Create(data.enemy_type);
        enemy.hp = data.hp;
        enemy.current_hp = enemy.hp;
        enemy.team = 333;
        enemy.x = data.gameObject.transform.localPosition.x;
        enemy.z = data.gameObject.transform.localPosition.y;
        enemy.y = 2;
        this.SetInValid();
    }
    public override void OnEnter()
    {
        tick.SetMax(Utils.ConvertToFPS(data.time));
    }

    Counter tick = Counter.Create();
}

/// <summary>
/// 刷新点
/// 只提供数据
/// </summary>
public class TerrainObjectEnemyBornPoint : CustomObject
{
    public override bool Init()
    {
        base.Init();

        return true;
    }
    public override void UpdateMS()
    {

    }
    public override void InitWithData(object obj)
    {
        TerrainObjectEnemyBornPointData data = obj as TerrainObjectEnemyBornPointData;
        this.x = data.gameObject.transform.localPosition.x;
        this.y = data.gameObject.transform.localPosition.y;
        this.z = data.gameObject.transform.localPosition.z;
        this.name = data.gameObject.name;
        OneEnemyBornData[] enemys = data.gameObject.GetComponents<OneEnemyBornData>();
        foreach (OneEnemyBornData enemy in enemys)
        {//初始化 启动器
            EnemyLauncher launch = EnemyLauncher.Create(enemy);  // ModelMgr.Create<EnemyLauncher>();
            ModelMgr.ins.Add(launch);
        }
        Debug.Log("TerrainObjectEnemyBornPoint  OK " + enemys.Length);
    }
}
