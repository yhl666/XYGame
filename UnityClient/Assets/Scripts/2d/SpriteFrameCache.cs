/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
///  sprite frame cache 
///   note this will create a  deep copy  SpriteFrame when you get it by name
/// </summary>
public sealed class SpriteFrameCache:Singleton<SpriteFrameCache>
{
    public void AddSpriteFrame(string name, SpriteFrame sp)
    {
        if (hash.Contains(name) == true)
        {
            hash[name] = sp;
            return;
        }
        hash.Add(name, sp);
    }

    public SpriteFrame AddSpriteFrameWithPng(string png)
    {
        SpriteFrame sp = SpriteFrame.CreateWithPng(png);

        if (sp != null)
        {
            this.AddSpriteFrame(png, sp);
        }
        return sp;
    }
    public SpriteFrame GetSpriteFrame(string name)
    {
        if (hash.Contains(name)) return (hash[name] as SpriteFrame).Clone();
        return null;
    }
    /// <summary>
    /// miss 自动添加
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public SpriteFrame GetSpriteFrameAuto(string png)
    {
        SpriteFrame sp = this.GetSpriteFrame(png);

        if (sp == null)
        {
            return this.AddSpriteFrameWithPng(png);
        }
        return sp;
    }

    public void RemoveSpriteFrame(string name)
    {
        if (hash.Contains(name))
        {
            SpriteFrame sp = hash[name] as SpriteFrame;

            hash[name] = null;//release references

            hash.Remove(name);
        }
    }

    public void RemoveSpriteFrameWithFile(string plist)
    {
        if (hash_plist_sprieFrame.Contains(plist))
        {
            ArrayList list = hash_plist_sprieFrame[plist] as ArrayList;
            foreach (string s in list)
            {
                this.RemoveSpriteFrame(s);
            }
            hash_plist_sprieFrame[plist] = null;
            hash_plist_sprieFrame.Remove(plist);
        }
    }

    public ArrayList AddSpriteFrameWithFile(string plist)
    {

        PlistDictionary dic = new PlistDictionary();
        dic.LoadWithFile(plist);


        return this.LoadWithFrameDic(dic, plist);
    }

    private ArrayList LoadWithFrameDic(PlistDictionary dic, string plist)
    {
        string path = Utils.GetFilePath(plist);

        // init  data
        ArrayList frames = new ArrayList();

        foreach (KeyValuePair<string, object> kv in dic["frames"] as PlistDictionary)
        { // parse frame data

            if (kv.Value is PlistDictionary)
            {
                FrameDataDic d = new FrameDataDic();
                PlistDictionary frameDic = kv.Value as PlistDictionary;

                RectInt frame = PlistDictionary.ParseRectInt(frameDic["frame"] as string);
                d.x = frame.x;
                d.y = frame.y;
                d.width = frame.width;
                d.height = frame.height;

                Vector2 offset = PlistDictionary.ParseVector2(frameDic["offset"] as string);
                d.offsetHeight = (int)offset.y;
                d.offsetWidth = (int)offset.x;

                d.rotated = (bool)frameDic["rotated"];

                RectInt source = PlistDictionary.ParseRectInt(frameDic["sourceColorRect"] as string);
                d.source_x = source.x;
                d.source_y = source.y;
                d.source_height = source.height;
                d.source_width = source.width;
                d.name = kv.Key;
                Vector2 size = PlistDictionary.ParseVector2(frameDic["sourceSize"] as string);
                d.sourceSizeHeight = (int)size.y;
                d.sourceSizeWidth = (int)size.x;
                frames.Add(d);

            }

        }

        // parse meta data
        var meta = (dic["metadata"] as PlistDictionary);
        PlistMetaData metaData = new PlistMetaData();

        metaData.format = (int)meta["format"];
        metaData.realTextureFileName = meta["realTextureFileName"] as string;
        metaData.size = PlistDictionary.ParseVector2(meta["size"] as string);
        metaData.smartupdate = meta["smartupdate"] as string;
        metaData.textureFileName = meta["textureFileName"] as string;
        // init  texture2d and sprite

        string name = metaData.realTextureFileName.Substring(0, metaData.realTextureFileName.Length - 4);
        ArrayList ret = new ArrayList();
        ArrayList spriteFrameNames = new ArrayList();
        Texture2D tex_origin = (Texture2D)Resources.Load(path + name);
        if (tex_origin == null) return null;
        /// var frame1 = frames[0] as FrameDataDic;

        foreach (FrameDataDic frame in frames)
        {
            Texture2D tex = null;
            Color[] datas = null;

            // fill with empty pixel
            Color[] d = new Color[frame.sourceSizeWidth * frame.sourceSizeHeight];
            for (int ii = 0; ii < frame.sourceSizeWidth * frame.sourceSizeHeight; ii++)
            {
                d[ii] = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }

            // there 2 way to process rotate

            ///-------------------way 1 to process rotate

            /*     if (frame.rotated)
                 {// rotate  ,   exchange  width with height 
                     //    continue;
                     datas = tex_origin.GetPixels(frame.x, tex_origin.height - frame.y - frame.width, frame.height, frame.width);
                     tex = new Texture2D(frame.sourceSizeHeight, frame.sourceSizeWidth, tex_origin.format, false);
                     //  tex.SetPixels(0, 0, frame.sourceSizeHeight, frame.sourceSizeWidth, d); // reset all
                     tex.SetPixels(0//-frame.offsetHeight + frame.source_y
                         , -frame.offsetWidth + frame.source_x, frame.source_height, frame.source_width, datas);
                 }
                 else
                 {
                     datas = tex_origin.GetPixels(frame.x, tex_origin.height -
                         frame.y
                         - frame.height
                         , frame.width, frame.height);

                     tex = new Texture2D(frame.sourceSizeWidth, frame.sourceSizeHeight, tex_origin.format, false);
                     //  tex.SetPixels(0, 0, frame.sourceSizeWidth, frame.sourceSizeHeight, d);// reset all
                     tex.SetPixels(-frame.offsetWidth + frame.source_x, 0// frame.offsetHeight + frame.source_y
                         , frame.width, frame.height, datas);
                 }

                 tex.Apply();

                 Sprite sp = null;
                 Rect rect = new Rect(new Vector2(0, 0), new Vector2(tex.width, tex.height));
                 if (frame.rotated)
                 {  // pivot
                     sp = Sprite.Create(tex, rect, new Vector2(0f, 0.5f));
                 }
                 else
                 {
                     sp = Sprite.Create(tex, rect, new Vector2(0.5f, 0.0f));
                 }

                 */

            ///-------------------way 2 to process rotate

            ////process rotate for texture2D
            if (frame.rotated)
            {// rotate  ,   exchange  width with height 
                datas = tex_origin.GetPixels(frame.x, tex_origin.height - frame.y - frame.width, frame.height, frame.width);

                ArrayList dd = new ArrayList();
                for (int hh = 0; hh < frame.height; hh++)
                {
                    for (int ww = frame.width - 1; ww >= 0; ww--)
                    {
                        dd.Add(datas[frame.height * ww + hh]);
                    }
                }
                int multi = frame.width * frame.height;
                for (int ii = 0; ii < multi; ii++)
                {
                    datas[ii] = (Color)dd[ii];
                }
            }
            else
            {

                datas = tex_origin.GetPixels(frame.x, tex_origin.height -
                    frame.y
                    - frame.height
                    , frame.width, frame.height);
            }
            tex = new Texture2D(frame.sourceSizeWidth, frame.sourceSizeHeight, tex_origin.format, false);
            tex.SetPixels(0, 0, frame.sourceSizeWidth, frame.sourceSizeHeight, d);// reset all
            tex.SetPixels(-frame.offsetWidth + frame.source_x, 0// frame.offsetHeight + frame.source_y
                , frame.width, frame.height, datas);
            tex.Apply();

            Sprite sp = null;
            Rect rect = new Rect(new Vector2(0, 0), new Vector2(tex.width, tex.height));

            sp = Sprite.Create(tex, rect, new Vector2(0.5f, 0f));

            sp.name = frame.name;

            var ss = SpriteFrame.Create();
            ss.sprite = sp;
            ss.name = frame.name;
            //  ss.rotated = frame.rotated;
            ss.rect = rect;
            ss.size = new Vector2(frame.width, frame.height);
            this.AddSpriteFrame(ss.name, ss);
            ret.Add(ss.Clone()); // return the clone
            spriteFrameNames.Add(ss.name);

        }

        //add to plist hash table
        if (hash_plist_sprieFrame.Contains(dic.plist))
        {
            hash_plist_sprieFrame[dic.plist] = spriteFrameNames;
        }
        else
        {
            hash_plist_sprieFrame.Add(dic.plist, spriteFrameNames);
        }

        //cache 内部 是 SpriteFrame 的唯一引用
        //get 获取的始终是 SpriteFrame的Clone 对象

        ///    SpriteFrame sssp = ret[0] as SpriteFrame;

        //texture2d is the same
        //  Debug.Log(sssp.Clone().sprite.texture.GetHashCode() + "         " + sssp.sprite.texture.GetHashCode());
        //   Debug.Log(sssp.Clone().sprite.GetHashCode() + "         " + sssp.sprite.GetHashCode());

        return ret;
    }


    public void Clear()
    {
        hash.Clear();
        hash_plist_sprieFrame.Clear();
    }

    public static void PrintCacheStatus()
    {
        Debug.Log("SpriteFrameCache: " + ins.hash.Count + " in Cache");
        foreach (DictionaryEntry kv in ins.hash)
        {
            Debug.Log("SpriteFrameCache: " + kv.Key);

        }
    }

    private Hashtable hash = new Hashtable();//sprite frame name ---- spriteframe
    private Hashtable hash_plist_sprieFrame = new Hashtable(); //  sprite plist file  -----  spriteframe's name (string) ArrayList

}

