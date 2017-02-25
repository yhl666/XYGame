#pragma  once
#include "Defs.h"
#include <unordered_map>
#include "SocketServer.h"
#include "ClientEventHandler.h"
class Player;
class Room;
/**
 * @brief  impl class for main function
 */
class ServerApp :public ClientEventHandler
{
private:
	 
public:

	
	/**
	* @brief start up and init  app
	*/
	void Startup();

	int Run();

	static void RoomThreadFunc(void *);
private:
	int fps;
	SocketServer srv;
	bool isTerminal = false;
};



/**
* @brief  impl class for main function
*/
class ServerAppDynamic :public ClientEventHandler
{
private:

public:


	/**
	* @brief start up and init  app
	*/
	void Startup();

	int Run();

	static void RoomThreadFunc(void *);
private:
	int fps;
	SocketServer srv;
	bool isTerminal = false;
};




/**
* @brief  impl class for main function
*/
class ServerAppBattlePVP :public ClientEventHandler
{
private:

public:


	/**
	* @brief start up and init  app
	*/
	void Startup();

	int Run();

	static void RoomThreadFunc(void *);
private:
	int fps;
	SocketServer srv;
	bool isTerminal = false;
	std::unordered_map<std::string, Room*> rooms;
};



/**
* @brief  impl class for main function
*/
class ServerAppBattlePVE :public ClientEventHandler
{
private:

public:


	/**
	* @brief start up and init  app
	*/
	void Startup();

	int Run();

	static void RoomThreadFunc(void *);
private:
	int fps;
	SocketServer srv;
	bool isTerminal = false;
 
};
