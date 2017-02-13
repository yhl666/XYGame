#ifndef SVR_COMMON_UTIL_H
#define SVR_COMMON_UTIL_H

#include <cstdint>  // for uint16_t
class CRedis;
namespace Util {

bool SetServerIdEnv(uint16_t uServerId);
uint16_t GetServerId();  // 导出到 lua get_server_id()
CRedis &GetRedis();

}  // namespace Util
#endif  // SVR_COMMON_UITL_H
