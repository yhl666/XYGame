#include "util.h"

#include "get_world.h"  // for GetWorld()
#include "log.h"
#include "world.h"  // for GetServerId()

#include <stdio.h>  // for snprint()
#include <stdlib.h>  // for setenv()

namespace Util {

	// 不能使用GetServerId(), 只能传参，因为World还没初始化。
	bool SetServerIdEnv(uint16_t uServerId)
	{
		const char LOG_NAME[] = "SetServerIdEnv";
		static char buf[128] = { 0 };  // putenv need a buffer
		int nLen = snprintf(buf, sizeof(buf), "SERVER_ID=%u", uServerId);
		if (nLen < 0)
		{
			LOG_ERROR(Fmt("snprintf() failed. (%1%)%2%") % errno % strerror(errno));
			return false;
		}

		int nErr = putenv(buf);
		if (0 == nErr) return true;
		LOG_ERROR(Fmt("putenv() failed. (%1%)%2%") % errno % strerror(errno));
		return false;
	}

	uint16_t GetServerId()
	{
		return GetWorld()->GetServerId();
	}



	CRedis& GetRedis()
	{
		return GetWorld()->GetRedis();
	}
}