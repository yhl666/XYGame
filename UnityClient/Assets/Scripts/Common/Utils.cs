/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System.IO;

public sealed class Utils
{
    public static void Log(string s)
    {
        /*  StreamWriter file = new StreamWriter("log.txt", true);
             file.WriteLine(s);
             file.Close();
             file.Dispose();

             */
    }

    public static float ClaculateDistance(Vector2 v1, Vector2 v2)
    {
        float dx = v1.x - v2.x;
        float dy = v1.y - v2.y;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }
    public static float ClaculateDistance(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(v1, v2);
    }
    public static float ClaculateDistance(Vector2 v1, Vector3 v2)
    {
        return Vector3.Distance(new Vector3(v1.x, v1.y, v2.z), v2);
    }
    public static float ClaculateDistance(Vector3 v1, Vector2 v2)
    {
        return Vector3.Distance(v1, new Vector3(v2.x, v2.y, v1.z));
    }

    /// <summary>
    /// y和 z 的混合比例 默认为 1：1 45度视角
    /// </summary>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static float ConvertYZToView25DY(float y, float z)
    {
        return (y + z);
    }


    public static int RandomSeed = 0;

    public static string SkipWhite(string str)
    {
        string ret = "";
        return ret;
    }

    public static float GetAngle(Vector3 from, Vector3 to)
    {
        return GetDegree(new Vector2(from.x, from.z), new Vector2(to.x, to.z));
    }

    /// <summary>
    ///   0-360
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static float GetDegree(Vector2 from, Vector2 to)
    {
        const float ONE_DEGREE = DATA.ONE_DEGREE;//一度的弧度

        float dx = to.x - (from.x);
        float dy = to.y - (from.y);
        float dposx = 0.0f, dposy = 0.0f;


        float degree = Mathf.Atan(dy / dx) * 57.29578f;

        if (float.IsNaN(degree)) return 0.0f;
        float degree_total = 0.0f;

        if (dx == 0.0f || dy == 0.0f)
        {
            return 0.0f;
        }


        if (dx >= 0.0f && dy >= 0.0f)// 0 1 2 //1
        {
            dposy = 1 * Mathf.Sin(degree * ONE_DEGREE);
            dposx = 1 * Mathf.Cos(degree * ONE_DEGREE);
            degree_total = degree;
        }
        else if (dx >= 0.0f && dy <= 0.0f)// 0 7 6 //4
        {
            degree = -degree;
            dposy = -1 * Mathf.Sin(degree * ONE_DEGREE);
            dposx = 1 * Mathf.Cos(degree * ONE_DEGREE);

            degree_total = 360.0f - degree;
        }
        else if (dx <= 0.0f && dy >= 0.0f)// 2 3 4 //2
        {
            degree = -degree;
            dposy = 1 * Mathf.Sin(degree * ONE_DEGREE);
            dposx = -1 * Mathf.Cos(degree * ONE_DEGREE);
            degree_total = 180.0f - degree;
        }
        else//4 5 6 // 3
        {
            dposy = -1 * Mathf.Sin(degree * ONE_DEGREE);
            dposx = -1 * Mathf.Cos(degree * ONE_DEGREE);
            // degree_total = 180 + degree;
            degree_total = 180.0f + degree;
        }


        return degree_total;

    }
    public static Vector3 Vector3To2DVector3(Vector3 v, Vector3 vv)
    {
        return new Vector3(v.x, v.y, vv.z);
    }


    public static string GetFileName(string file)
    {
        if (file.IndexOf("/") == -1) return file;
        int idx = file.LastIndexOf("/");
        string ret = file.Substring(idx + 1, file.LastIndexOf(".") - 1 - idx);

        return ret;
    }
    public static string GetFilePath(string file)
    {
        string ret = "";
        if (file.IndexOf("/") != -1)
        {
            ret = file.Substring(0, file.LastIndexOf("/") + 1); // include "/"
        }
        return ret;
    }

    public static float deltaTime
    {
        get
        {
            //  return Time.deltaTime;
            return 1.0f / fps;
        }
    }

    public static float fps
    {
        get
        {
            return 40.0f;
        }
    }
    public static int fpsi
    {
        get
        {
            return 40;
        }
    }
    public static void SetTargetFPS(int fps)
    {
        Application.targetFrameRate = fps;
    }

    /// <summary>
    /// unity 世界坐标（单位米） 转换为2D像素坐标
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public float ConvertToPixelPosition(float p)
    {
        return p * 100.0f;
    }
    /// <summary>
    ///  2D像素坐标 转换为unity 的世界坐标(单位：米)
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public float ConvertToUnityPosition(float p)
    {
        return p / 100.0f;
    }


    public Vector2 ConvertToPixelPosition(Vector2 p)
    {
        return p * 100.0f;
    }
    public Vector2 ConvertToUnityPosition(Vector2 p)
    {
        return p / 100.0f;
    }
    public Vector3 ConvertToPixelPosition(Vector3 p)
    {
        return new Vector3(p.x * 100.0f, p.y * 100.0f, p.z);
    }
    public Vector3 ConvertToUnityPosition(Vector3 p)
    {
        return new Vector3(p.x / 100.0f, p.y / 100.0f, p.z);
    }

    public static object Create(string _class_name)
    {
        System.Type t = System.Type.GetType(_class_name);
        if (t == null)
        {
            Debug.LogError("UnKnown type:" + _class_name);
            return null;
        }
        return (System.Activator.CreateInstance(t));
    }
}
