#ifndef __BASE_SERVER_HEAD__CLIENTSERVER
#define __BASE_SERVER_HEAD__CLIENTSERVER


#include <windows.h>
#include <string>
#include <thread>
#include <mutex>
#include <queue>
#include <memory>
#include <unordered_map>

#include "world.h"

#include "timer_queue.h"
struct lua_State;
 
using namespace std;
class XYGameLog
{//记录玩家关键操作，可用于GM系统分析 处理
public:

	static XYGameLog *GetInstance()
	{
		static XYGameLog ins;
		return &ins;//thread safe with c++11
	}
private:

public:
//记录一条日志接口
	void Log(const std::string &file,const std::string & info);
	void CreateDir(const std::string &file);

	void BindLua(lua_State *l);

private:
	XYGameLog();
	//定期持久化到文件
	void SyncToFile();
	std::unordered_map <std::string,std::string> hash;
};
















#endif		// __BASE_SERVER_HEAD__
