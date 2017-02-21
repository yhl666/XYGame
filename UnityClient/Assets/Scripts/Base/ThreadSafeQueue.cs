/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System.Threading;


public sealed class ThreadSafeQueue : object
{
    public void Enqueue(object obj)
    {
        _mutex.WaitOne();
        _queue.Enqueue(obj);
        _mutex.ReleaseMutex();

    }
    public int Count()
    {
        _mutex.WaitOne();
        int c = _queue.Count;
        _mutex.ReleaseMutex();
        return c;

    }
    public object Dequeue()
    {
        _mutex.WaitOne();
        if (_queue.Count == 0)
        {
            _mutex.ReleaseMutex();
            return null;

        }
        object obj = _queue.Dequeue();
        _mutex.ReleaseMutex();

        return obj;
    }

    public bool Empty()
    {
        _mutex.WaitOne();
        bool ret = (0 == _queue.Count);
        _mutex.ReleaseMutex();
        return ret;

    }
    public void Clear()
    {
        _mutex.WaitOne();
        _queue.Clear();
        _mutex.ReleaseMutex();
    }





    //un safe

    public void Lock()
    {
        _mutex.WaitOne();
    }
    public void UnLock()
    {
        _mutex.ReleaseMutex();
    }
    public void UnSafeEnqueue(object obj)
    {
        _queue.Enqueue(obj);
    }
    public int UnSafeCount()
    {
        int c = _queue.Count;
        return c;

    }
    public object UnSafeDequeue()
    {
        if (_queue.Count == 0)
        {
            return null;
        }
        object obj = _queue.Dequeue();
        return obj;
    }

    public bool UnSafeEmpty()
    {
        bool ret = (0 == _queue.Count);
        return ret;

    }
    public void UnSafeClear()
    {
        _queue.Clear();
    }


    public ThreadSafeQueue()
    {
        _queue = new Queue();
        _mutex = new Mutex();
    }


    private Queue _queue;
    public Mutex _mutex;
};

