#include "Config.h"
#include <fstream>
#include "Utils.h"
using namespace std;

int Config::MAX_PLAYER_COUNT=5;//ÿ��������������
int Config::SERVER_PORT=0; // �������˿�
int Config::MAX_BUFFERS11=4096;	//socket һ��ͨ�� ��󻺳��С
int Config::GAME_OVER_FRAME_COUNT=10240;	//���ڶ���֡ʱ ��Ϸ����  5����
int Config::ROOM_UNABLE_FRAME_COUNT=1024;	//���ڶ���֡ʱ ���䲻������Ҽ���  1����


bool Config::InitWithFile(std::string file)
{
	fstream f;
	f.open(file, ios::in);
	if (f.is_open() == false)return false;
	string line;
	while (f >> line)
	{
		string value;
		f >> value;
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