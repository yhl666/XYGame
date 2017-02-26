#pragma  once
#include <iostream>
#include <vector>
#include "Defs.h"
#include "ClientEventHandler.h"

class FrameData;
class SocketClient;
class Room;


/**
 * @brief  a player can enter a room annd bind a clientsocket
 * @note this can rebind a clientsocket to support client reconnect
 */
class Player :public ClientEventHandler
{
private:
	SocketClient *socket = nullptr;
	int no = 0;// player numbers
	std::vector<FrameData*> frames;
	int current_fps = 0;
	bool _isConnected = true;
	int current_send_ok_fps = 1;
	Room *room = nullptr;

	bool _isOver = false;
public:

	void AddFrameData(FrameData*data);
	int GetMaxFrameCount();;

	/**
	 * @brief  process this player frame data
	 */
	void IncreateFps(int target_fps);

	void SetNo(int no);
	int GetNo();
	/**
	 * @return frame data bu fps index
	 */
	FrameData *GetFrameDataByFps(int fps);

	void SendJsonData(std::string msg,bool);

	/**
	 * @brief  send a json data to client
	 */
	void SendJsonData(std::string msg);
public:
	/**
	 * @brief bind socket client to this player
	 */
	void BindSocketClient(SocketClient * socket);

	void UnBindSocketClient(SocketClient*socket = nullptr);
	/**
	 * @brief  on player connect
	 */
	virtual void OnConnected(SocketClient *client)override;

	/**
	 * @brief  on player dis connected
	 */
	virtual void OnDisConnected(SocketClient *client)override;

	/**
	 * @brief  on send message to player
	 */
	virtual void OnSendMessage(SocketClient *client, std::string what)override;

	/**
	 * @brief on recv a message from player
	 */
	virtual void OnRecvMessage(SocketClient *client, std::string what)override;

	/**
	 * @brief  on reconnected
	 */
	virtual void OnReConnected(SocketClient *client)override;

	virtual void OnRecvEmptyMessage(SocketClient *client)override;

public:
	/**
	 * @brief  create a player with socketcliet and bind it
	 */
	static Player *Create(SocketClient * socket_bind,Room*);
	~Player();

	bool isConnected(){ return _isConnected; }
	bool GetIsOver() { return _isOver; }

public:
	void Recv();
	void RecvTick();
	void CheckEmptyFrameTick(int fps);

	void SendTick();

	/**
	 * @brief  is this player able close this room
	 */
	bool isReadyForGameOver();
private:
	std::vector<std::string> _sendMsg;
private:
	bool SendJsonDataHelper(int frame);

};


