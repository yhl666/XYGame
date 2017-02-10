#include "Config.h"
#include <fstream>
#include "Utils.h"
using namespace std;

int Config::MAX_PLAYER_COUNT=5;//每个房间最大玩家数
int Config::SERVER_PORT=0; // 服务器端口
int Config::MAX_BUFFERS11=4096;	//socket 一次通信 最大缓冲大小
int Config::GAME_OVER_FRAME_COUNT=10;	//大于多少帧时 游戏结束  5分钟
int Config::ROOM_UNABLE_FRAME_COUNT=1;	//大于多少帧时 房间不允许玩家加入  1分钟


bool Config::InitWithFile(std::string file)
{
	fstream f;
	f.open(file, ios::in);
	if (f.is_open() == false)return false;
	string line;
	char buf[256];

	while (!f.eof() && f.getline(buf, sizeof (char)* 256, 0x0D0A))
	{	//window文件格式换行是0x0D0A,UNIX/LINUX是0x0A
		line = buf;
		string value;
	
		if (!f.eof() && f.getline(buf, sizeof (char)* 256, 0x0A))
		{
			value = buf;
		}
		else
		{
			continue;
		}



		line = line.substr(1, line.find("]") - 1);  //skip remark

		// parse config
		if (line == "MAX_PLAYER_COUNT")
		{
			MAX_PLAYER_COUNT = Utils::stoi(value);
		}
		else if (line == "SERVER_PORT")
		{
			SERVER_PORT = Utils::stoi(value);
		}
		else if (line == "MAX_BUFFERS")
		{
			MAX_BUFFERS11 = Utils::stoi(value);
		}
		else if (line == "GAME_OVER_FRAME_COUNT")
		{
			GAME_OVER_FRAME_COUNT = Utils::stoi(value);
		}
		else if (line == "ROOM_UNABLE_FRAME_COUNT")
		{
			ROOM_UNABLE_FRAME_COUNT = Utils::stoi(value);
		}

	}

	f.close();
	return true;
}