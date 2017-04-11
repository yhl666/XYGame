
/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Timers;


public class ClientServerApp : GAObject
{
    public static ClientServerApp ins
    {
        get
        {
            return ClientServerApp.GetInstance();
        }
    }

    private static ClientServerApp _ins = null;

    public static ClientServerApp GetInstance()
    {
        if (_ins == null)
        {
            _ins = new ClientServerApp();
         /*  {


                 PublicData.ins.client_server_room_info = "pvproom_id:270,p1:1,p2:2,mode:pve,";
                 PublicData.ins.is_client_server = true;
   
                 SceneMgr.Load("BattlePVE");

             }*/
            _ins.Init();
            _ins.OnEnter();
        }
        return _ins;
    }




    public override bool Init()
    {

        base.Init();
        try
        {
            if (_inner_socket == null)
                Connect();


        }
        catch (Exception e)
        {


        }
        if (t_send == null)
        {
            t_send = new Thread(new ThreadStart(this.ThreadFunc_Send));
            t_recv = new Thread(new ThreadStart(this.ThreadFunc_Recv));
        }
      ///  this.recvQueue.Enqueue("pvproom_id:69,p1:2,p2:3,");
        return true;
    }

    private void Connect()
    {
        _inner_socket = new TcpClient();

        _inner_socket.Connect("127.0.0.1", 8899);
        _inner_tcp_stream = _inner_socket.GetStream();
        Debug.Log("Connect LogicServer OK");

    }
    public override void OnEnter()
    {
        base.OnEnter();

        t_send.Start();
        t_recv.Start();

    }
    public override void OnDispose()
    {
        if (t_send != null)
        {
            t_send.Abort();
            t_recv.Abort();
        }
        _inner_tcp_stream.Close();
        _inner_socket.Close();
        base.OnDispose();
        Debug.Log("Thread Close");

    }

    public override void OnEvent(int type, object userData)
    {

    }
    public int GetCurrentQueueCount()
    {
        return recvQueue.Count();
    }
    public override void UpdateMS()
    {

        if (is_processing)
        {
            //正在计算
            if (PublicData.ins.client_server_result == "")
            {
                return;//等待计算完成
            }

            this.sendQueue.Enqueue(PublicData.ins.client_server_result);
            PublicData.ins.client_server_result = "";
            is_processing = false;

        }
        else
        {
            Thread.Sleep(10);
            Utils.SetTargetFPS(60);
            this.ProcessOneRequest();
        }


    }


    private void ProcessOneRequest()
    {
        if (recvQueue.Empty() == true) return;
        Debug.Log("队列剩余请求:" + recvQueue.Count());

        string request = recvQueue.Dequeue() as string;
        HashTable kv = Json.Decode(request);

        PublicData.ins.client_server_room_info = request;
        PublicData.ins.is_client_server = true;
        is_processing = true;
        Utils.SetTargetFPS(0xffffff);
 
        string mode = kv["mode"];
        if ( mode == "pvp")
        {
            PublicData.ins.battle_mode = "pvp";
            PublicData.ins.is_pve = false;
            SceneMgr.Load("BattlePVP");
        }
        else
        {
            PublicData.ins.battle_mode = "pve";
            PublicData.ins.is_pve = true;
            SceneMgr.Load("BattlePVE25D");
        }








    }


    private bool is_processing = false;
    private void ThreadFunc_Recv()
    {
        //接受LogicServer请求的线程

        string string_last = "";

        while (true)
        {

            Thread.Sleep(30);


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
                    ///         Debug.Log("recv:" + str);

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

                    recvQueue.Lock();
                    for (int i = 0; i < recv_cache.Count; i++)
                    {
                        recvQueue.UnSafeEnqueue(recv_cache[i] as string);
                        Debug.Log(recv_cache[i] as string);

                    }
                    recvQueue.UnLock();
                }
                else
                {

                }

            }
            catch (Exception e)
            {
                Debug.Log("连接已经断开" + e.Message);

                try
                {
                    Connect();


                }
                catch (Exception ee)
                {
                    Debug.Log(ee.Message);


                }
            };







        }

    }
    private void ThreadFunc_Send()
    {
        //向LogicServer发送处理结果的线程

        while (true)
        {
            Thread.Sleep(30);

            while (sendQueue.Empty() == false)
            {

                string msg = (string)sendQueue.Dequeue() + "^";

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
    ThreadSafeQueue recvQueue = new ThreadSafeQueue();//LogicServer的请求队列
    ThreadSafeQueue sendQueue = new ThreadSafeQueue();// 发送LogicServer的队列


    private static TcpClient _inner_socket = null;
    private static NetworkStream _inner_tcp_stream = null;

    static private Thread t_send = null;
    static private Thread t_recv = null;



}
