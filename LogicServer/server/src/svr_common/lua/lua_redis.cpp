//author :caoshanshan
#include "lua_redis.h"

#include "log.h"
#include "redis/redis.h"  // for CRedis
#include "util.h"  // for GetRedis()

#include <LuaIntf/LuaIntf.h>

namespace {

	using string = std::string;
	using LuaRef = LuaIntf::LuaRef;

	using Util::GetRedis;

	void Set(const string& key, const string& value, const LuaIntf::LuaRef& luaCallback)
	{

		LuaIntf::LuaRef *cb = new LuaIntf::LuaRef(luaCallback);  // no const
		GetRedis().Set(key, value, [=](string msg)
		{
			(*cb)(msg);
			delete cb;
		});
	}

	void Get(const string& key, const LuaIntf::LuaRef& luaCallback)
	{
		LuaIntf::LuaRef *cb = new LuaIntf::LuaRef(luaCallback);  // no const
		GetRedis().Get(key, [=](string msg)
		{
			(*cb)(msg);
			delete cb;
		});

	}

	void Execute(const string& cmd, const LuaIntf::LuaRef& luaCallback)
	{
		LuaIntf::LuaRef *cb = new LuaIntf::LuaRef(luaCallback);  // no const
		GetRedis().Execute(cmd, [=](string msg)
		{
			(*cb)(msg);
			delete cb;
		});




	}  // namespace
}
namespace LuaRedis {

	void Bind(lua_State* L)
	{
		assert(L);
		using namespace LuaIntf;
		LuaBinding(L).beginModule("c_redis")
			.addFunction("set", &Set)
			.addFunction("get", &Get)
			.addFunction("exec", &Execute)
			.endModule();
	}

}  // namespace LuaRedis
