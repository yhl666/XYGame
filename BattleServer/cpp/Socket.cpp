
#include "Socket.h"
#include "windows.h"




//inner class
class SocketImpl_Win32
{
public:
	void Send(int socket, std::string msg);
	std::string Recv(int socket);

};


//inner class
class SocketImpl_Linux
{

};

