/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class AudioSpeaker
{
    public int no = 0;
    private AudioSource source = null;
    public static AudioSpeaker Create(int no)
    {
        AudioSpeaker ret = new AudioSpeaker();
        ret.no = no;
        ret.Init();
        return ret;
    }
    void Init()
    {
        source = Camera.main.gameObject.AddComponent<AudioSource>();
    }
    public void Speak(AudioClip clip, bool loop = false)
    {
        this.source.Stop();
        if (loop)
        {
            this.source.clip = clip;
            this.source.loop = true;
            this.source.Play();
        }
        else
        {
            this.source.loop = false;
            this.source.PlayOneShot(clip);
        }
    }

    public void PostEvent(AudioEvents.Events e, bool loop = false)
    {
        Speak(AudioEvents.ins.GetClip(e), loop);
    }
    public void Stop()
    {
        this.source.Stop();
    }
}