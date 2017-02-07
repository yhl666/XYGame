
#include "Utils.h"
#include "ClientEventHandler.h"
#include "Buffer.h"
#include <windows.h>
#include "SocketClient.h"
#pragma comment(lib,"ws2_32.lib") 

using namespace std;


void   SocketClient::BindEventHandler(ClientEventHandler*handler)
{
	PTR_PARAM_CHECK_RETURN(handler);

	this->handler = handler;
	this->handler->OnConnected(this);
}

void   SocketClient::UnBindEventHandler(ClientEventHandler*handler)
{
	this->handler = nullptr;
}

SocketClient * SocketClient::Create(int socket)
{
	SocketClient *ret = new SocketClient;
	if (!ret) return nullptr;
	ret->socket = socket;

	return ret;
}

int  SocketClient::Send(const string &  m)
{
	int ret = 0;
	string msg(m);  //std::move(m) ;
	msg += NET_MSG_ENG_LINE;
	ret = ::send(socket, msg.c_str(), msg.size(), 0);

	if (ret == SOCKET_ERROR)
	{
		int code = WSAGetLastError();

		if (code == WSAEWOULDBLOCK)
		{
			//	handler->OnRecvEmptyMessage(this);
			return 0;
		}
		else if (code == WSAENETDOWN)
		{//dis
			if (handler)	handler->OnDisConnected(this);
			return -1;
		}
		else
		{//ok
			//handler->OnRecvMessage(this, ret.GetString());
			if (handler)handler->OnDisConnected(this);
			return ret;
		}

		return -1;
	}
	else if (ret == 0)
	{//send error
		if (handler)handler->OnDisConnected(this);

		return 0;
	}
	else
	{
		if (handler)handler->OnSendMessage(this, msg);
		///cout << "Send:" <<msg << endl;

	}
	return ret;
}

bool SocketClient::SetAsync()
{

	u_long mode = 1;
	if (0 == ioctlsocket(socket, FIONBIO, &mode))//OK
	{
		return true;
	}
	else
	{
		//	exit(0);
		//error
		this->Release();
		return false;
	}
}

Buffer  SocketClient::Recv()
{
	Buffer ret;

	int c = ::recv(this->socket, ret.GetBuffer(), Buffer::MAX_BUFFERSIZE, 0);

	ret.setBufferSize(c);

	if (c == SOCKET_ERROR)
	{
		ret.setBufferSize(0);
		int code = WSAGetLastError();

		if (code == WSAEWOULDBLOCK)
		{
			//	handler->OnRecvEmptyMessage(this);
		}
		else if (code == WSAENETDOWN)
		{//dis
			if (handler)	handler->OnDisConnected(this);
		}
		else
		{//ok
			//handler->OnRecvMessage(this, ret.GetString());
			if (handler)	handler->OnDisConnected(this);
		}

	}
	else if (c == 0)
	{
		//handler->OnRecvEmptyMessage(this);
		if (handler)handler->OnDisConnected(this);
	}
	else
	{
		if (handler)
		{
			//cout << "--------------------------------" << endl;
			string str = ret.GetString();
			str_last += str;

			//handler->OnRecvMessage(this, str);
			//	cout << str << endl;
			int last = 0;
			for (int i = 0; i < str_last.size(); i++)
			{
				char ch = str_last[i];

				if (ch == NET_MSG_ENG_LINE)
				{
					string s = str_last.substr(last, i - last);
					if (s != "")
					{
						//	cout <<__FUNCTION__<<"  0000   " <<s << endl;
						handler->OnRecvMessage(this, s);
					}
					last = i + 1;
				}
			}
			str_last = str_last.substr(last);
			//	cout << "........................." << endl;
		}
	}
	return ret;
}

SocketClient::~SocketClient()
{
	if (this->socket)
		closesocket(this->socket);
}

