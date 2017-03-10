/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class TerrainObjectHpPackData: MonoBehaviour
{

    [SerializeField]
    public float hp_percent; // 回血百分比


    [SerializeField]
    public float distance = 0.5f;//圆形判定距离

    [SerializeField]
    public float cd_time = 30.0f;//刷新时间
    void Start()
    {

    }


    void Update()
    {

    }
}
