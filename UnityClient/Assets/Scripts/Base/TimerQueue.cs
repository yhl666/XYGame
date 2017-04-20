/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class TimerQueue : Singleton<TimerQueue>
{
    /// <summary>
    ///  添加一个帧同步定时器
    /// </summary>
    /// <param name="fps_delay"></param>
    /// <param name="cb"></param>
    /// <param name="repeat_times">  <0 will be run forever</param>
    public void AddTimerMSI(int fps_delay, VoidFuncVoid cb, int repeat_times = 1)
    {
        if (repeat_times == 0) return;
        TimerMS time = new TimerMS();
        time.delay = fps_delay;
        time.cb = cb;
        time.repeat_times = repeat_times;
        time.Init();
        list_ms.Add(time);
    }
    public void AddTimerMS(float time_delay, VoidFuncVoid cb, int repeat_times = 1)
    {
        if (repeat_times == 0) return;
        this.AddTimerMSI((int)(time_delay / Utils.deltaTime), cb, repeat_times);
    }
    /// <summary>
    /// 添加一个真实时间定时器
    /// </summary>
    /// <param name="time_delay"></param>
    /// <param name="cb"></param>
    /// <param name="repeat_times">  <0 will be run forever</param>
    public void AddTimer(float time_delay, VoidFuncVoid cb, int repeat_times = 1)
    {
        if (repeat_times == 0) return;
        Timer time = new Timer();
        time.delay = time_delay;
        time.cb = cb;
        time.repeat_times = repeat_times;
        time.Init();
        list.Add(time);
    }


    /// <summary>
    /// 帧同步 tick
    /// </summary>
    public void TickMS()
    {
        this.Tick(ref list_ms);
    }
    /// <summary>
    /// unity engine tick
    /// </summary>
    public void Tick()
    {
        this.Tick(ref list);
    }
    private void Tick(ref List<TimerBase> list)
    {
        int list_count = list.Count;
        for (int i = 0; i < list_count; i++)
        {
            TimerBase timer = list[i] as TimerBase;
            timer.Tick();
        }

        for (int i = 0; i < list.Count; )
        {
            TimerBase b = list[i] as TimerBase;
            if (b.IsInValid())
            {
                list.Remove(b);
            }
            else
            {
                ++i;
            }
        }
    }
    public void Clear()
    {
        this.list_ms.Clear();
        this.list.Clear();
    }
    List<TimerBase> list_ms = new List<TimerBase>();
    List<TimerBase> list = new List<TimerBase>();

}


class TimerBase : GAObject
{
    public virtual void Tick()
    {

    }

    public VoidFuncVoid cb = null;
    public int repeat_times = 1;
    protected int repeat_times_current = 0;
}



sealed class TimerMS : TimerBase
{
    public override void Tick()
    {
        if (tick.Tick())
        {
            return;
        }
        if (cb != null)
        {
            cb();
        }
        tick.Reset();

        if (++repeat_times_current >= repeat_times && repeat_times >= 0)
        {
            this.SetInValid();
        }

    }

    public TimerMS()
    {

    }

    public override bool Init()
    {
        tick.SetMax(delay);

        return true;
    }
    Counter tick = Counter.Create();
    public int delay = 0;

}
sealed class Timer : TimerBase
{
    public override void Tick()
    {
        if (current < delay)
        {
            current += Time.deltaTime;
            return;
        }
        if (cb != null)
        {
            cb();
        }
        current = 0.0f;
        if (++repeat_times_current >= repeat_times && repeat_times >= 0)
        {
            this.SetInValid();
        }
    }

    public override bool Init()
    {
        return true;
    }
    float current = 0.0f;
    public float delay = 0.0f;

}
