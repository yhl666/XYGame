/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public enum EnemyProtoType
{
    小怪1 = 0,//小怪1
    小怪1加强版, // 小怪1 加强版
    小怪2, // 小怪2
    小怪2加强版,// 小怪2 加强版
    小怪3, // 小怪3
}

public class EnemyPrototype : MonoBehaviour
{
    public string code = "202001";// 原型标示符
    public EnemyProtoType type = EnemyProtoType.小怪1;
    //   public int level = 1;// 当前等级
    public int damage = 200;// 基本攻击力
    public int exp = 100;//死亡掉落的经验值
    public int hp = 100;//血量



    /// <summary>
    /// 返回当前类型对应的 class name
    /// </summary>
    /// <returns></returns>
    public string GetClassType()
    {
        string[] class_types = { "Enemy1", "Enemy1_Strengthen", "Enemy2", "Enemy2_Strengthen", "Enemy3" };

        return class_types[(int)type];

        if (type == EnemyProtoType.小怪1)
        {
            return "Enemy1";
        }
        else if (type == EnemyProtoType.小怪1加强版)
        {
            return "Enemy1_Strengthen";
        }
        else if (type == EnemyProtoType.小怪2)
        {
            return "Enemy2";
        }
        else if (type == EnemyProtoType.小怪2加强版)
        {
            return "Enemy2_Strengthen";
        }
        else if (type == EnemyProtoType.小怪3)
        {
            return "Enemy3";
        }
        else
        {
            return "";
        }
    }
    /// <summary>
    /// 获取原型
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static EnemyPrototype GetPrototype(string code)
    {
        foreach (EnemyPrototype type in prototypes)
        {
            if (type.code == code)
            {
                return type;
            }
        }
        return null;
    }
    public void OnDestroy()
    {
        prototypes.Remove(this);
    }
    public void Awake()
    {
        prototypes.Add(this);
    }
    public static ArrayList prototypes = new ArrayList();
}
