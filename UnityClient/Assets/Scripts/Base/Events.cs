using UnityEngine;
using System.Collections;


public sealed class Events : object
{//数组实现hash  事件，提高效率
    public static int MAX_EVENT_LENGTH = 1000;//最大事件长度



    //------------------------------------  global event--------------------------------------



    public const int ID_BATTLE_START = 0;//战斗开始
    public const int ID_BATTLE_END = 1;// 战斗结束
    public const int ID_PLAYER_DEATH = 2;// 玩家死亡

    public const int ID_PUSH_MSG = 7;//显示推送消息

    public const int ID_UI_WAIT = 9;//等待消息
    public const int ID_UI_NOWAIT = 10;//停止等待


    public const int ID_ADD_ASYNC = 13;//添加loading动态执行 函数 ="addAsync"

    public const int ID_LOADING_NOW = 14;//加入游戏中
    public const int ID_LOADING_OK = 15;// 加入游戏完毕
    public const int ID_NET_DISCONNECT = 16;//网络断开连接
    public const int ID_EXIT = 17;//游戏退出




    public const int ID_START = 18;
    public const int ID_GAMEOVER = 19;
    public const int ID_NET_ERROR = 20;

    public const int ID_BTN_LEFT = 21;
    public const int ID_BTN_RIGHT = 22;
    public const int ID_BTN_JUMP = 23;
    public const int ID_BTN_ATTACK = 24;
    public const int ID_SPINE_COMPLETE = 25;
    public const int ID_STAND = 26;
    public const int ID_HURT = 28;
    public const int ID_LOADING_SHOW = 29;// 显示加载界面
    public const int ID_LOADING_HIDE = 30;// 隐藏加载界面，tip: 界面加载完成会自动关闭  可用此消息手动打开关闭界面显示
    public const int ID_LOADING_SYNC_STRING = 31;// 更新 loading界面的提示信息，默认显示加载中xxxxx



    public const int ID_BEFORE_ALLMODEL_UPDATEMS = 32;//在所有Model 帧同步运算前 (已设置来自服务器状态数据，但未处理)
    public const int ID_AFTER_ALLMODEL_UPDATEMS = 33;//在所有Model 帧同步运算后(已设置来自服务器状态数据，已处理)

    public const int ID_BEFORE_ONEENTITY_UPDATEMS = 34;//在Y一个Entity 帧同步运算前 (已设置来自服务器状态数据，但未处理)
    public const int ID_AFTER_ONEENTITY_UPDATEMS = 35;//在一个Entity 帧同步运算后(已设置来自服务器状态数据，已处理)




    public const int ID_LAUNCH_SKILL1 = 100;//释放技能1

    //------------------------------local event-----------------------------------------------------------

    /* public const int ID_DISABLE_RUN = 100;//禁止run 状态
     public const int ID_ENABLE_RUN = 101;// 启用run 状态

     public const int ID_DISABLE_JUMP = 102;// jump state
     public const int ID_ENABLE_JUMP = 103;// jump state

     public const int ID_DISABLE_ATTACK = 104;//attack state
     public const int ID_ENABLE_ATTACK = 105;// attack
     */




    //----------------------------------------user event -------------------------------------------------------


}
