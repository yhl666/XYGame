// Author: Jin Qing (http://blog.csdn.net/jq0123)
//Author :cao chan chan

#include "redis.h"

#include <adapters/boostasioadapter.h>  // for Adapter (BoostAsioAdapter)
using namespace std;

using Callback = std::function<void(string msg)>;

void getCallback(redisAsyncContext *c, void *r, void *privdata) {

	redisReply *reply = (redisReply*)r;

	if (reply != NULL && reply->str != nullptr)
	{
		(*(std::function<void(string msg)>*)privdata)(reply->str);
	}
	else
	{
		(*(std::function<void(string msg)>*)privdata)("");
	}

	delete privdata;
}


void setCallback(redisAsyncContext *c, void *r, void *privdata) {
	redisReply *reply = (redisReply*)r;

	if (reply != NULL && reply->str != nullptr)
	{
		(*(std::function<void(string msg)>*)privdata)(reply->str);
	}
	else
	{
		(*(std::function<void(string msg)>*)privdata)("");
	}

	delete privdata;
}



void execCallback(redisAsyncContext *c, void *r, void *privdata) {
	redisReply *reply = (redisReply*)r;

	printf(" %lld ", reply->integer);

	if (reply != NULL)
	{
		string param = "ret:";

		if (reply->str != nullptr)
		{ // is string

			string str = reply->str;

			if (str.find("ERR") == string::npos)
			{//ok
				param = param + "ok,msg:" + str;
			}
			else
			{
				param = param + "error,msg:" + str;
			}
		}
		else if (reply->integer != 0)
		{// is number
			char what[100];
			sprintf(what, "%lld", reply->integer);

			param = param + "ok,msg:" + what;
		}
		else
		{
			param = param + "error,msg:unknow error";
		}


		(*(std::function<void(string msg)>*)privdata)(param + ",");

	}
	else
	{
		(*(std::function<void(string msg)>*)privdata)("ret:error,msg:redis no reply error,");
	}

	delete privdata;
}



void connectCallback(const redisAsyncContext *c, int status) {
	if (status != REDIS_OK) {
		printf("Error: %s\n", c->errstr);
		return;
	}
	printf("Connected...\n");
}

void disconnectCallback(const redisAsyncContext *c, int status) {
	if (status != REDIS_OK) {
		printf("Error: %s\n", c->errstr);
		return;
	}
	printf("Disconnected...\n");
}






CRedis::CRedis()
{
}

CRedis::~CRedis()
{
}





bool CRedis::Init(io_service& rIos, const std::string& sHost, uint16_t uPort)
{
	//m_pAdapter.reset(new Adapter(rIos));

	//this->redis = redisConnect("127.0.0.1", 6379);


	while (true)
	{
		Sleep(100);
		ctx = redisAsyncConnect("127.0.0.1", 6379);
		if (ctx->err) {
			/* Let *ac leak for now... */
			printf(" Connected Redis Error  %s\n", ctx->errstr);
			delete ctx;
			continue;;
		}

		break;

	}


	/*jack in*/
	client = new redisBoostClient(rIos, ctx);
	redisAsyncSetConnectCallback(ctx, connectCallback);
	redisAsyncSetDisconnectCallback(ctx, disconnectCallback);

	/*loop forever, ever, even if there is no work queued*/

	boost::asio::io_service::work    forever(rIos);

	return true;  // XXX
}




void  CRedis::Set(const  std::string & key, const std::string & value, std::function<void(string msg)> cb)
{
	Callback *cbb = new Callback([=](string mg)
	{
		cb(mg);
	});
	string k = "set " + key + " " + value;
	redisAsyncCommand(ctx, setCallback, cbb, (char*)k.c_str());

	/*
	redisReply *reply = (redisReply*)redisCommand(this->redis, "SET %s %s", key.c_str(), value.c_str());
	if (reply->str == "OK")return true;
	return false;
	*/



}

void CRedis::Get(const  std::string & key, std::function<void(string msg)> cb)
{

	Callback *cbb = new Callback([=](string mg)
	{
		cb(mg);
	});
	string k = "get " + key;
	redisAsyncCommand(ctx, getCallback, cbb, (char*)k.c_str());


	/*
	redisReply* reply = (redisReply*)redisCommand(this->redis, "GET %s", key.c_str());
	std::string str = reply->str;
	freeReplyObject(reply);
	if (str == "<nil>") return "";
	return str;
	*/

}




void CRedis::Execute(const  std::string & cmd, std::function<void(string msg)> cb)
{
	Callback *cbb = new Callback([=](string mg)
	{
		cb(mg);
	});
	redisAsyncCommand(ctx, execCallback, cbb, (char*)cmd.c_str());

}
