#ifndef __BASE_SERVER_HEAD__CLIENTSERVER
#define __BASE_SERVER_HEAD__CLIENTSERVER



#include <string>
#include <thread>
#include <mutex>
#include <queue>
#include <memory>
#include "timer_queue.h"
struct lua_State;
 
template< typename T>
class ThreadSafeQueue
{
#define  LOCK(WHAT) std::lock_guard<std::mutex> locker(WHAT);

public:
	void pop()
	{
		LOCK(_mutex);
		_queue.pop();
	}
	void push(const T &t)
	{
		LOCK(_mutex);
		_queue.push(t);
	}
	const  T &  front()
	{
		LOCK(_mutex);
		return _queue.front();
	}
	bool empty()
	{
		LOCK(_mutex);
		bool ret = _queue.empty();
		return ret;
	}
private:
	std::mutex _mutex;
	std::queue<T> _queue;
};

class RequestInfo
{
public:
	std::string key;
	std::string value;
};

class ClientServer
{
public:

	static ClientServer *GetInstance()
	{
		static ClientServer ins;
		return &ins;//thread safe with c++11
	}
private:

public:


	/*
	* @brief 发送请求接口
	*/
	void AddSendMsg(std::string msg);

	void BindLua(lua_State *l);

private:
	ClientServer();
	void ProcessInLua(const std::string & msg);
	void InitSocket();
	void AcceptConnected();


	void ThreadFunc_Recv();

	void ThreadFunc_Send();


	ThreadSafeQueue<std::string> sendQueue;
	ThreadSafeQueue<std::string> recvQueue;
	int socket_client_server = 0;
	int socket = 0;

	std::unique_ptr<TimerQueue> m_pTimerQueue;

	std::mutex read_write_mutex;

};
















#endif		// __BASE_SERVER_HEAD__
