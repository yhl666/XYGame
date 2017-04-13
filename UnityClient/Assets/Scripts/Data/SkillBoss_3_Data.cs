/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
[System.Serializable]

[AddComponentMenu("DATA/SkillBoss3")]
[ExecuteInEditMode]
public class SkillBoss_3_Data : MonoBehaviour
{
    //common
    public int cd; // cd ，单位 帧数
    public string animation_name;// 技能角色动画名称
    public int last_time;//施法持续时间 单位帧
    public int[] call_times;//召唤波数 数组内容为每波触发的血量 百分比
    //   public SkillBoss3_Call_Data[] datas;


    /// <summary>
    /// 根据波数 获得数据 SkillBoss3_Call_Data
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ArrayList Get(int index)
    {
        if (index > call_times.Length) return null;
        SkillBoss_3_CallData[] datas = gameObject.GetComponents<SkillBoss_3_CallData>();
        ArrayList ret = new ArrayList();

        foreach (SkillBoss_3_CallData data in datas)
        {
            if (data.call_level == index)
            {
                ret.Add(data);
            }
        }
        return ret;
    }
    public static SkillBoss_3_Data ins = null;
    void Awake()
    {
        ins = this;
    }
}
