/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;



public class DATA
{
    public static void Init()
    {
        var path = DATA.dataPath;
    }
    public static string dataPath = Application.dataPath;


    public const string UI_PREFABS_FILE_BATTLE = "Prefabs/ui/Battle";
    public const string UI_PREFABS_FILE_DIRINPUT = "Prefabs/ui/DirInput";
    public const string UI_PREFABS_FILE_PUBLIC = "Prefabs/ui/Public";
    public const string UI_PREFABS_FILE_LOGIN = "Prefabs/ui/Login";

    public const string UI_LOADING = "加载中 {0}%  {1}";

    public const string UI_DEATH_ALIVE_TIME = "复活剩余时间:{0} 秒";
    public const string UI_PLAYING_VIDEOMODE = "播放录像中";
    public const string UI_WAIT_INFO_DEFAULT = "等待中";
    public const string UI_WAIT_INFO_OTHERS = "已进入房间，请等待";
    public const string UI_WAIT_ENTER = "正在加入游戏，请稍后";
    public const string UI_RECONNECTING = "重新连接服务器中";

    public const string UI_INFO_XY = "X:{0} Y:{1}";
    public const string UI_FPSMS = "FPS:{0} {1}ms";
    public const string UI_FRAME = "Frame:{0}";

    public const string EMPTY_STRING = "";


    public const float ONE_DEGREE = 0.0174533f;//一度的弧度

    public const int HERO_ALIVE_TICK = 4800;//2分钟


    public const float DEFAULT_JUMP_SPEED = 0.2f;//默认起跳初速度;
    public const float GRAVITY_RATIO = 0.05f;//默认重力比例


}

