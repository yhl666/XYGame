#pragma  once
#include "Defs.h"
#include <vector>
#include "Buffer.h"

class ClientEventHandler;
class SocketClient;

/**
 * @brief  windows socket wrapper  for server
 */
class SocketServer
{

private:

public:
	/**
	 * @brief 	 recv all clients msg
	 */
	/*std::vector<Buffer> & Recv();
	*/
	/**
	 * @brief accept connect loop function
	 */
	SocketClient * AcceptLoop();

	/**
	 * @brief   lazy init
	 */
	bool Init();
private:
	int socket;

	//std::vector<SocketClient*> clients;
//	std::vector<Buffer> _datas;
};

