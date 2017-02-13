// Author: Jin Qing (http://blog.csdn.net/jq0123)
//Author :cao chan chan

#include "redis.h"

#include <adapters/boostasioadapter.h>  // for Adapter (BoostAsioAdapter)
using namespace std;

CRedis::CRedis()
{
}

CRedis::~CRedis()
{
}

bool CRedis::Init(io_service& rIos, const std::string& sHost, uint16_t uPort)
{
	m_pAdapter.reset(new Adapter(rIos));

	this->redis = redisConnect("127.0.0.1", 6379);

	return true;  // XXX
}




bool  CRedis::Set(const  std::string & key, const std::string & value)
{
	redisReply *reply = (redisReply*)redisCommand(this->redis, "SET %s %s", key.c_str(), value.c_str());
	if (reply->str == "OK")return true;
	return false;

}
std::string  CRedis::Get(const  std::string & key)
{
	redisReply* reply = (redisReply*)redisCommand(this->redis, "GET %s", key.c_str());
	std::string str = reply->str;
	freeReplyObject(reply);
	if (str == "<nil>") return "";
	return str;

}
