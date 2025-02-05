﻿/*
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

#if UNITY_IOS || UNITY_ANDROID

    // public static string SERVER_IP = "192.168.1.200";
   // public static string LOGIC_SERVER_IP = "192.168.1.200";

      public static string SERVER_IP = "115.159.203.16";
    //  public static string SERVER_IP = "192.168.93.39";

     public static string LOGIC_SERVER_IP = "115.159.203.16";
    //  public static string LOGIC_SERVER_IP = "192.168.93.39";

#else

     public static string SERVER_IP = "127.0.0.1";
 // public static string SERVER_IP = "115.159.203.16";
    //  public static string SERVER_IP = "192.168.93.39";

    public static string LOGIC_SERVER_IP = "127.0.0.1";
   //  public static string LOGIC_SERVER_IP = "115.159.203.16";
    //  public static string LOGIC_SERVER_IP = "192.168.93.39";

#endif


    public const int LOGIC_SERVER_PORT = 8002;
    public const int SERVER_PORT = 8111;


    public static float fps_delay = 0.025f;
    public const int MAX_NETSOCKET_BUFFER_SIZE = 500;

    public const int MAX_FRAME_COUNT = 60000;

    public const int MAX_FPS = 40;

    public static ViewMode VIEW_MODE = ViewMode.M25D;

    public static float DESIGNED_XF = 1136f; //设计屏幕分辨率
    public static float DESIGNED_YF = 640f;

    public static float SCREEN_SCALE_X = Screen.width / 1136f; // 实际分辨率缩放比例
    public static float SCREEN_SCALE_Y = Screen.height / 640f;

    //-------网络优化设置
    public const bool NETWORK_SYNC_OPTIMA_ENABLE=true;//开启网络流量优化，减少网络同步次数
    public const int NETWORK_SYNC_OPTIMA_FPS = 20;//网络同步频率  逻辑帧数是固定40帧
   
    //------日志UI 调试信息 设置
    public static bool DEBUG_LoadDebugWindow = true;//是否加载 控制台代码和资源
    public static bool DEBUG_EnableDebugWindow = false;//开启界面日志输出
    public static bool DEBUG_EnableAutoClean = true;//开启自动清理 ，主要用于Console性能优化
    public static int DEBUG_MaxCount = 100; //开启自动清理后，最大日志保留条数
    public static bool DEBUG_EnableAutoShowStackTrace = true;//自动显示 调用栈; 当Error 或者Exception时候
};
