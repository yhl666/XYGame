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

public enum AudioMode
{
    _2D,
    _3D,
}

/// <summary>
///  
/// </summary>
public sealed class AudioMgr : SingletonGAObject<AudioMgr>
{
        AudioEvents eee = new AudioEvents();

        public override bool Init()
        {
            this.AddSpeaker(0);
            this.AddSpeaker(1);
            eee.LoadAllEvents();

            return true;
        }

        public AudioSpeaker AddSpeaker(int no)
        {
            if (this.GetSpeaker(no) != null) return null;
            var ret = AudioSpeaker.Create(no);
            this.speakers.Add(ret);
            return ret;
        }
        public void PostEvent(AudioEvents.Events e, int no, bool loop = false)
        {
            AudioSpeaker s = this.GetSpeaker(no);
            s.PostEvent(e, loop);
        }
        public void PostEvent(AudioEvents.Events e, bool loop = false)
        {
            this.PostEvent(e, 0, loop);
        }
        public void StopSpeak(int no)
        {
            this.GetSpeaker(no).Stop();
        }

        public AudioSpeaker GetSpeaker(int no)
        {
            foreach (AudioSpeaker s in speakers)
            {
                if (s.no == no)
                {
                    return s;
                }
            }
            return null;
        }
        ArrayList speakers = new ArrayList();
}
