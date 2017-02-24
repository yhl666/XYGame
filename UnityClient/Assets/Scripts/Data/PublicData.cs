/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using GameBox.Service.GiantLightFramework;
using GameBox.Framework;
using GameBox.Service.ByteStorage;

public class PublicData : object
{
    public static PublicData ins
    {
        get
        {
            return GetInstance();
        }
    }

    private static PublicData _ins = null;

    public static PublicData GetInstance()
    {
        if (_ins == null)
        {
            _ins = new PublicData();
        }
        return _ins;
    }
    public static void DestroyInstance()
    {
        _ins = null;
    }

    public string _game_over_info_string;

    public ArrayList _recv_last_game;

    public bool isVideoMode = false;

    public int ms = 0;

    public bool isSelfAlive = true;
    public int _on_enter_max_fps = 0;

    public string player_name = "Default";

    public DAO.User self_user = null;

    // for frame sync  input status
    public bool IS_left = false;//left
    public bool IS_right = false;//right
    public bool IS_jump = false;//jump
    public bool IS_atk = false;//atk
    public bool IS_s1 = false;//skill 1
    public bool IS_stand = false;//stand


    public IGiantGame game = null;


    public string client_server_room_info = "";
    public bool is_client_server = false;
    public string client_server_result = "";



    // -------------------------------------------------------------pvp
    public DAO.User user_pvp_other = null;
    public string pvp_room_no = "1024";//房间id
    public string pvp_room_max = "1";//房间最大数

    //是否是PVP 模式
    public bool is_pvp_friend = false;
    public bool is_pvp_friend_ai = false;
    public bool is_pvp_friend_owner = true;//主动发起的一方

 
    public void ResetPVP()
    {
        user_pvp_other = null;
        pvp_room_no = "1024";//房间id
        pvp_room_max = "1";//房间最大数

        //是否是PVP 模式
        is_pvp_friend = false;
        is_pvp_friend_ai = false;
        is_pvp_friend_owner = true;//主动发起的一方

  
    }
}
