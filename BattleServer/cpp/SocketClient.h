#pragma  once
#include "Defs.h"

#include "Buffer.h"


/**
* @brief  windows socket wrapper  for client
* @note
*/
class SocketClient:public Memory
{
	ClientEventHandler *handler = nullptr;

public:

	/**
	 * @brief  create a socket wrapper with windows socket HANDLE
	 */
	static SocketClient *Create(int socket = 0);

	/**
	* @brief  the interface to send message to cliet
	*/
	int Send(const std::string & msg);

	/**
	 * @brief  the interface to recv message from cliet
	 */
	Buffer Recv();

	/**
	* @brief bind a event handler to this class
	*/
	void BindEventHandler(ClientEventHandler*handler);

	/**
	* @brief un bind a event handler to this class
	*/
	void UnBindEventHandler(ClientEventHandler*handler=nullptr);


	/**
	 * @brief  close bind windows socket
	 */
	~SocketClient();


	bool  SetAsync();
private:
	SocketClient(){};
	int socket = 0;
	std::string str_last;
};


