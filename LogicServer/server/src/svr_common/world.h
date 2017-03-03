#ifndef __CWorld__BASE__HEAD__
#define __CWorld__BASE__HEAD__

#include "server_type.h"

#include <memory>  // for unique_ptr<>
#include <set>

#include <LuaIntf/LuaIntf.h>  // for LuaContext

class CAsioServer;
class CCfgReader;
class CRedis;
class CServerMailBox;
class CServerMailBoxMgr;

class CWorld
{
public:
	CWorld();
	virtual ~CWorld();

public:
	virtual bool Init(const std::string& sCfgFilePath, CAsioServer& rSvr);

public:
	uint16_t GetServerId();
	CRedis& CWorld::GetRedis() const
	{
		if (m_pRedis)
			return *m_pRedis;
		throw std::string("Uninitialized.");
	}
public:
	CAsioServer* GetServer();
	CServerMailBox* GetServerMailbox(uint16_t nServerId);

public:
	lua_State* GetLuaState()
	{
		return m_luaCtx.state();
	}

	CCfgReader* GetCfgReader() const
	{
		return m_pCfg.get();
	}

	CServerMailBoxMgr& GetSvrMbMgr()
	{
		return *m_pSvrMbMgr;
	}

	void BlockingRun();

private:
	// 获取配置的服务器绑定端口
	uint16_t GetServerPort();

protected:
	void InitMailboxMgr();
	bool DoLuaInitFile(const std::string& sLuaInitFileName);

protected:
	virtual void InitLua();

private:
	LuaIntf::LuaContext m_luaCtx;
	std::unique_ptr<CServerMailBoxMgr> m_pSvrMbMgr;
	std::unique_ptr<CCfgReader> m_pCfg;
	CAsioServer* m_pServer = nullptr;
	std::unique_ptr<CRedis> m_pRedis;
};

#endif  // __CWorld__BASE__HEAD__
