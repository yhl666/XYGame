using UnityEngine;
using System.Collections;

public class Model : GAObject
{
    public override void Update()
    {

    }
    public override void UpdateMS()
    {

    }
    public override bool Init()
    {

        return true;
    }

    public bool visible = true;
    ///  public View view = null;

}



public class MapBase : Model
{
    public void AddEntity(Entity obj)
    {

    }

    /// <summary>
    ///  this will 
    /// </summary>
    public override void UpdateMS()
    {
        this.UpdateX();
        this.UpdateY();

    }
    private void UpdateX()
    {

    }
    private void UpdateY()
    {

    }
    /// <summary>
    /// 返回当前x坐标的高度
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>

    public float GetHeight(float x)
    {
        int xx = (int)x;
        return 0.0f;
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



    protected ArrayList heights = new ArrayList();
    protected ArrayList objs = new ArrayList();
}



[System.Serializable]
public class TerrainBlock
{
    public float x_left;
    public float x_right;
    public float height;

    public static TerrainBlock Create(float x_left, float x_right, float height)
    {
        TerrainBlock ret = new TerrainBlock();
        ret.x_left = x_left;
        ret.x_right = x_right;
        ret.height = height;

        return ret;
    }
    private TerrainBlock()
    { }
}
public class Terrain : Model
{
    /// <summary>
    /// 根据x坐标 返回海拔高度
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public float GetHeight(float x)
    {

        return 0.0f;
    }

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
    public override bool Init()
    {
        base.Init();
        // 1米为单位
        Debug.Log("Terrain Init");

        Transform[] objs = GameObject.Find("Terrain").transform.FindChild("Points").GetComponentsInChildren<Transform>();

        Transform last = objs[1] as Transform;
        ;

        for (int i = 2; i < objs.Length; i++)
        {
            Transform t = objs[i] as Transform;

            blocks.Add(TerrainBlock.Create(last.position.x, t.position.x, t.position.y));
            last = t;
        }


        //   blocks.Add(TerrainBlock.Create(0f, 2.2f, 0f));
        // blocks.Add(TerrainBlock.Create(2.2f, 4.7f, 0.64f));
        //  blocks.Add(TerrainBlock.Create(4.7f, 7.335f, 1.255f));



        return true;
    }

    [SerializeField]
    public ArrayList blocks = new ArrayList();
}

