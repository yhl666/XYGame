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

		bool ret = GetRedis().Set(key, value);
		LuaIntf::LuaRef cb = luaCallback;  // no const
		try
		{
			if (ret)
			{
				cb("ok");
			}
			else
			{
				cb("");
			}
		}
		catch (const LuaIntf::LuaException& e)
		{

		}
	}

	void Get(const string& key, const LuaIntf::LuaRef& luaCallback)
	{

		string ret = GetRedis().Get(key);
		LuaIntf::LuaRef cb = luaCallback;  // no const
		try
		{
			cb(ret);
		}
		catch (const LuaIntf::LuaException& e)
		{

		}

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
			.endModule();
	}

}  // namespace LuaRedis
