#pragma  once

#include <assert.h>
#include "Memory.h"
#include "Config.h"
#define  TRUE 1
#define  FALSE 0


#define  LOG_FUNC_NAME Utils::log(__FUNCTION__);

#define ZT_ASSERT(cond,what) assert(cond)

//是否开启多线程
#define  ENABLE_MULTI_THREAD FALSE

//是否开启 内存泄露检测，开启多线程时 此设置无效
#define  MEMORY_TRACE TRUE

//参数指针检查
#define  PTR_PARAM_CHECK_RETURN(param,return_what)  if(!param)return return_what;

//一条消息网络传输结尾
#define  NET_MSG_ENG_LINE '^'


//一次通信最大buffer size
#define  MAX_BUFFERS 2048