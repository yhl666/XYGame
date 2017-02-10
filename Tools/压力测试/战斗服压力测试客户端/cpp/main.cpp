#include <algorithm>
#include<iostream>
#include "functional"
#include <fstream>
#include <cmath>
#include "stdio.h"
#include "stdlib.h"
#include "conio.h"
#include <thread>
#include<random>
#include <memory>
#include <map>
#include <mutex>
#include <unordered_map>
#include <fstream>
#include <set>
#include <cmath>
#include <chrono>
#include <iostream>
#include <winsock2.h>
#include "assert.h"
#include <string>
using namespace std;
using namespace std::chrono;

#pragma comment (lib,"wsock32")

#include "FrameData.h"



string ip;

class Client
{
public:
	~Client()
	{
		closesocket(socket);
	}
	static Client* Create()
	{
		Client* ret = new Client;
		ret->Init();
		return ret;
	}
	int Send(string what)
	{
		what += '^';
		int n = ::send(socket, what.c_str(), what.size(), 0);
		return n;
	}

	string   Recv()
	{
		char buf[1000];
		memset(buf, 0, 1000);
		int len = ::recv(socket, buf, 999, 0);
		/////	cout << len  <<"   "<<buf<< endl;
		return  (buf);
	}
private:

	void Init()
	{
		WSADATA wsadata;
		WORD wVersion = MAKEWORD(2, 0);
		WSAStartup(wVersion, &wsadata);

		int sock, length;
		length = sizeof(sockaddr);

		struct sockaddr_in server_ipaddr;
		memset(&server_ipaddr, 0, length);
		server_ipaddr.sin_family = AF_INET;
		server_ipaddr.sin_port = htons(8111);

		//	server_ipaddr.sin_addr.s_addr = htonl(0);

	
		server_ipaddr.sin_addr.S_un.S_addr = inet_addr(ip.c_str());/// "118.89.144.109");
		sock = ::socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

		char buffer[200];

		int len = sizeof(SOCKADDR);
		if (connect(sock, (SOCKADDR*)&server_ipaddr, len) == 0)
		{
			this->socket = sock;
			cout << "Connect OK" << endl;
		}
		else
		{
			//	exit(0);
			cout << "Connect Error" << endl;
			Sleep(1000);
			exit(0);
		}
	}

public:
	int socket = 0;
};


void ThreadFunc(void*arg)
{
	Client *client = (Client*)arg;
	client->Send("cmd:new:111");
	FrameData *frame = FrameData::CreateEmpty();
	string msg = frame->toJson();
	if (socket == 0)return;

	
	while (true)
	{
		string str = client->Recv();
		if (str.size() > 0)
		{
			cout << str << endl;
			if (str.find("Over") != string::npos)
			{
				delete client;
				exit(0);
				return;
			}else
			client->Send(msg);
		}
	}
}

int main()
{
 
	fstream f;
	f.open("ip.ini", ios::in);
	f >> ip;
	f.close();


	const int MAX_CLIENTS = 1;

	for (int i = 0; i < MAX_CLIENTS; i++)
	{
		Client *client = Client::Create();
		std::thread t(std::bind(ThreadFunc, client));
		t.join();
	}




	while (true)
	{
		Sleep(1000);
	}


 
	return 0;
}



