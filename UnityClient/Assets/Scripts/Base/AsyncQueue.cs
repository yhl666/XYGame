/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
using System.Threading;

sealed class AsyncQueueInfo
{
    public ObjectFuncVoid task = null;
    public VoidFuncObject func_cb = null;
    public bool InMainThread = true;
}




/// <summary>
/// 异步任务队列
/// 全局唯一 可不用DestroyInstance
/// </summary>
public sealed class AsyncQueue
{
    public void Enqueue(ObjectFuncVoid task, VoidFuncObject func_cb = null, bool InMainThread = true)
    {
        AsyncQueueInfo info = new AsyncQueueInfo();
        info.task = task;
        info.InMainThread = InMainThread;
        info.func_cb = func_cb;
        this._queue.Enqueue(info);
    }

    private AsyncQueue()
    {
        this.thread = new Thread(new ThreadStart(this.ThreadFunc));

        this.thread.Start(this);
        Debug.Log("[AsyncQueue]:AsyncQueue Thread Start");
    }
    ~AsyncQueue()
    {
        this.Dispose();
    }
    private void ThreadFunc()
    {
        while (true)
        {
            Thread.Sleep(1);
            if (_queue.Empty() == false)
            {
                AsyncQueueInfo info = _queue.Dequeue() as AsyncQueueInfo;
                object ret = info.task();
                if (info.func_cb == null) return;

                if (info.InMainThread)
                {
                    EventDispatcher.ins.AddFuncToMainThread(() =>
                        {
                            info.func_cb(ret);
                        });
                }
                else
                {
                    info.func_cb(ret);
                }
            }
        }
    }
    public int Count()
    {
        return _queue.Count();
    }
    public static AsyncQueue Create()
    {
        AsyncQueue ret = new AsyncQueue();

        return ret;
    }
    void Dispose()
    {
        if (thread != null)
        {
            thread.Abort();
            thread = null;
        }
        Debug.Log("[AsyncQueue]:AsyncQueue Thread End");
    }

    private Thread thread = null;
    private ThreadSafeQueue _queue = new ThreadSafeQueue();

    public static AsyncQueue ins
    {
        get
        {
            return AsyncQueue.GetInstance();
        }
    }

    private static AsyncQueue _ins = null;

    public static AsyncQueue GetInstance()
    {
        if (_ins == null)
        {
            _ins = new AsyncQueue();
        }
        return _ins;
    }
    public static void DestroyInstance()
    {
        _ins.Dispose();
        _ins = null;
    }
}