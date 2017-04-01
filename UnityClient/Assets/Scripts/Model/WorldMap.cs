/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
public class WorldMap : Model
{

    public Terrain GetTerrain()
    {
        return terrain;
    }

    public string GetWorldMapName()
    {
        return "城镇中心";
    }
    public TerrainPlatform GetTerrainPlatform()
    {
        return platform;
    }

    /// <summary>
    ///   自动切割 Entity 到 合法的X坐标
    /// Entity的x值 和target 的x值 做切割
    /// 
    /// 如果target位于地形块内，那么会设置成一个合法的坐标X点
    /// 比如实现闪现机制 会撞墙
    /// </summary>
    /// <param name="who">需要设置x坐标的Entity</param>
    /// <param name="target">目标x点</param>
    public void ClipPositionX(Entity who, float target)
    {
        var block = terrain.GetBlock(target);

        if (block != null)
        {
            if (who.GetRealY() >= block.height)
            {//没有往更高处走，不会撞墙 直接设置
                who.x = target; ;
            }
            else
            {//撞墙，处理 目标点的海拔更高，因此要处理为撞墙
                if (who.x > target)
                {//左撞墙
                    if (block.x_right >= terrain.limit_x_left)
                    {
                        who.x = block.x_right;
                    }
                    else
                    {
                        who.x = terrain.limit_x_right;
                    }
                }
                else
                {//右撞墙
                    if (block.x_left >= terrain.limit_x_left)
                    {
                        who.x = block.x_left;
                    }
                    else
                    {
                        who.x = terrain.limit_x_left;
                    }
                }
            }
        }
    }
    /// <summary>
    ///  this will 
    /// </summary>
    public override void UpdateMS()
    {


    }
    /// <summary>
    /// 将Entity 坐标 转换为 unity世界坐标
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public Vector2 ConvertToWorldSpace(Entity obj)
    {
        return new Vector2(0, 0);
    }
    public ArrayList GetCustomObjects()
    {
        return custom_objs;
    }
    public ArrayList GetCustomObjects<T>() where T : CustomObject
    {
        Type t = typeof(T);
        if (t == typeof(TerrainObjectRevivePoint))
        {
            return custom_objs_revivepoints;
        }

        ArrayList ret = new ArrayList();
        foreach (CustomObject obj1 in custom_objs)
        {
            if ( obj1.GetType() == t)
            {
                ret.Add(obj1);
            }
        }
        return ret;
    }
    public ArrayList GetRevivePoints()
    {
        return custom_objs_revivepoints;
    }

    protected ArrayList custom_objs = new ArrayList();
    protected ArrayList custom_objs_revivepoints = new ArrayList();
    protected ArrayList objs = new ArrayList();
    protected Terrain terrain = null;
    protected TerrainPlatform platform = null;
}


public sealed class BattleWorldMap : WorldMap
{
    /// <summary>
    /// 用这个地图 更新Entity世界信息
    /// </summary>
    /// <param name="what"></param>
    private void UpdateEntity(Entity what)
    {
        //   
        /*if (false == this.UpdateWithPlaform(what))
        {
            this.UpdateWithTerrain(what);
        }*/
    }


    /// <summary>
    ///  更新地形，更新  Entity的海拔高度
    /// </summary>
    /// <param name="what"></param>
    private void UpdateWithTerrain(Entity what)
    {
        float x = what.x;
        float y;
        TerrainBlock block = terrain.GetBlock(x);
        if (block != null)
        {
            y = block.height;
            if (y == what.height)
            {
                if ((block.x_right + block.x_left) / 2.0f < x)
                {
                    what.isInOneTerrainRight = true;
                }
                else
                {
                    what.isInOneTerrainRight = false;
                }
            }
            what.height = y;
        }
    }
    private bool UpdateWithPlaform(Entity what)
    {
        {
            var pair = what.GetTagPair("terrain_limit");
            if (pair == null)
            {//未找到

            }
            else
            {
                var block = pair.value as TerrainBlock;

                if (what.x < block.x_left)
                {
                    what.x = block.x_left;
                }
                else if (what.x > block.x_right)
                {
                    what.x = block.x_right;
                }

                what.height_platform = block.height;

                return true;
            }
        }
        float x = what.x;

        ArrayList blocks = platform.GetBlocks(x);

        if (blocks.Count > 0)
        {
            float MAX_HEIGHT = 0f;
            TerrainBlock current_block = null;
            foreach (TerrainBlock block in blocks)
            {

                if (block.name == "limit")
                {
                    if (what.HasTag("terrain_limit") == false)
                    {//未上来
                        if (what.GetRealY() >= block.height - 0.5f)
                        {
                            if (Mathf.Abs(what.x - block.x_left) < 0.1f)
                            {
                                what.x = block.x_left;

                            }
                            else if (Mathf.Abs(what.x - block.x_right) < 0.1f)
                            {
                                what.x = block.x_right;
                            }
                        }

                        if (what.GetRealY() >= block.height - 1f)
                        {
                            if (what.x > block.x_left && what.x < block.x_right)
                            {
                                what.SetRealY(block.height - 1f);
                            }
                        }

                        continue;
                    }
                }
                float y = block.height;

                if (y == what.GetRealY())
                {//平台没变直接放弃处理
                    return true;
                }

                if (what.GetRealY() > y)
                {
                    MAX_HEIGHT = Mathf.Max(y, MAX_HEIGHT);
                    current_block = block;
                }
            }
            if (MAX_HEIGHT == 0f)
            {// 没选出比现在更高的平台
                return false;
            }

            what.height_platform = MAX_HEIGHT;

            return true;
        }
        else
        {

        }
        return false;

    }
    public override bool Init()
    {
        base.Init();

        ///---------------------- 初始化地图 地形 和平台
        terrain = ModelMgr.Create<Terrain>();
        platform = ModelMgr.Create<TerrainPlatform>();

        EventDispatcher.ins.AddEventListener(this, Events.ID_BEFORE_ONEENTITY_UPDATEMS);

        //----------------初始化CustomObjects

        GameObject obj_terrain = GameObject.Find("Terrain");
        {// -- init revive points
            Transform p = obj_terrain.transform.FindChild("RevivePoints");
            if (p == null) return true;
            Transform[] objs = p.GetComponentsInChildren<Transform>();
            foreach (Transform obj in objs)
            {
                TerrainObjectRevivePointData data = obj.gameObject.GetComponent<TerrainObjectRevivePointData>();
                if (data == null) continue;

                CustomObject t = ModelMgr.Create<TerrainObjectRevivePoint>();
                t.LoadWithData(data);
                this.custom_objs_revivepoints.Add(t);
                this.custom_objs.Add(t);

            }
        }


        {// -- init hp pack
            Transform p = obj_terrain.transform.FindChild("HpPacks");
            if (p == null) return true;
            Transform[] objs = p.GetComponentsInChildren<Transform>();

            foreach (Transform obj in objs)
            {
                TerrainObjectHpPackData data = obj.gameObject.GetComponent<TerrainObjectHpPackData>();
                if (data == null) continue;

                CustomObject t = ModelMgr.Create<TerrainObjectHpPack>();

                t.LoadWithData(data);

                this.custom_objs.Add(t);
            }
        }




        {// -- init  transform
            Transform p = obj_terrain.transform.FindChild("Transform");
            if (p == null) return true;
            Transform[] objs = p.GetComponentsInChildren<Transform>();

            foreach (Transform obj in objs)
            {
                TerrainObjectTransformData data = obj.gameObject.GetComponent<TerrainObjectTransformData>();
                if (data == null) continue;

                CustomObject t = ModelMgr.Create<TerrainObjectTransform>();

                t.LoadWithData(data);

                this.custom_objs.Add(t);
            }
        }

        return true;
    }

    public override void OnEvent(int type, object userData)
    {
        if (Events.ID_BEFORE_ONEENTITY_UPDATEMS == type)
        {
            this.UpdateEntity(userData as Entity);
        }

    }
}


public sealed class LogicWorldMap : WorldMap
{
    /// <summary>
    /// 用这个地图 更新Entity世界信息
    /// </summary>
    /// <param name="what"></param>
    private void UpdateEntity(Entity what)
    {

    }



    public override bool Init()
    {
        base.Init();

        terrain = ModelMgr.Create<Terrain>();
        platform = ModelMgr.Create<TerrainPlatform>();
        return true;
    }

    public override void OnEvent(int type, object userData)
    {


    }
}
