﻿/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class TerrainObjectTransformData : MonoBehaviour
{
    /// <summary>
    /// 可强制传送到目标点
    /// </summary>
    [SerializeField]
    public GameObject next_point = null; // 目标传送点

    [SerializeField]
    public int cd; //  传送点cd ，单位：帧
    void Start()
    {

    }


    void Update()
    {

    }
}
