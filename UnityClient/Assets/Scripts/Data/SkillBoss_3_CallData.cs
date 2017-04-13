/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class SkillBoss_3_CallData : MonoBehaviour
{
    //common
    public int call_level;//第几次召唤的
    public int id; // 出生点id
    public string code; //怪物原型代码
    public float time;//出现时间 该时间从Boss开始放召唤技能开始 单位秒
}
