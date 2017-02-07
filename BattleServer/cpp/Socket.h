#pragma  once
#include "Defs.h"
#include <string>

//TODO


//this is socket wrapper for server
class Socket
{
public:

	int Send( std::string msg);
	std::string Recv();
	int Accept();
	void Connect(std::string ip, int port);
	void DisConnect();


	void Init(bool isAsync = true);

	void BindSocket(int socket)// this is for clients socket
	{
		this->_inner_socket_handler = socket;
	}
private:

	int _inner_socket_handler = 0;
};


// impl