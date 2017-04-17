/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

/// <summary>
/// 摄像机视角 类型，游戏本身 3D系统构建
/// 只是摄像机视角类型区别
/// 2.5D模式下  高度Y轴表示，
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
 //   public static string SERVER_IP = "115.159.203.16";

    public const int SERVER_PORT = 8111;

    public const int MAX_NETSOCKET_BUFFER_SIZE = 500;

    public const int MAX_FRAME_COUNT = 60000;

    public const int MAX_FPS = 40;

    public static ViewMode VIEW_MODE = ViewMode.M25D;

    public static float DESIGNED_XF = 1136f; //设计屏幕分辨率
    public static float DESIGNED_YF = 640f;

    public static float SCREEN_SCALE_X = Screen.width / 1136f; // 实际分辨率缩放比例
    public static float SCREEN_SCALE_Y = Screen.height / 640f;

};
