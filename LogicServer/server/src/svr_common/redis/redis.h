// Author: Jin Qing (http://blog.csdn.net/jq0123)

#ifndef SVR_COMMON_REDIS_H
#define SVR_COMMON_REDIS_H

#include "asio/asio_fwd.h"  // for io_service

#include <cstdint>  // for uint16_t
#include <memory>  // for unique_ptr
#include <string>
#include "../../../deps/hiredis-for-windows_490671/hiredis/hiredis.h";
#include <iostream>
namespace RedisCluster {
	class BoostAsioAdapter;
}

// Async redis operations.
class CRedis
{
public:
	CRedis();
	virtual ~CRedis();

public:
	// 初始化是阻塞式的。仅第1次调用有效，忽略重复调用。
	using io_service = boost::asio::io_service;
	bool Init(io_service& rIos, const std::string& sHost, uint16_t uPort);


	bool Set(const  std::string & key, const std::string & value);
	std::string  Get(const  std::string & key);

private:
	using Adapter = RedisCluster::BoostAsioAdapter;
	std::unique_ptr<Adapter> m_pAdapter;
	redisContext  *redis = nullptr;

};

#endif  // SVR_COMMON_REDIS_H
