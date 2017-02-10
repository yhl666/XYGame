
#include "Room.h"
#include "Player.h"
#include "Utils.h"
#include "FrameData.h"
#include "Config.h"
#include <string>
#include <fstream>
#include "SocketClient.h"
using namespace  std;




Room *  Room::Create(int id)
{
	Room*ret = new Room(id);
	if (ret)return ret;
	return  nullptr;
}

Room::Room()
{


}

Room::Room(int id)
{
	this->SetID(id);
	//this->log = new LogFile(Utils::itos(id));
}

bool Room::isEmptyRoom()
{
	if (disconnect == players.size())return true;
	return false;
}

void Room::Add(Player*player)
{
	this->players.push_back(player);
}


Player* Room::GetPlayerByNo(int no)
{
	for (int i = 0; i < players.size(); i++)
	{
		if (players[i]->GetNo() == no)
		{
			return players[i];
		}
	}

	return nullptr;
}

int  Room::GetCurrentFps()
{
	return this->current_fps;
}


void  Room::SetID(int id)
{
	this->id = id;
}

void Room::Remove(Player * player)
{
	//auto iter = std::find(players.begin(), players.end(), player);
	//	players.erase(iter);

}


void Room:: BroadcastCustomData(const std::string & msg, bool isAddToCache)
{
	if (players.size() <= 0)return;
	string str =  msg;
	str += NET_MSG_ENG_LINE;

	for (auto p : players)
	{
		p->SendJsonData(str);
	}
	if (isAddToCache)
	{
		this->_brocastDatas.push_back(str);
	}
	Utils::log("Broadcast:%s", msg.c_str());
}

void Room::BroadcastFrameData()
{
	string jsondata;
	int i = 0;

	for (Player *p : players)
	{

		FrameData *f = p->GetFrameDataByFps(current_fps);
		jsondata += f->toJson();
		jsondata += "+";
	
		i++;
	}
	jsondata += NET_MSG_ENG_LINE;

	for (auto p : players)
	{
		p->SendJsonData(jsondata);
	}
	//Utils::log("Broadcast:%s", jsondata.c_str());
///	Utils::log("%s", jsondata.c_str());


	//this->log->log("Broadcast:%s", jsondata.c_str());
	//this->log->log("%s", jsondata.c_str());



	this->_brocastDatas.push_back(jsondata);

}



void Room::LazyClearPlayer()
{
	for (auto it = players.begin(); it != players.end();)
	{

		if ((*it)->isConnected() == false)
		{
			(*it)->Release();
			it = players.erase(it);
		}
		else
		{
			++it;
		}

	}

}

void Room::CheckPlayerAlive()
{
	int count = 0;
	for (auto it = players.begin(); it != players.end(); ++it)
	{

		if ((*it)->isConnected() == false)
		{
			++count;
		}
	}
	this->disconnect = count;
}


void Room::IncreaseFps()
{
	if (players.size() <= 0)return;
	if (isOver)
	{
		//check all client recv
		for (Player * p : players)
		{
			if (p->isReadyForGameOver() == false)return;
		}
		this->can_destory = true;
		return;
	}


	if (this->isGameOver())
	{
	
		this->BroadcastCustomData("cmd:Over");
		this->isOver = true;
		return;
	}



	this->CheckEmptyFrameTick();
	// process
	current_fps++;

	for (Player * p : players)
	{
		p->IncreateFps(current_fps);
	}

	this->BroadcastFrameData();

	//this->LazyClearPlayer();
	this->CheckPlayerAlive();

}

void  Room::CheckEmptyFrameTick()
{
	for (Player * p : players)
	{
		p->CheckEmptyFrameTick(this->current_fps);
	}
}


void  Room::Recv()
{
	for (auto p : players)
	{
		p->Recv();
	}
}


void Room::RecvTick()
{

	for (Player * p : players)
	{
		p->RecvTick();
	}
}


void Room::SendTick()
{

	for (Player * p : players)
	{
		p->SendTick();
	}
}



Room::~Room()
{
	for (auto p : players)
	{
		p->Release();
	}
	//this->log->Release();
	Utils::log("~Room id=%d  Destory", id);
}


bool Room::isGameOver()
{
	return current_fps >  Config::GAME_OVER_FRAME_COUNT;
}

bool Room::CanDestory()
{
	return this->can_destory;// || this->isEmptyRoom();
};


void Room::AddPlayerDynamic(Player*player)
{
	PTR_PARAM_CHECK_RETURN(player);

	//this->LockWithRAII();

	//	std::lock_guard<std::mutex> locker(_mutex);

	std::string multiData;
	//std::fstream file;
	//file.open("1.txt", ios::out);

	/*this->players.push_back(player);
	for (int i = 0;i<current_fps;i++)
	{
	player->OnRecvEmptyMessage(0);


	player->IncreateFps(i+1);



	player->SendJsonData(_brocastDatas[i]);
	file <<_brocastDatas[i]<<endl;
	multiData += _brocastDatas[i] ;
	}*/
	if (_brocastDatas.size() > 0)
	{

		for (int i = 0; i < current_fps; i++)
		{//clac fps
			player->OnRecvEmptyMessage(0);
			player->IncreateFps(i + 1);
		}
		for (int i = 0; i < _brocastDatas.size(); i++)
		{// add brocast data
			//file <<_brocastDatas[i]<<endl;
			multiData += _brocastDatas[i];
		}
		multiData[multiData.size() - 1] = '\0';
		player->SendJsonData(multiData);

		//	file << multiData << endl;
	}
	this->Add(player);

};
bool Room::JoinAble()
{
	if (players.size() >= Config::MAX_PLAYER_COUNT)
	{
		return false;
	}
	if (current_fps >  Config::ROOM_UNABLE_FRAME_COUNT)
	{
		return false;

	}
	return true;;
}


void Room::ReConnect(SocketClient*client, int no, int fps)
{

	PTR_PARAM_CHECK_RETURN(client);

	//	std::lock_guard<std::mutex> locker(_mutex);

	Player *player = this->GetPlayerByNo(no);
	if (!player)
	{
		client->Release();
		return;
	}
	//	client->SetAsync();
	//this->LockWithRAII();
	client->Send("cmd:ReConnect");


	std::string multiData;

	int range = current_fps;
	if (range >= _brocastDatas.size())
	{
		range = _brocastDatas.size();
	}
	for (int i = fps - 1; i < range; i++)
	{
		//	player->OnRecvEmptyMessage(0);


		//	player->IncreateFps(i + 1);

		multiData += _brocastDatas[i];
	}
	//	multiData.push_back( '\0');
	player->BindSocketClient(client);

	player->SendJsonData(multiData);

	cout << multiData << endl;

}

void  Room::Lock()
{
	//if (isLock)ZT_ASSERT(false, "has been locked");
	this->_mutex.lock();
	this->isLock = true;
}

void Room::UnLock()
{
	//if (!isLock)ZT_ASSERT(false, "has not been locked");
	this->_mutex.unlock();
	this->isLock = false;
}

void Room::LockWithRAII()
{

	//	if (isLock)ZT_ASSERT(false, "has been locked");
	//isLock = true;

	///std::lock_guard<std::mutex> locker(this->_mutex);

	//isLock = false;

}

