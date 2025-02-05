
#include "ServerApp.h"
#include "windows.h"
#include "Player.h"
#include "SocketClient.h"
#include "time.h"
#include "Utils.h"
#include "Room.h"
#include <thread>
#include "FrameData.h"
#include "Config.h"
using namespace  std;


#define  CHECK_LEACKS111

int ServerApp::Run()
{
	while (this->isTerminal == false)
	{
		int  seed = time(0);
		string seeds = Utils::itos(seed);

		Room*room = Room::Create(seed);

		cout << "seed = " << seed << endl;
		for (int i = 0; i < Config::MAX_PLAYER_COUNT; i++)
		{
			auto cli = srv.AcceptLoop();
			if (cli)
			{
				Player*player = Player::Create(cli, room);
				cli->SetAsync();
				room->Add(player);
				player->SetNo(room->GetPlayerCounts());

				cli->Send("cmd:Start:" + seeds + ":" + Utils::itos(player->GetNo()));
			}
		}

		std::thread t(std::bind(ServerApp::RoomThreadFunc, room));

#if  ENABLE_MULTI_THREAD ==FALSE
		t.join();
#else
		t.detach();
#endif

	}

	return 0;
}


void   ServerApp::RoomThreadFunc(void *arg1)
{
	Room *room = (Room*)arg1;


	LARGE_INTEGER nFreq;
	LARGE_INTEGER nLast;
	LARGE_INTEGER nNow;
	LARGE_INTEGER perFrame;

	QueryPerformanceFrequency(&nFreq);
	perFrame.QuadPart = (LONGLONG)(1.0 / 40.0f * nFreq.QuadPart);//25MS


	QueryPerformanceCounter(&nLast);


	while (true)
	{
		QueryPerformanceCounter(&nNow);

		if (nNow.QuadPart - nLast.QuadPart > perFrame.QuadPart)
		{// time to increase fps
			nLast.QuadPart = nNow.QuadPart;
			room->IncreaseFps();
			if (room->CanDestory())
			{
				room->Release();
				return;
			}
		}
		else
		{//loop for all recv send tick
			room->RecvTick();
			room->SendTick();
			std::this_thread::sleep_for(std::chrono::microseconds(1));//Sleep(0);
		}
	}


}

void ServerApp::Startup()
{
	srv.Init();
}


#ifndef  CHECK_LEACKS



void ServerAppDynamic::Startup()
{
	srv.Init();
}

int ServerAppDynamic::Run()
{
	while (this->isTerminal == false)
	{
		int  seed = time(0);
		string seeds = Utils::itos(seed);

		Room*room = Room::Create(seed);

		Utils::log("Create New Room seed = %d", seed);
		int count = 0;

		SocketClient * cli;

		std::thread t(std::bind(ServerAppDynamic::RoomThreadFunc, room));
		t.detach();
		while (true) // loop for accept Player Connecr
		{
			SocketClient * cli = nullptr;

			bool joinAble;
			room->Lock();
			joinAble = room->JoinAble();
			room->UnLock();
			while (joinAble && !cli)
			{
				cli = srv.AcceptLoop();
				//	Sleep(100);
				room->Lock();

				if ((room->GetPlayerCounts() > 0 && room->isEmptyRoom()))
				{
					room->UnLock();
					break;;
				}
				joinAble = room->JoinAble();

				room->UnLock();
			}
			if (!cli)
			{
				Utils::log("Room id=%d UnJoinAble", room->GetID());
				break;
			}

			if (cli)
			{

				string str;
				while (true)
				{
					str = cli->Recv().GetString();

					if (str != "" && str.size() > 0)
					{
						str[str.size() - 1] = '\0';
						break;
					}
				}

				cli->SetAsync();


				room->Lock();
				TranslateDataPack * pack = TranslateDataPack::Decode(str);
				if (pack)
				{
					if (pack->isCustomData)
					{
						string cmd = pack->customs[0];
						if (cmd == "reconnect")
						{
							int fps = Utils::stoi(pack->customs[1]);
							int no = Utils::stoi(pack->customs[2]);
							room->ReConnect(cli, no, fps);
						}
						else if ((cmd == "new"))
						{
							Player*player = Player::Create(cli, room);
							player->SetNo(++count);

							cli->Send("cmd:Start:" + seeds + ":" + Utils::itos(player->GetNo()) + ":" + Utils::itos(room->GetCurrentFps()));
							room->AddPlayerDynamic(player);

							string noo = Utils::itos(player->GetNo());
							cout << "       .." << noo << "." << endl;
							string name = "name";

							room->BroadcastCustomData("cmd:new:" + name + ":" + noo);



							//	room->BroadcastCustomData("cmd:new:" + pack->customs[1] + ":9");// +Utils::itos(player->GetNo()));


							cout << "Room id=" << room->GetID() << " Add Player id=" << player->GetNo() << " name=" << pack->customs[1] << endl;;

						}
						else
						{
							Utils::log("unknow:%s", cmd.c_str());
						}

					}
					else
					{
						//Utils::log("Accept noscene connected %s", str.c_str());
					}
				}
				else
				{
					//Utils::log("Accept noscene connected %s", str.c_str());
					//cli->Release();
				}

				pack->Release();
				room->UnLock();
			}

		}


	}

	return 0;
}



void   ServerAppDynamic::RoomThreadFunc(void *arg1)
{
	PTR_PARAM_CHECK_RETURN(arg1);

	Room *room = (Room*)arg1;

	LARGE_INTEGER nFreq;
	LARGE_INTEGER nLast;
	LARGE_INTEGER nNow;
	LARGE_INTEGER perFrame;

	QueryPerformanceFrequency(&nFreq);
	perFrame.QuadPart = (LONGLONG)(1.0 / 40.0 * nFreq.QuadPart);//25MS


	QueryPerformanceCounter(&nLast);

	while (true)
	{
		QueryPerformanceCounter(&nNow);
		//room->LockWithRAII();

		room->Lock();
		if (nNow.QuadPart - nLast.QuadPart > perFrame.QuadPart)
		{// time to increase fps
			nLast.QuadPart = nNow.QuadPart;

			room->IncreaseFps();
			if (room->CanDestory() || (room->GetPlayerCounts() > 0 && room->isEmptyRoom()))
			{
				Sleep(100);
				room->UnLock();
				room->Release();

				Memory::PrintTrace();
				return;
			}
			//check new player
		}
		else
		{//loop for all recv send tick
			room->RecvTick();
			room->SendTick();
			std::this_thread::sleep_for(std::chrono::microseconds(1));//Sleep(0);
		}

		room->UnLock();
	}
}





#else







//////  single thread for  leacks check



void ServerAppDynamic::Startup()
{
	srv.Init();
}

int ServerAppDynamic::Run()
{
	//while (this->isTerminal == false)
	{
		int  seed = time(0);
		string seeds = Utils::itos(seed);

		Room*room = Room::Create(seed);

		Utils::log("Create New Room seed = %d", seed);
		int count = 0;

		SocketClient * cli;


		//	std::thread t(std::bind(ServerAppDynamic::RoomThreadFunc, room));
		//	t.detach();
		while (true) // loop for accept Player Connecr
		{
			SocketClient * cli = nullptr;

			bool joinAble;
			room->Lock();
			joinAble = room->JoinAble();
			room->UnLock();
			while (joinAble && !cli)
			{
				cli = srv.AcceptLoop();
				//	Sleep(100);
				room->Lock();

				if ((room->GetPlayerCounts() > 0 && room->isEmptyRoom()))
				{
					room->UnLock();
					break;;
				}
				joinAble = room->JoinAble();

				room->UnLock();
				RoomThreadFunc(room);
			}
			if (!cli)
			{
				Utils::log("Room id=%d UnJoinAble", room->GetID());
				break;
			}

			if (cli)
			{

				string str;
				while (true)
				{
					str = cli->Recv().GetString();

					if (str != "" && str.size() > 0)
					{
						str[str.size() - 1] = '\0';
						break;
					}
				}

				cli->SetAsync();


				room->Lock();
				TranslateDataPack * pack = TranslateDataPack::Decode(str);
				if (pack)
				{
					if (pack->isCustomData)
					{
						string cmd = pack->customs[0];
						if (cmd == "reconnect")
						{
							int fps = Utils::stoi(pack->customs[1]);
							int no = Utils::stoi(pack->customs[2]);
							room->ReConnect(cli, no, fps);
						}
						else if ((cmd == "new"))
						{
							Player*player = Player::Create(cli, room);
							player->SetNo(++count);

							cli->Send("cmd:Start:" + seeds + ":" + Utils::itos(player->GetNo()) + ":" + Utils::itos(room->GetCurrentFps()));
							room->AddPlayerDynamic(player);
							room->BroadcastCustomData("cmd:new:" + pack->customs[1] + ":" + Utils::itos(player->GetNo()));
							cout << "Room id=" << room->GetID() << " Add Player id=" << player->GetNo() << " name=" << pack->customs[1] << endl;;

						}
						else
						{
							//Utils::log("unknow:%s1", cmd.c_str());
						}

					}
					else
					{
						//Utils::log("Accept noscene connected %s", str.c_str());
					}
				}
				else
				{
					//Utils::log("Accept noscene connected %s", str.c_str());
					//cli->Release();
				}

				pack->Release();
				room->UnLock();
			}

		}

		while (true)
		{
			RoomThreadFunc(room);

		}


		Sleep(1111111);
		return 0;
	}
}

LARGE_INTEGER nFreq;
LARGE_INTEGER nLast;
LARGE_INTEGER nNow;
LARGE_INTEGER perFrame;


void   ServerAppDynamic::RoomThreadFunc(void *arg1)
{

	Room *room = (Room*)arg1;

	QueryPerformanceFrequency(&nFreq);
	perFrame.QuadPart = (LONGLONG)(1.0 / 40.0 * nFreq.QuadPart);//25MS



	///	while (true)
	{
		QueryPerformanceCounter(&nNow);
		//room->LockWithRAII();

		room->Lock();
		if (true || nNow.QuadPart - nLast.QuadPart > perFrame.QuadPart)
		{// time to increase fps
			nLast.QuadPart = nNow.QuadPart;

			room->IncreaseFps();
			if (room->CanDestory() || (room->GetPlayerCounts() > 0 && room->isEmptyRoom()))
			{
				Sleep(100);
				room->UnLock();
				room->Release();


				Memory::PrintTrace();

				Memory::Clear();
				_CrtDumpMemoryLeaks();
				system("pause");
				exit(0);


			}
			//check new player
		}
		else
		{//loop for all recv send tick
			room->RecvTick();
			room->SendTick();
			std::this_thread::sleep_for(std::chrono::microseconds(1));//Sleep(0);
		}

		room->UnLock();
	}

	QueryPerformanceCounter(&nLast);
	return;
}


#endif














////---------------------------------------PVP 战斗服务器

void ServerAppBattlePVP::Startup()
{
	srv.Init();
	Utils::log("Startup Battle PVP Server OK");
}

int ServerAppBattlePVP::Run()
{
	while (this->isTerminal == false)
	{

		SocketClient * cli = nullptr;


		while (cli == nullptr)//接受连接请求
		{
			cli = srv.AcceptLoop();
			Sleep(1);
		}

		if (cli)
		{
			string str;//接受第一条消息
			while (true)
			{
				str = cli->Recv().GetString();

				if (str != "" && str.size() > 0)
				{
					str[str.size() - 1] = '\0';
					break;
				}
			}

			///
			TranslateDataPack * pack = TranslateDataPack::Decode(str);
			if (pack)
			{
				if (pack->isCustomData)
				{



					string cmd = pack->customs[0];

					if (cmd == "new_pvp_friend")
					{
						string user_no = pack->customs[1];
						string room_no = pack->customs[2];
						string room_max = pack->customs[3];


						Room *room = nullptr;
						room = rooms[room_no];
						if (room == nullptr)
						{
							room = Room::Create(Utils::stoi(room_no));
							rooms[room_no] = room;
							Utils::log("Create New Room id = %d", room->GetID());
						}
						room->Lock();

						Player*player = Player::Create(cli, room);
						player->SetNo(Utils::stoi(user_no));


						room->AddPlayerDynamic(player);

						cli->Send("cmd:Start:0:" + Utils::itos(player->GetNo()) + ":" + Utils::itos(room->GetCurrentFps()));
						Sleep(1000);
						Utils::log(" Room id = %d  add Player %d", room->GetID(), player->GetNo());
						if (room->GetPlayerCounts() == Utils::stoi(room_max))
						{
							const vector<Player*> & players = room->GetPlayers();

							for (int i = 0; i < players.size(); i++)
							{
								Player*player = players[i];
								room->BroadcastCustomData("cmd:new_pvp_friend:" + Utils::itos(player->GetNo()));
							}
							std::thread t(std::bind(ServerAppDynamic::RoomThreadFunc, room));
							t.detach();

						}
						else
						{

						}

						room->UnLock();



					}
					else
					{
						Utils::log("unknow:%s1", cmd.c_str());
					}




				}
			}
			pack->Release();

		}



































		/*int  seed = time(0);
		string seeds = Utils::itos(seed);

		Room*room = Room::Create(seed);

		Utils::log("Create New Room seed = %d", seed);
		int count = 0;

		SocketClient * cli;

		///std::thread t(std::bind(ServerAppDynamic::RoomThreadFunc, room));
		//	t.detach();
		while (true) // loop for accept Player Connecr
		{
		SocketClient * cli = nullptr;

		bool joinAble;
		room->Lock();
		joinAble = room->JoinAble();
		room->UnLock();
		while (joinAble && !cli)
		{
		cli = srv.AcceptLoop();
		//	Sleep(100);
		room->Lock();

		if ((room->GetPlayerCounts() > 0 && room->isEmptyRoom()))
		{
		room->UnLock();
		break;;
		}
		joinAble = room->JoinAble();

		room->UnLock();
		}
		if (!cli)
		{
		Utils::log("Room id=%d UnJoinAble", room->GetID());
		break;
		}

		if (cli)
		{

		string str;
		while (true)
		{
		str = cli->Recv().GetString();

		if (str != "" && str.size() > 0)
		{
		str[str.size() - 1] = '\0';
		break;
		}
		}

		cli->SetAsync();


		room->Lock();
		TranslateDataPack * pack = TranslateDataPack::Decode(str);
		if (pack)
		{
		if (pack->isCustomData)
		{
		string cmd = pack->customs[0];
		if (cmd == "reconnect")
		{
		int fps = Utils::stoi(pack->customs[1]);
		int no = Utils::stoi(pack->customs[2]);
		room->ReConnect(cli, no, fps);
		}
		else if ((cmd == "new"))
		{
		Player*player = Player::Create(cli, room);
		player->SetNo(++count);

		cli->Send("cmd:Start:" + seeds + ":" + Utils::itos(player->GetNo()) + ":" + Utils::itos(room->GetCurrentFps()));
		room->AddPlayerDynamic(player);

		string noo = Utils::itos(player->GetNo());
		cout << "       .." << noo << "." << endl;
		string name = "name";

		room->BroadcastCustomData("cmd:new:" + name + ":" + noo);



		//	room->BroadcastCustomData("cmd:new:" + pack->customs[1] + ":9");// +Utils::itos(player->GetNo()));


		cout << "Room id=" << room->GetID() << " Add Player id=" << player->GetNo() << " name=" << pack->customs[1] << endl;;

		}
		else
		{
		//Utils::log("unknow:%s1", cmd.c_str());
		}

		}
		else
		{
		//Utils::log("Accept noscene connected %s", str.c_str());
		}
		}
		else
		{
		//Utils::log("Accept noscene connected %s", str.c_str());
		//cli->Release();
		}

		pack->Release();
		room->UnLock();
		}

		}*/


	}

	return 0;
}



void   ServerAppBattlePVP::RoomThreadFunc(void *arg1)
{
	PTR_PARAM_CHECK_RETURN(arg1);

	Room *room = (Room*)arg1;

	LARGE_INTEGER nFreq;
	LARGE_INTEGER nLast;
	LARGE_INTEGER nNow;
	LARGE_INTEGER perFrame;

	QueryPerformanceFrequency(&nFreq);
	perFrame.QuadPart = (LONGLONG)(1.0 / 40.0 * nFreq.QuadPart);//25MS


	QueryPerformanceCounter(&nLast);

	while (true)
	{
		QueryPerformanceCounter(&nNow);
		//room->LockWithRAII();

		room->Lock();
		if (nNow.QuadPart - nLast.QuadPart > perFrame.QuadPart)
		{// time to increase fps
			nLast.QuadPart = nNow.QuadPart;

			room->IncreaseFps();
			if (room->CanDestory() || (room->GetPlayerCounts() > 0 && room->isEmptyRoom()))
			{
				Sleep(100);
				room->UnLock();
				room->Release();

				Memory::PrintTrace();
				return;
			}
			//check new player
		}
		else
		{//loop for all recv send tick
			room->RecvTick();
			room->SendTick();
			std::this_thread::sleep_for(std::chrono::microseconds(1));//Sleep(0);
		}

		room->UnLock();
	}
}




////---------------------------------------PVE 战斗服务器

void ServerAppBattlePVE::Startup()
{
	srv.Init();
	Utils::log("Startup Battle PVE Server OK");
}

int ServerAppBattlePVE::Run()
{
	int i = 0;
	while (this->isTerminal == false)
	{

		SocketClient * cli = nullptr;
		Room *room = nullptr;
		room = Room::Create(++i);

		Utils::log("Create New Room id = %d", room->GetID());
		while (cli == nullptr)//接受连接请求
		{
			cli = srv.AcceptLoop();
			Sleep(1);
		}

		if (cli)
		{
			string str;//接受第一条消息
			while (true)
			{
				str = cli->Recv().GetString();

				if (str != "" && str.size() > 0)
				{
					str[str.size() - 1] = '\0';
					break;
				}
			}

			///
			TranslateDataPack * pack = TranslateDataPack::Decode(str);
			if (pack)
			{
				if (pack->isCustomData)
				{



					string cmd = pack->customs[0];

					if (cmd == "new_pvp_friend")
					{
						string user_no = pack->customs[1];
						string room_no = pack->customs[2];
						string room_max = pack->customs[3];


						if (room == nullptr)
						{

						}
						room->Lock();

						Player*player = Player::Create(cli, room);
						player->SetNo(Utils::stoi(user_no));


						room->AddPlayerDynamic(player);

						cli->Send("cmd:Start:0:" + Utils::itos(player->GetNo()) + ":" + Utils::itos(room->GetCurrentFps()));
						Sleep(1000);
						Utils::log(" Room id = %d  add Player %d", room->GetID(), player->GetNo());
						if (room->GetPlayerCounts() == 1)
						{
							const vector<Player*> & players = room->GetPlayers();

							for (int i = 0; i < players.size(); i++)
							{
								Player*player = players[i];
								room->BroadcastCustomData("cmd:new_pvp_friend:" + Utils::itos(player->GetNo()));
							}
							std::thread t(std::bind(ServerAppBattlePVE::RoomThreadFunc, room));
							t.detach();

						}
						else
						{

						}

						room->UnLock();



					}
					else
					{
						Utils::log("unknow:%s1", cmd.c_str());
					}




				}
			}
			pack->Release();

		}

























	}

	return 0;
}



void   ServerAppBattlePVE::RoomThreadFunc(void *arg1)
{
	PTR_PARAM_CHECK_RETURN(arg1);

	Room *room = (Room*)arg1;

	LARGE_INTEGER nFreq;
	LARGE_INTEGER nLast;
	LARGE_INTEGER nNow;
	LARGE_INTEGER perFrame;

	QueryPerformanceFrequency(&nFreq);
	perFrame.QuadPart = (LONGLONG)(1.0 / Config::GAME_FPS * nFreq.QuadPart);//25MS


	QueryPerformanceCounter(&nLast);


	QueryPerformanceCounter(&nLast);
/*	room->Lock();
	room->BroadcastCustomData("cmd:Ready");
	while (true)
	{//等待所有客户端准备完毕 再开始
		Sleep(100);
		if (room->isAllReady())
		{
			break;
		}
	}
	room->UnLock();
	cout << "Ready" << endl;
	*/

	while (true)
	{
		QueryPerformanceCounter(&nNow);
		//room->LockWithRAII();

		room->Lock();
		if (nNow.QuadPart - nLast.QuadPart > perFrame.QuadPart)
		{// time to increase fps
			nLast.QuadPart = nNow.QuadPart;

			room->IncreaseFps();
			if (room->CanDestory() || (room->GetPlayerCounts() > 0 && room->isEmptyRoom()))
			{
				Sleep(100);
				room->UnLock();
				room->Release();

				Memory::PrintTrace();
				return;
			}
			//check new player
		}
		else
		{//loop for all recv send tick
			room->RecvTick();
			room->SendTick();
			std::this_thread::sleep_for(std::chrono::microseconds(1));//Sleep(0);
		}

		room->UnLock();
	}
}






////--------------------------------------- 战斗服务器

void ServerAppBattle::Startup()
{
	srv.Init();
	Utils::log("Startup Battle Server OK");
}

int ServerAppBattle::Run()
{
	while (this->isTerminal == false)
	{

		SocketClient * cli = nullptr;


		while (cli == nullptr)//接受连接请求
		{
			cli = srv.AcceptLoop();
			Sleep(1);
		}

		if (cli)
		{
			string str;//接受第一条消息
			while (true)
			{
				str = cli->Recv().GetString();

				if (str != "" && str.size() > 0)
				{
					str[str.size() - 1] = '\0';
					break;
				}
			}

			///
			TranslateDataPack * pack = TranslateDataPack::Decode(str);
			if (pack)
			{
				if (pack->isCustomData)
				{



					string cmd = pack->customs[0];

					if (cmd == "new_pvp_friend")
					{
						string user_no = pack->customs[1];
						string room_no = pack->customs[2];
						string room_max = pack->customs[3];


						Room *room = nullptr;
						room = rooms[room_no];
						if (room == nullptr)
						{
							room = Room::Create(Utils::stoi(room_no));
							rooms[room_no] = room;
							Utils::log("Create New Room id = %d", room->GetID());
						}
						room->Lock();

						Player*player = Player::Create(cli, room);
						player->SetNo(Utils::stoi(user_no));


						room->AddPlayerDynamic(player);

						cli->Send("cmd:Start:0:" + Utils::itos(player->GetNo()) + ":" + Utils::itos(room->GetCurrentFps()));
						Sleep(1000);
						Utils::log(" Room id = %d  add Player %d", room->GetID(), player->GetNo());
						if (room->GetPlayerCounts() == Utils::stoi(room_max))
						{
							const vector<Player*> & players = room->GetPlayers();

							for (int i = 0; i < players.size(); i++)
							{
								Player*player = players[i];
								room->BroadcastCustomData("cmd:new_pvp_friend:" + Utils::itos(player->GetNo()));
							}
							std::thread t(std::bind(ServerAppBattle::RoomThreadFunc, room));
							t.detach();

						}
						else
						{

						}

						room->UnLock();

					}
					else
					{
						Utils::log("unknow:%s1", cmd.c_str());
					}
				}
			}
			pack->Release();

		}

	}

	return 0;
}



void   ServerAppBattle::RoomThreadFunc(void *arg1)
{
	PTR_PARAM_CHECK_RETURN(arg1);

	Room *room = (Room*)arg1;

	LARGE_INTEGER nFreq;
	LARGE_INTEGER nLast;
	LARGE_INTEGER nNow;
	LARGE_INTEGER perFrame;

	QueryPerformanceFrequency(&nFreq);
	perFrame.QuadPart = (LONGLONG)(1.0 / Config::GAME_FPS  * nFreq.QuadPart);//25MS


	Utils::log("Ready For Waitting all Clients");
	room->Lock();
	room->BroadcastCustomData("cmd:Ready:1:2");
	while (true)
	{//等待所有客户端准备完毕 再开始
		Sleep(200);
		room->RecvTick();
		room->CheckPlayerAlive();
		if (room->CanDestory() || (room->GetPlayerCounts() > 0 && room->isEmptyRoom()))
		{
			Sleep(100);
			room->UnLock();
			room->Release();
			//	Memory::PrintTrace();
			return;
		}

		if (room->isAllReady())
		{
			break;
		}
	}
	room->UnLock();
	Utils::log("Start to Run Game");
	QueryPerformanceCounter(&nLast);
	while (true)
	{
		QueryPerformanceCounter(&nNow);
		//room->LockWithRAII();

		room->Lock();
		if (nNow.QuadPart - nLast.QuadPart > perFrame.QuadPart)
		{// time to increase fps
			nLast.QuadPart = nNow.QuadPart;

			room->IncreaseFps();
			if (room->CanDestory() || (room->GetPlayerCounts() > 0 && room->isEmptyRoom()))
			{
				Sleep(100);
				room->UnLock();
				room->Release();

				//	Memory::PrintTrace();
				return;
			}
			//check new player
		}
		else
		{//loop for all recv send tick
			room->RecvTick();
			room->SendTick();
			std::this_thread::sleep_for(std::chrono::microseconds(1));//Sleep(0);
		}

		room->UnLock();
	}
}


