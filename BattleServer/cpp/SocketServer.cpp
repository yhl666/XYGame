
#include "SocketServer.h"
#include "windows.h"
#include <iostream>
#include "SocketClient.h"
#include "ClientEventHandler.h"
#include "Buffer.h"
#include "Config.h"
#include "Utils.h"
#pragma comment(lib,"ws2_32.lib") 

using namespace std;

/*
vector<Buffer> &   SocketServer::Recv()
{
for (auto cli : clients)
{
this->_datas.push_back(cli->Recv());
}
return _datas;
}*/

SocketClient *   SocketServer::AcceptLoop()
{
	Sleep(1);
	struct sockaddr_in  client_ipaddr;
	int len = sizeof(sockaddr);
	int socket = accept(this->socket, (sockaddr *)&client_ipaddr, &len);
	if (socket == SOCKET_ERROR)
	{
		return nullptr;
	}
	else if (socket == INVALID_SOCKET)
	{
		cout << "error code :" << WSAGetLastError() << endl;
		WSACleanup();
		ZT_ASSERT(false, 0);

	}
	SocketClient*ret = SocketClient::Create(socket);

	//	this->clients.push_back(ret);
	return ret;

}


bool   SocketServer::Init()
{
	WSADATA wsadata;
	WORD wVersion = MAKEWORD(2, 0);
	WSAStartup(wVersion, &wsadata);

	int sock, length;
	length = sizeof(sockaddr);

	struct sockaddr_in server_ipaddr;
	memset(&server_ipaddr, 0, length);
	server_ipaddr.sin_family = AF_INET;
	server_ipaddr.sin_port = htons(Config::SERVER_PORT);

	//server_ipaddr.sin_addr.s_addr = htonl(0);
	 server_ipaddr.sin_addr.s_addr = inet_addr("192.168.1.200");
	;
	// htonl("192.168.1.200");

	sock = ::socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

	::bind(sock, (sockaddr *)&server_ipaddr, length);

	listen(sock, 180);
	this->socket = sock;
	Utils::log("Startup Server at port %d", Config::SERVER_PORT);


	u_long mode = 1;
	if (0 == ioctlsocket(sock, FIONBIO, &mode))//OK
	{
		return true;
	}
	else
	{
		exit(0);

		return false;
	}


	return true;

}

//send msg to all clients
/*void Broadcast(string msg)
{
for (auto cli : clients)
{
cli->Send(msg);
}
}

//send a msg to a client
int Send(SocketClient *client, string msg)
{
client->Send(msg);
}*/