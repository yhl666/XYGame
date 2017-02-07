using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml.Linq;
using System.Linq;
using System.IO;


/// <summary>
/// 
/// 
/// for sprite frame animation  which resources from cocos2dx
/// two file must be provided  XXX.plist  XXX.png  
/// @note XXX.pvr.czz is not support
/// 
/// by css
/// 
/// </summary>




///------------------------------------------------for plist parser

/// <summary>
///  data structure for  plist file parse
/// </summary>
sealed class RectInt
{
    public int x;
    public int y;
    public int width;
    public int height;

}


/// <summary>
///  the structure of tree  of key-value
/// </summary>
sealed class PlistDictionary : Dictionary<string, object>
{
    private void LoadWithString(string str)
    {
        var doc = XDocument.Parse(str);
        var dict = doc.Element("plist").Element("dict");

        IEnumerable<XElement> iter = dict.Elements();

        this.Parse(iter);
    }

    private void Parse(IEnumerable<XElement> iter)
    {
        for (int ii = 0; ii < iter.Count(); ii += 2)
        {
            XElement key = iter.ElementAt(ii);
            XElement value = iter.ElementAt(ii + 1);
            var v = this.ParseValue(value);

            this[key.Value] = v;
        }

    }

    private object ParseValue(XElement what)
    {
        string name = what.Name.ToString();

        if (name == "string")
        {
            return what.Value;
        }
        if (name == "integer")
        {
            return int.Parse(what.Value);
        }

        if (name == "true")
        {
            return true;
        }
        if (name == "false")
        {
            return false;
        }
        if (name == "dict")
        {//  sub node
            PlistDictionary ret = new PlistDictionary();
            ret.Parse(what.Elements());
            return ret;
        }

        return null;
    }


    public void LoadWithFile(string plist)
    {
        this.plist = plist;
        string str = Resources.Load(plist).ToString();
        //remove header
        int i = str.IndexOf("<!DOCTYPE");
        if (i != -1)
        {
            str = str.Remove(i, str.IndexOf("\n", i) - i);
        }
        this.LoadWithString(str);
    }


    // helper func
    public static RectInt ParseRectInt(string s)
    {
        RectInt ret = new RectInt();
        s = s.Replace("{", "");
        s = s.Replace("}", "");

        string[] values = s.Split(',');

        if (values.Length != 4)
        {
            throw new Exception(" error");
        }



        ret.x = int.Parse(values[0]);
        ret.y = int.Parse(values[1]);
        ret.width = int.Parse(values[2]);
        ret.height = int.Parse(values[3]);

        return ret;
    }

    public static Vector2 ParseVector2(string s)
    {
        Vector2 ret = new Vector2();
        s = s.Replace("{", "");
        s = s.Replace("}", "");

        string[] values = s.Split(',');

        if (values.Length != 2)
        {
            throw new Exception(" error");
        }
        ret.x = float.Parse(values[0]);
        ret.y = float.Parse(values[1]);
        return ret;
    }
    public string plist;
}


/// <summary>
///  data structure  for plist meta data parse
/// </summary>
sealed class PlistMetaData
{
    public int format;
    public string realTextureFileName;
    public Vector2 size;
    public string smartupdate;
    public string textureFileName;

}

sealed class FrameDataDic
{
    public string name = "";
    //大图 切割大小
    public int x;
    public int y;
    public int width;
    public int height;

    //原图大小
    public int source_x;
    public int source_y;
    public int source_width;
    public int source_height;

    //原始小图大小
    public int sourceSizeWidth;
    public int sourceSizeHeight;

    public int offsetWidth;
    public int offsetHeight;

    public bool rotated;
}

