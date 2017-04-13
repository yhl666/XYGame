/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;



public sealed class AudioMgrImpl
{
    public void PlayMusic()
    {

    }
    public void PlayEffect()
    {

    }


}

/// <summary>
///  
/// </summary>
public sealed class AudioMgr : SingletonGAObject<AudioMgr>
{
    /// <summary>
    /// 发起背景音乐事件
    /// </summary>
    /// <param name="_event"></param>
    public void PostEventMusic(AudioEvents _event)
    {

    }
    /// <summary>
    /// 发起 音效请求
    /// </summary>
    /// <param name="_event"></param>
    public void PostEventEffect(AudioEvents _event)
    {

    }
}
