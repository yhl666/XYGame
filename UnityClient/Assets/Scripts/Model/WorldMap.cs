using UnityEngine;
using System.Collections;

public class WorldMapBase : Model
{

    public Terrain GetTerrain()
    {
        return terrain;
    }
    public void AddEntity(Entity obj)
    {
        objs.Add(obj);
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


public sealed class BattleWorldMap : WorldMapBase
{
    /// <summary>
    /// 用这个地图 更新Entity世界信息
    /// </summary>
    /// <param name="what"></param>
    public void UpdateEntity(Entity what)
    {
     //   
      if(false==  this.UpdateWithPlaform(what))
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

            Debug.Log(what.height + what.y  + "       "  +  y );
               if (what.height + what.y >= y)
            {
                what.height_platform = y;
                return true;
            }
        }
        else
        {
            Debug.Log(0);
        }
        return false;

    }
    public override bool Init()
    {
        base.Init();

        terrain = ModelMgr.Create<Terrain>();
        platform = ModelMgr.Create<TerrainPlatform>();
        return true;
    }
}
