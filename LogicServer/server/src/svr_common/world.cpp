/*----------------------------------------------------------------
// Copyright (C) 2016 苏州，顾宏
//
// 模块名：CWorld
// 创建者：Macro Gu
// 修改者列表：
// 创建日期：2016.1.11
// 模块描述：消息转发， mailbox 列表维护， 部分消息触发等
//----------------------------------------------------------------*/

#include "world.h"

#ifndef _WIN32
#include <termios.h>
#endif

#include "asio/asio_server.h"  // for GetServerId()
#include "cfg_reader.h"  // for GetValue()
#include "client_mailbox.h"  // for CClientMailBox
#include "event_handler.h"  // for EventHandler
#include "log.h"
#include "lua/lua_game_clt_id.h"  // for LuaGameCltId
#include "lua/lua_logger.h"  // for LuaLogger
#include "lua/lua_mongodb.h"
#include "lua/lua_rpc.h"  // for LuaRpc
#include "lua/lua_rpc_call_context.h"  // for LuaRpcCallContext
#include "lua/lua_server.h"  // for LuaServer
#include "lua/lua_timer_queue.h"  // for LuaTimerQueue
#include "redis/redis.h"  // for CRedis
#include "server_mailbox.h"  // for CServerMailBox
#include "server_mailbox_mgr.h"  // for CServerMailBoxMgr
#include "lua/lua_redis.h"
#include "XYGameLog.h"
const char LOG_NAME[] = "CWorld";

CWorld::CWorld() :
	m_pSvrMbMgr(new CServerMailBoxMgr)  // unique_ptr
{
}

CWorld::~CWorld()
{
	assert(m_pSvrMbMgr);  // unique_ptr
}

void CWorld::InitMailboxMgr()
{
	assert(m_pSvrMbMgr);
	assert(m_pCfg);
	m_pSvrMbMgr->Init(*m_pCfg);
}

void CWorld::BlockingRun()
{
	assert(m_pServer);
	m_pServer->BlockingRun(GetServerPort());
}

uint16_t CWorld::GetServerPort()
{
	assert(m_pServer);
	uint16_t uSvrId = m_pServer->GetServerId();

	const ServerMailBoxList& mbs = m_pSvrMbMgr->GetSvrMailBoxes();
	auto iter = mbs.begin();
	for (; iter != mbs.end(); ++iter)
	{
		const CServerMailBox* pmb = *iter;
		if (pmb->GetServerId() == uSvrId)
		{
			return pmb->GetRemotePort();
		}
	}

	return 0;
}

bool CWorld::Init(const std::string& sCfgFilePath, CAsioServer& rSvr)
{
	m_pServer = &rSvr;
	m_pCfg.reset(new CCfgReader(sCfgFilePath));
	m_pRedis.reset(new CRedis);
	if (!m_pRedis->Init(rSvr.GetIoService(), "127.0.0.1", 7000))  // Todo: config
		return false;

	try
	{
		InitMailboxMgr();
		InitLua();
	}
	catch (const std::string& e)
	{
		LOG_FATAL("CWorld::Init() exception: " << e);
		return false;
	}
	if (!DoLuaInitFile("common/init_common.lua"))
		return false;

	return true;
}

uint16_t CWorld::GetServerId()
{
	CAsioServer* p = GetServer();
	if (p)
	{
		return p->GetServerId();
	}
	else
	{
		return 0;
	}
}

// Todo: 有些函数如 GetServerId(), GetServerMailbox() 不该是 CWorld 成员，改成 Util 函数。

CServerMailBox* CWorld::GetServerMailbox(uint16_t nServerId)
{
	CAsioServer* s = GetServer();
	if (s)
	{
		return s->GetServerMailbox(nServerId);
	}

	return nullptr;
}

CAsioServer* CWorld::GetServer()
{
	return m_pServer;
}

void CWorld::InitLua()
{
	lua_State* L = GetLuaState();
	LuaLogger::Bind(L);
	LuaRpc::Bind(L);
	LuaRpcCallContext::Bind(L);
	LuaMongoDb::Bind(L);
	LuaTimerQueue::Bind(L);
	LuaServer::Bind(L);
	LuaGameCltId::Bind(L);
	LuaRedis::Bind(L);

	XYGameLog::GetInstance()->BindLua(L);
	// lua脚本的顶层目录名
	assert(m_pCfg);
	std::string sLuaDir = m_pCfg->GetValue("lua", "lua_script_dir");
	m_luaCtx.setGlobal("G_LUA_ROOTPATH", sLuaDir);
}

// sLuaInitFileName 必须是相对于 LuaPath 目录的文件名，如 "base/init_base.lua"
bool CWorld::DoLuaInitFile(const std::string& sLuaInitFileName)
{
	// lua脚本的顶层路径名
	std::string sLuaDir = m_pCfg->GetValue("lua", "lua_script_dir");
	std::string sLuaInitFilePath =
		(Fmt("%1%/%2%") % sLuaDir % sLuaInitFileName).str();
	try {
		// 读取初始化文件, load所有的lua文件, 由init文件负责完成
		m_luaCtx.doFile(sLuaInitFilePath.c_str());
	}
	catch (LuaIntf::LuaException& e) {
		LOG_ERROR(Fmt("Failed to run '%1%', %2%") % sLuaInitFilePath % e.what());
		return false;
	}
	return true;
}

