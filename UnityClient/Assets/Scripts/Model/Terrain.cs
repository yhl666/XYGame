/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


/// <summary>
/// 地形块
/// </summary>
public class TerrainBlock : IComparer
{
    public string name = "";
    public float x_left;//地形块 左边界 单位米
    public float x_right;// 地形块右边界 单位米
    public float height; // 当前块 的海拔高度  单位米
    public int index = 0;// 当前地图块 在整个TerrainBase 地图中的 的index，从左往右
    public static TerrainBlock Create(float x_left, float x_right, float height, int index,string name)
    {
        TerrainBlock ret = new TerrainBlock();
        ret.x_left = x_left;
        ret.x_right = x_right;
        ret.height = height;
        ret.index = index;
        ret.name = name;
        return ret;
    }

    // 摘要: 
    //     比较两个对象并返回一个值，指示一个对象是小于、等于还是大于另一个对象。
    //
    // 参数: 
    //   x:
    //     要比较的第一个对象。
    //
    //   y:
    //     要比较的第二个对象。
    //
    // 返回结果: 
    //     值 条件 小于零 x 小于 y。 零 x 等于 y。 大于零 x 大于 y。
    //
    // 异常: 
    //   System.ArgumentException:
    //     x 和 y 都不实现 System.IComparable 接口。- 或 - x 和 y 的类型不同，它们都无法处理与另一个进行的比较。
    public int Compare(object x, object y)
    {
        TerrainBlock a = (TerrainBlock)x;
        TerrainBlock b = (TerrainBlock)y;
        if (a.x_left == b.x_left) return 0;
        if (a.x_left > b.x_left) return 1;
        if (a.x_left < b.x_left) return -1;

        return 0;

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

    public ArrayList GetBlocksWithName(string name)
    {
        ArrayList ret = new ArrayList();
        foreach(TerrainBlock block in blocks)
        {
            if(block.name == name)
            {
                ret.Add(block);
            }
        }
        return ret;
    }
    protected ArrayList blocks = new ArrayList();

    protected ArrayList Load(Transform[] objs ,bool auto_sort=true)
    {
        ArrayList blocks = new ArrayList();
        //按照左往右顺序读取配置点
        //因为Editor里面标记点顺序 和 点X坐标的顺序不一定一致 所以要以下处理

        //先取出x点 和高度
        for (int i = 1; i < objs.Length; i++)
        {
            Transform t = objs[i] as Transform;
            blocks.Add(TerrainBlock.Create(t.position.x, 0, t.position.y, 0,t.gameObject.name));
        }
        //排序   按照X大小 来排序
        if (auto_sort)
        {
            blocks.Sort(blocks[0] as IComparer);
        }
        //返回信息是 点 和海拔 并不是真正的TerrainBlock
        return blocks;
    }
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
        if (objs.Length < 3) return false;

        ArrayList blocks = this.Load(objs);

        TerrainBlock last = blocks[0] as TerrainBlock;

        for (int i = 1; i < blocks.Count; i++)
        {
            TerrainBlock t = blocks[i] as TerrainBlock;
            //2个点确定一个地形块，海拔由第一个点的 海拔决定
            this.blocks.Add(TerrainBlock.Create(last.x_left, t.x_left, t.height, this.blocks.Count,t.name));
            last = t;
        }

        Debug.Log("Terrain Init Point Count=" + (objs.Length - 1));
        //自动化 初始
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
        if (objs.Length < 3) return false;
        if (objs.Length % 2 == 0)
        {//必须是奇数
            Debug.LogError("Terrain Platform Count error");
            return false;
        }
        ArrayList blocks = this.Load(objs);
        //初始化地形 自动化
        for (int i = 0; i < blocks.Count; i += 2)
        {
            TerrainBlock p1 = blocks[i] as TerrainBlock;
            TerrainBlock p2 = blocks[i + 1] as TerrainBlock;

            this.blocks.Add(TerrainBlock.Create(p1.x_left, p2.x_left, p1.height, this.blocks.Count,p1.name));

        }

        Debug.Log("TerrainPlatform Init  Point Count=" + (objs.Length - 1));

        return true;
    }
}











/// <summary>
/// 地图上的 平台 地形，
/// </summary>
public class TerrainPlatformPVP : TerrainPlatform
{
    // 每个地图快 是单独成型的
    public override bool Init()
    {
        base.Init();

        GameObject obj_terrain = GameObject.Find("Terrain");
        Transform[] objs = obj_terrain.transform.FindChild("Platform").GetComponentsInChildren<Transform>();
        if (objs.Length < 3) return false;
        if (objs.Length % 2 == 0)
        {//必须是奇数
            Debug.LogError("Terrain Platform Count error");
            return false;
        }
        ArrayList blocks = this.Load(objs);
        //初始化地形 自动化
        for (int i = 0; i < blocks.Count; i += 2)
        {
            TerrainBlock p1 = blocks[i] as TerrainBlock;
            TerrainBlock p2 = blocks[i + 1] as TerrainBlock;

            this.blocks.Add(TerrainBlock.Create(p1.x_left, p2.x_left, p1.height, this.blocks.Count, p1.name));

        }

        Debug.Log("TerrainPlatform Init  Point Count=" + (objs.Length - 1));

        return true;
    }
}

