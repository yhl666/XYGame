/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.IO;
public class Files
{
    /* enum Mode
     {
         LINE, // 按照行读取模式  通过GetLine来获取
         ALL, // 一次性读取所有内容 通过GetString 来获取
     };*/
    private StreamReader reader = null;
    private string file;
    private string str = "";
    ArrayList lines = new ArrayList();
    int current_index = 0;
    private bool OpenFile()
    {
        try
        {
            reader = new System.IO.StreamReader(file, System.Text.Encoding.Default);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("open file error " + e.Message);
            return false;
        }
    }

    public static Files Create(string file)
    {
        Files ret = new Files();
        ret.file = DATA.dataPath + "/Resources/" + file;
        if (ret.OpenFile())
        {
            ret.Init();
            return ret;
        }
        else
        {
            return null;
            //open file error
        }

    }
    /// <summary>
    /// return whole file string
    /// </summary>
    /// <returns></returns>
    public string GetString()
    {
        return str;
    }
    /// <summary>
    /// return null when end not ""
    /// </summary>
    /// <returns></returns>
    public string GetNextLine()
    {
        if (lines.Count == 0 || current_index >= lines.Count) return null;
        return (string)(lines[current_index++]);
    }
    public int GetCurrentLineIndex()
    {
        return current_index;
    }
    public int GetLineCount()
    {
        return lines.Count;
    }
    /// <summary>
    /// if you use get line  when you don't need use this function
    /// </summary>
    public Files ResetLines()
    {
        current_index = 0;
        return this;
    }
    private void Init()
    {
        string str = null, line;
        while ((line = reader.ReadLine()) != null)
        {
            str += line.ToString();
            lines.Add(line);
        }
        if (reader != null)
        { // release handle
            reader.Close();
            reader = null;
        }
        this.str = str;
    }

}