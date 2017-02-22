#include <log4cxx/ndc.h>
#include <log4cxx/xml/domconfigurator.h>

#include "base_server.h"
#include "log.h"
#include "timer_queue.h"
#include "util.h"  // for SetServerIdEnv()
#include "world.h"
#include "world_base.h"

const char LOG_NAME[] = "main";
static CWorldBase s_worldBase;
CWorld* g_pTheWorld = &s_worldBase;

#include "ClientServer.h"






int main(int argc, char* argv[])
{
	log4cxx::NDC ndcMain("");
	if (argc < 3)
	{
		LOG_ERROR(Fmt("Usage: %s cfg_file server_id") % argv[0]);
		return -1;
	}
	//second args[2] is server id for self
	uint16_t uServerId = (uint16_t)atoi(argv[2]);
	if (!Util::SetServerIdEnv(uServerId))  // for log4cxx
		return -1;

	// Must after SetServerIdEnv().
	log4cxx::xml::DOMConfigurator::configureAndWatch("log4j/base.xml", 5000);
	LOG_INFO("-------------------------------");
	LOG_INFO(Fmt("Start base server (ID=%1%).") % uServerId);
	LOG_INFO("-------------------------------");

	// args[1] is configure file name
	const std::string sCfgFile = argv[1];
	CWorldBase& worldbase(s_worldBase);
	CBaseServer svr(uServerId, SERVER_BASE, worldbase);
	if (!worldbase.Init(sCfgFile, svr))
	{
		LOG_ERROR("World init error.");
		return -2;
	}


	//接入ClientServer
	ClientServer::GetInstance();





	worldbase.BlockingRun();
	return 0;
}
