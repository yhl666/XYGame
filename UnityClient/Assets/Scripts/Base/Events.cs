/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


public sealed class Events : object
{//数组实现hash  事件，提高效率
    public static int MAX_EVENT_LENGTH = 1000;//最大事件长度



    //------------------------------------  global event--------------------------------------



    public const int ID_BATTLE_START = 0;//战斗开始
    public const int ID_BATTLE_END = 1;// 战斗结束
    public const int ID_PLAYER_DEATH = 2;// 玩家死亡

    public const int ID_PUBLIC_PUSH_MSG = 7;//显示 全局 推送消息,比如 提示玩家等级不够 等

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



    public const int ID_BEFORE_ALLMODEL_UPDATEMS = 32;//在所有Model 帧同步运算前 在ModelMgr 里面触发
    public const int ID_AFTER_ALLMODEL_UPDATEMS = 33;//在所有Model 帧同步运算后

    public const int ID_BEFORE_ONEENTITY_UPDATEMS = 34;//在一个Entity 帧同步运算前 (已设置来自服务器状态数据，但未处理)在HeroMgr 和 EnemyMgr 里面触发
    public const int ID_AFTER_ONEENTITY_UPDATEMS = 35;//在一个Entity 帧同步运算后(已设置来自服务器状态数据，已处理)

    public const int ID_BEFORE_ONEBULLET_UPDATEMS = 36;  //在一个子弹 帧同步更新前
    public const int ID_AFTER_ONEBULLET_UPDATEMS = 37; //  // 在一个子弹帧同步更新后 BulletMgr触发

    public const int ID_BEFORE_ALLBULLET_UPDATEMS = 38;  //在所有子弹 帧同步更新前
    public const int ID_AFTER_ALLBULLET_UPDATEMS = 39; //  // 在所有子弹帧同步更新后 BulletMgr触发


    public const int ID_BEFORE_ONEBUFFER_UPDATEMS = 40;  //在一个Buffer 帧同步更新前
    public const int ID_AFTER_ONEBUFFER_UPDATEMS = 41; //  // 在一个buffer 帧同步更新后 BufferMgr触发

    public const int ID_BEFORE_ALLVIEW_UPDATEMS = 42;//在所有View 帧同步运算前 在ViewMgr 里面触发
    public const int ID_AFTER_ALLVIEW_UPDATEMS = 43;//在所有View 帧同步运算后

    public const int ID_BEFORE_ALLVIEW_UPDATE = 44;//在所有View Unity 更新后 在ViewMgr 里面触发
    public const int ID_AFTER_ALLVIEW_UPDATE = 45;//在所有View Unity 更新后

    public const int ID_LOGIC_NEW_POSITION = 46;//主场景 新目标点事件



    public const int ID_BTN_LOGIN = 47;
    public const int ID_BTN_REGISTER = 48;
    public const int ID_LOGIN_STATUS = 49;

    public const int ID_LAUNCH_SKILL1 = 100;//释放技能1


    public const int ID_RPC_WORLD_CHAT_NEW_MSG = 50;//服务器推送 世界聊天新消息通知
    public const int ID_WORLDCHAT_CELL_BTN_CLICKED = 51;//聊天小窗口按钮被点击
    public const int ID_WORLDCHAT_SEND_BTN_CLICKED = 52;//世界聊天窗口 点击发送按钮
    public const int ID_WORLDCHAT_CLOSE_BTN_CLICKED = 53;//世界聊天窗口 点击关闭按钮

    public const int ID_TOWN_MENU_CLICKED = 54;
    public const int ID_TOWN_MENU_CLOSE_CLICKED = 55;

    public const int ID_VIEW_NEW_CELLAPP_VIEW = 56; //新CELL APP的view 事件 提供给Root View添加


    public const int ID_FRIENDS_CLOSE_CLICKED = 57; // 关闭点击
    public const int ID_FRIENDS_ADD_CLICKED = 58;// 加好友点击
    public const int ID_FRIENDS_SHOW_CLICKED = 60;// 加好友点击


    public const int ID_TOWN_FRIENDS_CLICK = 59;//菜单 的 好友被点击


    public const int ID_RPC_NEW_FRIEND = 61;//有玩家添加你为好友
    public const int ID_ADD_FRIEND_SUCCESS = 63;//添加好友成功

    public const int ID_FRIEND_SYNC_VIEW = 62;//刷新UI

    public const int ID_FRIENDS_DELETE_CLICKED = 64;//  
    public const int ID_FRIENDS_PVP_CLICKED = 65;

    public const int ID_PUBLIC_GLOBALDIALOG_SHOW = 66;//全局对话框显示

    public const int ID_BATTLE_PVP_WAITFOR_RESULT_SHOW = 67;//等待战斗结果
    public const int ID_BATTLE_PVP_RETULT = 68;// 来自服务端的战斗结果通知
    public const int ID_REQUEST_EQUIP_GET_XXXXX = 200;

    public const int ID_TOWN_BTN_PVP_RAMDON_QUEUE_CLICKED = 69;//进入随机匹配1v1队列按钮
    public const int ID_TOWN_BTN_PVP_LEAVE_QUEUE_CLICKED = 70;//退出随机匹配1v1队列按钮
    public const int ID_TOWN_BTN_PVP_ENTER_QUEUE_CLICKED = 71;//进入随机匹配1v1队列按钮


    public const int ID_TOWN_PVP_QUEUE_RESULT = 72;//  匹配结果消息


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
