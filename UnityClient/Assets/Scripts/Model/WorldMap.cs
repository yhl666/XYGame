/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

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
        if (false == this.UpdateWithPlaform(what))
        {
            this.UpdateWithTerrain(what);
        }
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

        //  return false;

        float x = what.x;
        float y = 0.0f;

        TerrainBlock block = platform.GetBlock(x);


        if (block != null)
        {
            y = block.height;

            if (y == what.height)
            {
                return true;

            }

            //      Debug.Log(what.height + what.y  + "       "  +  y );
            if (what.height + what.y >= y)
            {
                what.height_platform = y;
                return true;
            }
        }
        else
        {

        }
        return false;

    }
    public override bool Init()
    {
        base.Init();

        terrain = ModelMgr.Create<Terrain>();
        platform = ModelMgr.Create<TerrainPlatform>();

        EventDispatcher.ins.AddEventListener(this, Events.ID_BEFORE_ONEENTITY_UPDATEMS);
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
