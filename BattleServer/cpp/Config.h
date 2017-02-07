#pragma  once
#include <string>
class Config
{
public:
	static int MAX_PLAYER_COUNT;//每个房间最大玩家数
	static int SERVER_PORT; // 服务器端口
	static int   MAX_BUFFERS11;	//socket 一次通信 最大缓冲大小
	static int  GAME_OVER_FRAME_COUNT;	//大于多少帧时 游戏结束  5分钟
	static int ROOM_UNABLE_FRAME_COUNT;	//大于多少帧时 房间不允许玩家加入  1分钟

	static bool InitWithFile(std::string file);
};




