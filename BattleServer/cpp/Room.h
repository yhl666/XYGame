#pragma  once
#include "Defs.h"

#include <vector>
#include <mutex>
class Player;
class SocketClient;
class LogFile;


/**
 * @brief  a room  which hold up a whole game rome
 */
class Room :public Memory
{
	int disconnect = 0;// the no of player disconnected
	bool isOver = false;
	bool can_destory = false;
public:

	static Room *  Room::Create(int id);

	bool isGameOver();
	bool isAllReady();
	bool CanDestory();

	int GetCurrentFps();
	/**
	 * @return  is room empty 
	 */
	bool isEmptyRoom();

	/**
	 * @brief  add player to room
	 */
	void Add(Player*player);

	Player*GetPlayerByNo(int no);

	int GetPlayerCounts(){ return this->players.size(); }

	/**
	 * @brief remove player to room
	 */
	void Remove(Player * player);

	/**
	 * @brief increase ghame fps and process
	 */
	void IncreaseFps();

	/**
	 * @brief  broadcast custom data such as string or char message to all players
	 */
	void BroadcastCustomData(const std::string & msg,bool isAddToCache=true);


	~Room();

	/**
	 * @brief  recv for a frame
	 * @note this function is conflict  conflict with RecvTick
	 */
	void Recv();

	/**
	 * @brief  recv tick for a frame
	 * loop tick for recv
	 */
	void RecvTick();
	/**
	 * @brief  if this frame not recv data then add a empty frame
	 */
	void CheckEmptyFrameTick();


	void SendTick();

	void ReConnect(SocketClient*client, int no ,int fps);

	bool JoinAble();
	const std::vector<Player*> & GetPlayers() const{ return players; }
	int GetID(){ return id; }

	/**
	* @brief  check which player is disconnect,if disconnect it will not release ,when room destroy ,all player will be destroy
	*/
	void CheckPlayerAlive();
private:
	/**
	 * @brief  set room id (id is a random seed)
	 */
	void SetID(int id);

	Room();
	Room(int id);
	/**
	 * @brief broadcast frame data to all players
	 */
	void BroadcastFrameData();

	/**
	 * @brief  lazy remove player which disconnected after this frame
	 */
	void LazyClearPlayer();



private:
	int max_players;
	int current_fps = 0;
	int id = 0;
	std::vector<Player*> players;

	std::vector<std::string > _brocastDatas;

private: // for multi thread  
	std::recursive_mutex _mutex;
	bool isLock=false;

	public:
	void AddPlayerDynamic(Player*player);
	void Lock();
	void UnLock();
	void LockWithRAII(); 

	LogFile *log;

};



