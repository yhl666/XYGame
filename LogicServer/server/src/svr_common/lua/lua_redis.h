#ifndef SVR_COMMON_LUA_LUA_REDIS_H
#define SVR_COMMON_LUA_LUA_REDIS_H

struct lua_State;

namespace LuaRedis
{
void Bind(lua_State* L);
}

#endif  // SVR_COMMON_LUA_LUA_REDIS_H
