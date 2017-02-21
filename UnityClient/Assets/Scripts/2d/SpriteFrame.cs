/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

/// <summary>
/// data structure for animations 
/// </summary>
public sealed class SpriteFrame
{
    public Sprite sprite = null;
    public string name;
    public Vector2 size;
   // public bool rotated = false;
    public Rect rect;
    public SpriteFrame Clone()
    {
        SpriteFrame ret = new SpriteFrame();

        ret.name = name;
        ret.size = size;
      //  ret.rotated = rotated;
        ret.rect = rect;
        // note the Sprite’s texture is the same in Unity even if you Instantiate
        ret.sprite = Sprite.Instantiate(sprite, null) as Sprite; // clone
        return ret;
    }

    public static SpriteFrame CreateWithPng(string png)
    {
        SpriteFrame ret = new SpriteFrame();

        string name = png.Substring(0, png.Length - 4);

        Texture2D tex = (Texture2D)Resources.Load(name);
        if (tex == null) return null;
        Rect rect = new Rect(new Vector2(0.0f, 0.0f), new Vector2(tex.width, tex.height));

        var sp = Sprite.Create(tex, rect, new Vector2(0.5f, 0.0f)) as Sprite;
   
        sp.name = png;
        ret.sprite = sp;
        ret.rect = rect;
        ret.size = new Vector2(tex.width, tex.height);
        ret.name = png;
        return ret;
    }

    public static SpriteFrame Create()
    {
        SpriteFrame ret = new SpriteFrame();
        return ret;
    }

    private SpriteFrame() { }

}
