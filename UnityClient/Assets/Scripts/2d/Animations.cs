using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;



/// <summary>
/// frame animaton res from cocos2dx
/// </summary>
public sealed class Animations : GAObject
{
    public VoidFuncVoid _OnComplete = null;// when complete this will be called
    private int loop = 1; // if <0 the will loop forever
    public Animations Clone()
    {
        Animations ret = new Animations();
        ret.frames = frames; // read only so could share frames
        ret.perFrame = perFrame;
        ret.name = name;
        return ret;
    }
    public SpriteRenderer target = null;
    public int current = 0;
    public int perFrame = 3;
    public int index = 0;
    public string name;


    public void SetPerFrameDelay(int perFrame)
    {
        this.perFrame = perFrame;
    }
    public SpriteFrame GetCurrentSpriteFrame()
    {
        return null;

    }
    public override void UpdateMS()
    {
        if (_run == false) return;

        if (this.IsInValid()) return;

        if (IsComplete())
        {
            loop--;
            if (_OnComplete != null) _OnComplete.Invoke();
            if (loop != 0)
            {
                index = 0;
            }
            else
            {
                return;
            }
        }
        current++;
        if (current >= perFrame)
        {
            SpriteFrame frame = (frames[index++] as SpriteFrame);

            target.sprite = frame.sprite;
            current = 0;
        }
    }

    /// <summary>
    ///  is complete
    /// </summary>
    /// <returns> bool </returns>
    public bool IsComplete()
    {
        return index >= frames.Count;
    }
    /// <summary>
    ///  set loop times  <0  is forever loop
    /// </summary>
    /// <param name="loop"></param>
    public void SetLoop(int loop)
    {
        this.loop = loop;
    }

    /// <summary>
    ///  create with plist png atlas file 
    /// </summary>
    /// <param name="plist"></param>
    /// <returns></returns>
    public static Animations CreateWithFile(string plist, SpriteRenderer target = null)
    {
        Animations ret = new Animations();
        ret.name = plist;
        ArrayList frames = SpriteFrameCache.ins.AddSpriteFrameWithFile(plist);
        if (frames == null || frames.Count == 0)
        {
            Debug.LogError("frames is empty " + plist);
            throw new NullReferenceException();
        }

        string file = Utils.GetFileName(plist);

        if (file.IndexOf("_") != -1)
        {
            //auto sort by file names
            // as XXX_0.png
            // as XXX_1.png
            // as XXX_2.png
            // as XXX_AB.png
            ArrayList framess = new ArrayList();
            for (int i = 0; ; i++)
            {
                string name = (file + "_" + i.ToString() + ".png");
                foreach (SpriteFrame frame in frames)
                {
                    if (frame.name == name)
                    {
                        framess.Add(frame);
                        break;
                    }
                }
                if (framess.Count == i + 1)
                {// find
                    ///   Debug.Log(name);
                    continue;
                }
                else
                {//not find 
                    break;
                }
            }
            if (framess.Count > 0)
                frames = framess;
        }

        ret.frames = frames;
        ret.target = target;
        return ret;
    }


    public static Animations Create(SpriteRenderer target = null)
    {
        Animations ret = new Animations();
        ret.target = target;
        return ret;
    }

    public void AddSpriteFrame(SpriteFrame sp)
    {
        this.frames.Add(sp);
    }

    /// <summary>
    ///  create with plist  animation file
    /// </summary>
    /// <param name="plist"></param>
    /// <returns></returns>
    public static Animations CreateWithAnimationFile(string plist)
    {
        Animations ret = new Animations();

        return null;
    }


    public override void OnDispose()
    {
        frames = null;
        base.OnDispose();
    }

    private Animations() { }
    ~Animations() { frames = null; }// release reference
    //read only
    private ArrayList frames = new ArrayList();// frame data

    private bool _run = false;

    public void Run()
    {
        this._run = true;
        this.UpdateMS();
    }
}
