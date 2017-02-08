using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Timers;
using System;



public class SocketClient : object
{

    private static TcpClient _inner_socket = null;
    private static NetworkStream _inner_tcp_stream = null;

    private ThreadSafeQueue _sendQueue;
    //    private ThreadSafeQueue _recvQueue;


    private ArrayList _recv_list;

    private Thread t_send;
    private Thread t_recv;

    private bool isThreadInit = false;
    private bool isConnected = false;
    virtual public void AddSendMsg(string msg)
    {
        _sendQueue.Enqueue(msg);
    }

    virtual public bool Startup()
    {
        this.InitSocket();
        if (this.Connect())
        {
            this.InitThread();
            Debug.Log("[NetWork]:Socket Thread Open");
        }
        else
        {
            return false;
        }
        return true;

    }
    virtual public void Terminal()
    {

        if (isThreadInit)
        {
            t_send.Abort();
            t_recv.Abort();

            Debug.Log("[NetWork]:Socket Thread Close");
        }
        if (isConnected)
        {
            _inner_tcp_stream.Close();
            _inner_socket.Close();
        }
    }

    private bool Connect()
    {
        try
        {
            _inner_socket.Connect(Config.SERVER_IP, Config.SERVER_PORT);
            _inner_tcp_stream = _inner_socket.GetStream();
            this.isConnected = true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
        return true;
    }



    private void InitSocket()
    {
        _inner_socket = new TcpClient();
        //  _recvQueue = new ThreadSafeQueue();
        _recv_list = new ArrayList();

        _sendQueue = new ThreadSafeQueue();

    }
    private void InitThread()
    {

        t_send = new Thread(new ThreadStart(this.ThreadFunction_Send));
        t_recv = new Thread(new ThreadStart(this.ThreadFunction_Recv));

        t_send.Start();
        t_recv.Start();

        isThreadInit = true;
    }

    private void ThreadFunction_Send()
    {

        while (true)
        {
            while (_sendQueue.Empty() == false)
            {
                Thread.Sleep(1);

                string msg = (string)_sendQueue.Dequeue() + "^";

                byte[] buffer = System.Text.Encoding.Default.GetBytes(msg);
                //   Debug.Log("send:" +msg);
                if (buffer.Length > Config.MAX_NETSOCKET_BUFFER_SIZE)
                {
                    Debug.LogError("buffer out of range");
                }
                //    Debug.Log("Send " + msg);
                _inner_tcp_stream.Write(buffer, 0, buffer.Length);
            }
        }

    }

    private void ThreadFunction_Recv()
    {


        double delay = 0.0;

        DateTime lastTime = DateTime.Now;

        int counter = 0;

        string string_last = "";

        while (true)
        {
            Thread.Sleep(1);


            byte[] buffer = new byte[Config.MAX_NETSOCKET_BUFFER_SIZE];
            int c = 0;
            try
            {
                c = _inner_tcp_stream.Read(buffer, 0, Config.MAX_NETSOCKET_BUFFER_SIZE);

                //process data
                if (0 < c)
                {
                    string str = System.Text.Encoding.Default.GetString(buffer, 0, c);
                    string_last += str;
                    //     Debug.Log("recv:" + str);

                    ArrayList recv_cache = new ArrayList();

                    int last = 0;
                    for (int i = 0; i < string_last.Length; i++)
                    {

                        char ch = string_last[i];
                        char t = '^';
                        if (ch.Equals(t))
                        {

                            string s = string_last.Substring(last, i - last);
                            if (s != "" && s != null)
                            {
                                recv_cache.Add(s);
                            }
                            last = i + 1;
                        }
                    }
                    string_last = string_last.Substring(last);

                    //sync cache to main thread
                    BattleApp.GetInstance().LockAdRecvMsg();
                    for (int i = 0; i < recv_cache.Count; i++)
                    {

                        //   BattleApp.GetInstance()._recvQueue.UnSafeEnqueue(recv_cache[i] as string);

                        BattleApp.GetInstance().AddRecvMsgUnSafe(recv_cache[i] as string);
                        _recv_list.Add(recv_cache[i] as string);
                        PublicData.GetInstance()._recv_last_game = _recv_list;

                    }
                    BattleApp.GetInstance().UnLockAdRecvMsg();

                }
                else
                {

                }

            }
            catch (Exception e)
            {
                Debug.Log("连接已经断开" + e.Message);
                this.isConnected = false;
                EventDispatcher.ins.AddFuncToMainThread(new Func<int>(() =>
                    {
                        EventDispatcher.ins.PostEvent(Events.ID_NET_DISCONNECT);

                        return 0;
                    }));


                return;
            };

            //process UI
            if (0 < c)
            {
                counter++;
                if (counter >= 10)
                {

                    // 25 is fixed tick
                    TimeSpan ts = DateTime.Now - lastTime;
                    delay = ts.TotalMilliseconds / 10.0f - 25.0f;
                    lastTime = DateTime.Now;
                    if (delay < 0) delay = 0;
                    PublicData.GetInstance().ms = (int)delay;


                    counter = 0;
                }
            }
        }
    }


}


public class SockClientWithVideoMode : SocketClient
{


    public ArrayList _recv_list;
    private Thread t_recv;


    public override void AddSendMsg(string msg)
    {

    }

    public override bool Startup()
    {

        this.InitThread();
        return true;
    }
    public override void Terminal()
    {

        t_recv.Abort();

        Debug.Log("[NetWork]:Socket Thread Close");

    }

    private void InitThread()
    {
        this._recv_list = PublicData.GetInstance()._recv_last_game;

        t_recv = new Thread(new ThreadStart(ThreadFunction_Recv));

        t_recv.Start();
        Debug.Log("[NetWork]:Socket Thread Start");

    }


    private void ThreadFunction_Recv()
    {

        int onenter_max_fps = PublicData.GetInstance()._on_enter_max_fps;

        int _recv_list_index = 0;


        //sync cache to main thread
        BattleApp.GetInstance().LockAdRecvMsg();

        while (_recv_list_index < onenter_max_fps)
        {
            BattleApp.GetInstance().AddRecvMsgUnSafe(_recv_list[_recv_list_index] as string);
            _recv_list_index++;
        }
        BattleApp.GetInstance().UnLockAdRecvMsg();

        while (true)
        {
            Thread.Sleep(this._thread_delte);
            BattleApp.GetInstance().AddRecvMsg(_recv_list[_recv_list_index] as string);
            _recv_list_index++;
        }
    }



    Mutex _mutex = new Mutex();
    private int _thread_delte = 1000 / Config.MAX_FPS;

    public void SetPlayeSpeed(int sp)
    {
        _mutex.WaitOne();
        this._thread_delte = sp;
        _mutex.ReleaseMutex();
    }
}