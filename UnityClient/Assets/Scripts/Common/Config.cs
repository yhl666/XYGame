﻿/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

/// <summary>
/// 摄像机视角 类型，游戏本身 3D系统构建
/// 只是摄像机视角类型区别
/// </summary>
 public enum ViewMode
 {
     M2D, // 2D视角 2D游戏XY轴
    M25D, // 2.5D  类似于 DNF的视角 Y轴是高度
     M3D, // 3D 模式
     M3D_LOCK,// 3D 锁定视角度 模式  默认45度 类似于LOL
 }

//debug 
public class Config : object
{
   public static string SERVER_IP = "127.0.0.1";
 //  public static string SERVER_IP = "114.215.96.20";

    public const int SERVER_PORT = 8111;

    public const int MAX_NETSOCKET_BUFFER_SIZE = 500;
 

    public const int MAX_FRAME_COUNT = 60000;
 

    public const int MAX_FPS = 40;

    public static ViewMode  VIEW_MODE= ViewMode.M25D;
  
};



