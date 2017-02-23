
/*

Author cao shanshan
Email me@dreamyouxi.com

*/
#include "ClientServer.h"
#include "world.h"  // for GetLuaState()
#include "get_world.h"  // for GetWorld()
#include "timer_queue_root.h"
#include "timer_queue.h"

#include <LuaIntf/LuaIntf.h>
#include "windows.h"
#include <iostream>
#include <winsock2.h>

#pragma comment(lib,"ws2_32.lib") 

using namespace std;
namespace {


	using LuaRef = LuaIntf::LuaRef;


	void Send(const string& what)
	{
		ClientServer::GetInstance()->AddSendMsg(what);
	}






	namespace LuaClientServer {

		void Bind(lua_State* L)
		{
			assert(L);
			using namespace LuaIntf;
			LuaBinding(L).beginModule("client_server")
				.addFunction("send", &Send)

				.endModule();
		}

	};

};

 



void ClientServer::BindLua(lua_State *l)
{
	LuaClientServer::Bind(l);
}



void   ClientServer::AddSendMsg(std::string msg)
{
	sendQueue.push(msg + "^");
}


ClientServer::ClientServer()
{

	m_pTimerQueue.reset(new TimerQueue);  // unique_ptr

	TimerQueueRoot& rTimerQueueRoot = TimerQueueRoot::Get();
	m_pTimerQueue->SetParent(&rTimerQueueRoot);

	this->InitSocket();

	std::thread t1(std::bind(&ClientServer::ThreadFunc_Recv, this));
	t1.detach();

	std::thread t2(std::bind(&ClientServer::ThreadFunc_Send, this));
	t2.detach();

}

void   ClientServer::InitSocket()
{
	WSADATA wsadata;
	WORD wVersion = MAKEWORD(2, 0);
	WSAStartup(wVersion, &wsadata);

	int   length;
	length = sizeof(sockaddr);

	struct sockaddr_in server_ipaddr;
	memset(&server_ipaddr, 0, length);
	server_ipaddr.sin_family = AF_INET;
	server_ipaddr.sin_port = htons(8899);

	server_ipaddr.sin_addr.s_addr = htonl(0);

	this->socket = ::socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

	::bind(this->socket, (sockaddr *)&server_ipaddr, length);

	listen(this->socket, 180);

	cout << "Startup Server at port 8899" << endl;;

	//阻塞运行

	this->AcceptConnected();

}


void   ClientServer::AcceptConnected()
{
	Sleep(1);
	struct sockaddr_in  client_ipaddr;
	int len = sizeof(sockaddr);
	int socket = accept(this->socket, (sockaddr *)&client_ipaddr, &len);
	if (socket == SOCKET_ERROR)
	{
		exit(0);
	}
	else if (socket == INVALID_SOCKET)
	{
		cout << "error code :" << WSAGetLastError() << endl;
		WSACleanup();
		exit(0);

	}
	isConnected = true;
	socket_client_server = socket;
	cout << "Accept ClientServer Connected ok" << endl;;
}


void   ClientServer::ThreadFunc_Recv()
{
	while (true)
	{
		Sleep(1);
		char buffer[4096];

		int ret = ::recv(socket_client_server, buffer, 2000, 0);

		if (ret == SOCKET_ERROR)
		{
			isConnected = false;
			this->AcceptConnected(); 
			continue;;
		}
		if (ret > 0)
		{
			buffer[ret] = 0;
			recvQueue.push(buffer);
			// process
			read_write_mutex.lock();
			this->ProcessInLua(buffer);
			read_write_mutex.unlock();
		}
	}
}

void   ClientServer::ThreadFunc_Send()
{
	/*this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");
	*/
	while (true)
	{
		Sleep(1);
		while (sendQueue.empty() == false)
		{
			string msg = sendQueue.front();
			cout << "send " << msg << endl;
			int ret = ::send(socket_client_server, msg.c_str(), msg.size(), 0);

			if (ret == SOCKET_ERROR)
			{
				continue;;
			}
			sendQueue.pop();
		}
	}
}


void ClientServer::ProcessInLua(const std::string & msg)
{
	/*GetWorld()->AddMainThreadFunc([=]()
	{

		using LuaIntf::LuaRef;
		LuaRef require(GetWorld()->GetLuaState(), "require");
		try
		{
			LuaRef handler = require.call<LuaRef>("xygame.base.client_server_handler");

			handler.dispatchStatic("handle", std::move( msg));
			// Todo: Register Lua service directly. No rpc_request_handler.
		}
		catch (const LuaIntf::LuaException& e)
		{
			cout << "Failed to call lua xygame.base.client_server_handler.handle(), " << e.what();
		}


	});*/

	m_pTimerQueue->InsertSingleFromNow(0, [=]()
	{
		//GetWorld()->ProcessThreadFunc();
		using LuaIntf::LuaRef;
		LuaRef require(GetWorld()->GetLuaState(), "require");
		try
		{
			LuaRef handler = require.call<LuaRef>("xygame.base.client_server_handler");

			handler.dispatchStatic("handle_client_server", (msg.substr(0,msg
				.size()-1)));
			// Todo: Register Lua service directly. No rpc_request_handler.

		///	this->AddSendMsg("pvproom_id:20,p1:1,p2:2,");

		}
		catch (const LuaIntf::LuaException& e)
		{
			cout << "Failed to call lua xygame.base.client_server_handler.handle_client_server(), " << e.what();
		}

	});

	///ClientServer::GetInstance()->AddSendMsg("hello ok^");
}
