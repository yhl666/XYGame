using UnityEngine;
using System.Collections;


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
        Debug.Log("Terrain Init");

        Transform[] objs = GameObject.Find("Terrain").transform.FindChild("WalkAble").GetComponentsInChildren<Transform>();
        if (objs.Length < 2) return false;

        Transform last = objs[1] as Transform;

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

