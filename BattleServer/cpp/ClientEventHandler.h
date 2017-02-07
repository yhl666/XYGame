#pragma  once
#include <iostream>
#include "Defs.h"

class SocketClient;



/**
 * @brief  handl protocol  for network event
 */
class ClientEventHandler :public Memory
{
public:
	virtual void OnConnected(SocketClient *client);
	virtual void OnDisConnected(SocketClient *client);
	virtual void OnSendMessage(SocketClient *client, std::string what);
	virtual void OnRecvMessage(SocketClient *client, std::string what);
	virtual void OnReConnected(SocketClient *client);
	virtual void OnRecvEmptyMessage(SocketClient *client);
};

