using UnityEngine;
using System.Collections;


/// <summary>
/// 地形块
/// </summary>
public class TerrainBlock
{
    public float x_left;//地形块 左边界 单位米
    public float x_right;// 地形块右边界 单位米
    public float height; // 当前块 的海拔高度  单位米
    public int index = 0;// 当前地图块 在整个TerrainBase 地图中的 的index，从左往右
    public static TerrainBlock Create(float x_left, float x_right, float height, int index)
    {
        TerrainBlock ret = new TerrainBlock();
        ret.x_left = x_left;
        ret.x_right = x_right;
        ret.height = height;
        ret.index = index;
        return ret;
    }


    private TerrainBlock()
    { }
}


public class TerrainBase : Model
{
    /// <summary>
    /// 返回x坐标 对应的海拔高度
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public float GetHeight(float x)
    {

        return this.GetBlock(x).height;
    }

    /// <summary>
    /// 返回x坐标 对应的地形块
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public TerrainBlock GetBlock(float x)
    {
        foreach (TerrainBlock block in blocks)
        {
            if (block.x_left <= x && block.x_right >= x)
            {
                return block;
            }
        }
        return null;
    }

    /// <summary>
    /// 返回当前块  的下一块
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public TerrainBlock GetNextBlock(TerrainBlock block)
    {
        return this.GetBlockWithIndex(block.index + 1);
    }
    /// <summary>
    /// 返回当前块  的上一块
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public TerrainBlock GetLastBlock(TerrainBlock block)
    {
        return this.GetBlockWithIndex(block.index - 1);
    }


    /// <summary>
    /// 返回 index 指定的 地形块 0序开始
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TerrainBlock GetBlockWithIndex(int index)
    {
        if (index <= blocks.Count && index >= 0)
        {
            return blocks[index] as TerrainBlock;
        }
        return null;
    }


    protected ArrayList blocks = new ArrayList();
}


/// <summary>
///  可走范围的基本地形
/// </summary>
public class Terrain : TerrainBase
{
    //每个地图块都是无缝连接

    public override bool Init()
    {
        base.Init();
      
        GameObject obj_terrain = GameObject.Find("Terrain");
        Transform[] objs = obj_terrain.transform.FindChild("WalkAble").GetComponentsInChildren<Transform>(); 
        if (objs.Length < 2) return false;

        Transform last = objs[1] as Transform;

        for (int i = 2; i < objs.Length; i++)
        {
            Transform t = objs[i] as Transform;

            blocks.Add(TerrainBlock.Create(last.position.x, t.position.x, t.position.y, blocks.Count));
            last = t;
        }

        Debug.Log("Terrain Init " + objs.Length);

        GameObject map_range = obj_terrain.transform.FindChild("MapRange").gameObject;

        limit_x_left = map_range.transform.FindChild("Point_limit_x_left").transform.position.x;
        limit_x_right = map_range.transform.FindChild("Point_limit_x_right").transform.position.x;

        limit_y_down = map_range.transform.FindChild("Point_limit_y_down").transform.position.y;
        limit_y_up = map_range.transform.FindChild("Point_limit_y_up").transform.position.y;

        return true;
    }


    public float limit_x_left = 0.0f; //地图左边界
    public float limit_x_right = 0.0f;//地图右边界
    public float limit_y_up = 0.0f;// 地图上边界
    public float limit_y_down = 0.0f;//地图下边界



}




/// <summary>
/// 地图上的 平台 地形，
/// </summary>
public class TerrainPlatform : TerrainBase
{
    // 每个地图快 是单独成型的
    public override bool Init()
    {
        base.Init();

        GameObject obj_terrain = GameObject.Find("Terrain");
        Transform[] objs = obj_terrain.transform.FindChild("Platform").GetComponentsInChildren<Transform>();
        if (objs.Length < 2) return false;


        for (int i = 1; i < objs.Length; i += 2)
        {
            Transform p1 = objs[i] as Transform;
            Transform p2 = objs[i + 1] as Transform;

            blocks.Add(TerrainBlock.Create(p1.position.x, p2.position.x, p1.position.y, blocks.Count));

        }
        Debug.Log("TerrainPlatform Init  " + objs.Length);

        return true;
    }
}