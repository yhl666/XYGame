/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

/// <summary>
/// 帧同步  自定义 帧操作
/// 比如 购买物品 升级技能等 需要同步的 自定义操作
/// </summary>
public enum FrameCustomsOpt
{
    UnKnown = 0,
    Test = 1,
}

public sealed class FrameData : object
{
    public int fps = 0;//帧数信息
    public int no = 0;//该房间的玩家编号

    // 按钮状态，可用int的位 来表示，暂时不优化

    public int left = 0;//left
    public int right = 0;//right
    public int jump = 0;//jump
    public int atk = 0;//atk
    public int s1 = 0;//skill 1
    public int stand = 0;//stand
    public int revive = 0;//revive point
    public int dir = -1;
    public int opt = 0;
    private FrameData()
    {

    }

    public string toJson()
    {
        string s = "";

        s += "no:" + no.ToString() + ",";
        s += "fps:" + fps.ToString() + ",";
        s += "dir:" + dir.ToString() + ",";

        s += "left:" + left.ToString() + ",";
        s += "right:" + right.ToString() + ",";
        s += "jump:" + jump.ToString() + ",";
        s += "atk:" + atk.ToString() + ",";
        s += "s1:" + s1.ToString() + ",";
        s += "stand:" + stand.ToString() + ",";
        s += "revive:" + revive.ToString() + ",";
        s += "opt:" + opt.ToString() + ",";

        return s;

    }

    public string toUploadJson(bool skip = true)
    {
        string s = "";
        //转换为 上传服务器的json数据，不包含fps等非关键信息，节省流量
        if (skip)
        {
            s += "dir:" + dir.ToString() + ",";
            if (left != 0) s += "left:" + left.ToString() + ",";
            if (right != 0) s += "right:" + right.ToString() + ",";
            if (jump != 0) s += "jump:" + jump.ToString() + ",";
            if (atk != 0) s += "atk:" + atk.ToString() + ",";
            if (s1 != 0) s += "s1:" + s1.ToString() + ",";
            if (stand != 0) s += "stand:" + stand.ToString() + ",";
            if (revive != 0) s += "revive:" + revive.ToString() + ",";
            if (opt > 0) s += "opt:" + opt.ToString() + ",";

        }
        else
        {
            s += "dir:" + dir.ToString() + ",";
            s += "no:" + no.ToString() + ",";
            s += "left:" + left.ToString() + ",";
            s += "right:" + right.ToString() + ",";
            s += "jump:" + jump.ToString() + ",";
            s += "atk:" + atk.ToString() + ",";
            s += "s1:" + s1.ToString() + ",";
            s += "stand:" + stand.ToString() + ",";
            s += "revive:" + revive.ToString() + ",";
            s += "opt:" + opt.ToString() + ",";
        }
        return s;

    }
    public static FrameData CreateWithJson(string json)
    {
        FrameData ret = new FrameData();
        if (ret != null && ret.setJson(json))
        {
            return ret;
        }
        return null;
    }

    public static FrameData Create()
    {
        FrameData ret = new FrameData();
        if (ret != null)
        {
            return ret;
        }
        return null;
    }
    public static ArrayList CreateWithMultiJson(string json)
    {

        ArrayList ret = new ArrayList();
        int last = 0;
        for (int i = 0; i < json.Length; i++)
        {
            if (json[i].Equals('+'))
            {
                string json1 = json.Substring(last, i - last);
                last = i + 1;

                FrameData d = new FrameData();
                if (d != null && d.setJson(json1))
                {
                    ret.Add(d);
                }
            }
        }

        return ret;
    }


    public bool setJson(string json)
    {
        int last = 0;


        string k = "0", v = "0";
        for (int i = 0; i < json.Length; i++)
        //  foreach( char ch in json)
        {
            char ch = json[i];
            if (ch.Equals(':'))
            {
                k = json.Substring(last, i - last);
                last = i + 1;
            }

            if (ch.Equals(','))
            {
                v = json.Substring(last, i - last);
                last = i + 1;

                if (k == "left")
                {
                    left = int.Parse(v);
                }

                if (k == "right")
                {
                    right = int.Parse(v);
                }

                if (k == "fps")
                {
                    fps = int.Parse(v);
                }

                if (k == "no")
                {
                    no = int.Parse(v);
                }
                if (k == "jump")
                {
                    jump = int.Parse(v);
                }
                if (k == "atk")
                {
                    atk = int.Parse(v);
                }
                if (k == "s1")
                {
                    s1 = int.Parse(v);
                }
                if (k == "stand")
                {
                    stand = int.Parse(v);
                }
                if (k == "revive")
                {
                    revive = int.Parse(v);
                }
                if (k == "dir")
                {
                    dir = int.Parse(v);
                }
                if (k == "opt")
                {
                    opt = int.Parse(v);
                }
            }

        }
        return true;

    }
}
