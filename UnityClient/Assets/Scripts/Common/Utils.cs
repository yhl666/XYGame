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


    public static int RandomSeed = 0;

    public static string SkipWhite(string str)
    {

        string ret = "";


        return ret;


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
            return  40.0f;
        }
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
        return p *100.0f;
    }
    public Vector2 ConvertToUnityPosition(Vector2 p)
    {
        return p / 100.0f;
    }
    public Vector3 ConvertToPixelPosition(Vector3 p)
    {
        return new Vector3(p.x * 100.0f, p.y * 100.0f, p.z );
    }
    public Vector3 ConvertToUnityPosition(Vector3 p)
    {
        return new Vector3(p.x / 100.0f, p.y / 100.0f, p.z);
    }
}
